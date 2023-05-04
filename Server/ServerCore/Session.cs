using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerCore;

public abstract class Session
{
    private Socket? _socket;
    private int _disconnected = 0;
    private Queue<byte[]> _sendQueue = new();
    private object _sendLock = new();
    private List<ArraySegment<byte>> _pendingList = new();
    private SocketAsyncEventArgs _recvArgs = new();
    private SocketAsyncEventArgs _sendArgs = new();

    public abstract void OnConnected(EndPoint endPoint);
    public abstract void OnRecv(ArraySegment<byte> buffer);
    public abstract void OnSend(int numOfBytes);
    public abstract void OnDisconnected(EndPoint? endPoint);

    public void Start(Socket? socket)
    {
        _socket = socket;

        _recvArgs.Completed += OnRecvCompleted;
        _recvArgs.SetBuffer(new byte[1024], 0, 1024);
        RegisterRecv();

        _sendArgs.Completed += OnSendCompleted;
    }

    public void Disconnect()
    {
        if (Interlocked.Exchange(ref _disconnected, 1) == 1) return;
        OnDisconnected(_socket?.RemoteEndPoint);
        _socket?.Shutdown(SocketShutdown.Both);
        _socket?.Close();
    }

    private void RegisterRecv()
    {
        var pending = _socket != null && _socket.ReceiveAsync(_recvArgs);
        if (pending == false) OnRecvCompleted(null, _recvArgs);
    }


    private void OnRecvCompleted(object? sender, SocketAsyncEventArgs args)
    {
        if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
            try
            {
                Debug.Assert(args.Buffer != null, "args.Buffer != null");
                OnRecv(new ArraySegment<byte>(args.Buffer, args.Offset, args.BytesTransferred));
                RegisterRecv();
            }
            catch (Exception e)
            {
                Console.WriteLine($"OnRecvCompleted failed {e}");
            }
        else
            Disconnect();
    }


    public void Send(byte[] sendBuffer)
    {
        lock (_sendLock)
        {
            _sendQueue.Enqueue(sendBuffer);
            if (_pendingList.Count == 0) RegisterSend();
        }
    }

    private void RegisterSend()
    {
        while (_sendQueue.Count > 0)
        {
            var sendBuffer = _sendQueue.Dequeue();
            _pendingList.Add(sendBuffer);
        }

        _sendArgs.BufferList = _pendingList;

        var pending = _socket?.SendAsync(_sendArgs);
        if (pending == false) OnSendCompleted(null, _sendArgs);
    }

    private void OnSendCompleted(object? value, SocketAsyncEventArgs args)
    {
        lock (_sendLock)
        {
            if (args.SocketError == SocketError.Success && args.BytesTransferred > 0)
                try
                {
                    _sendArgs.BufferList = null;
                    _pendingList.Clear();

                    OnSend(args.BytesTransferred);

                    if (_sendQueue.Count > 0) RegisterSend();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"OnSendCompleted failed {ex}");
                }
            else
                Disconnect();
        }
    }
}