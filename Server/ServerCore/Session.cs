using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerCore;

public abstract class PacketSession : Session {
    public static readonly int HeaderSize = sizeof(ushort);

    public sealed override int OnRecv(ArraySegment<byte> buffer) {
        int processLen = 0;
        while (true) {
            // 최소 헤더 파싱 여부
            if (buffer.Count < HeaderSize) {
                break;
            }

            ushort dataSize = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
            if (dataSize > buffer.Count)
                break;

            OnRecvPacket(new ArraySegment<byte>(buffer.Array, buffer.Offset, dataSize));

            processLen += dataSize;
            buffer = new ArraySegment<byte>(buffer.Array, buffer.Offset + dataSize, buffer.Count - dataSize);
        }
        return processLen;
    }

    public abstract void OnRecvPacket(ArraySegment<byte> buffer);

}

public abstract class Session
{
    private Socket _socket;
    private int _disconnected = 0;
    RecvBuffer _recvBuffer = new RecvBuffer(1024);
    private Queue<ArraySegment<byte>> _sendQueue = new();
    private object _sendLock = new();
    private List<ArraySegment<byte>> _pendingList = new();
    private SocketAsyncEventArgs _recvArgs = new();
    private SocketAsyncEventArgs _sendArgs = new();

    public abstract void OnConnected(EndPoint endPoint);
    public abstract int OnRecv(ArraySegment<byte> buffer);
    public abstract void OnSend(int numOfBytes);
    public abstract void OnDisconnected(EndPoint endPoint);

    public void Start(Socket socket)
    {
        _socket = socket;

        _recvArgs.Completed += OnRecvCompleted;
        _sendArgs.Completed += OnSendCompleted;

        RegisterRecv();
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
        _recvBuffer.Clean();
        var segment = _recvBuffer.WriteSegment;
        _recvArgs.SetBuffer(segment.Array, segment.Offset, segment.Count);
        var pending = _socket != null && _socket.ReceiveAsync(_recvArgs);
        if (pending == false) OnRecvCompleted(null, _recvArgs);
    }


    private void OnRecvCompleted(object sender, SocketAsyncEventArgs args)
    {
        if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
            try
            {
                // Write 커서 이동
                if (_recvBuffer.OnWrite(args.BytesTransferred) == false) {
                    Disconnect();
                    return;
                }

                Debug.Assert(args.Buffer != null, "args.Buffer != null");
                int processLen = OnRecv(_recvBuffer.ReadSegment);
                if (processLen < 0 || _recvBuffer.DataSize < processLen) {
                    Disconnect();
                    return;
                }

                if (_recvBuffer.OnRead(processLen) == false) {
                    Disconnect();
                    return;
                }

                RegisterRecv();
            }
            catch (Exception e)
            {
                Console.WriteLine($"OnRecvCompleted failed {e}");
            }
        else
            Disconnect();
    }


    public void Send(ArraySegment<byte> sendBuffer)
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

    private void OnSendCompleted(object value, SocketAsyncEventArgs args)
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