using System.Diagnostics;
using System.Net.Sockets;
using System.Text;

namespace ServerCore;

public class Session {
    private Socket _socket;
    private int _disconnected = 0;
    private Queue<byte[]> _sendQueue = new Queue<byte[]>();
    private object _sendLock = new object();
    private List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();
    private SocketAsyncEventArgs _recvArgs = new SocketAsyncEventArgs();
    private SocketAsyncEventArgs _sendArgs = new SocketAsyncEventArgs();


    public void Start(Socket socket) {
        _socket = socket;

        _recvArgs.Completed += OnRecvCompleted;
        _recvArgs.SetBuffer(new byte[1024], 0, 1024);
        RegisterRecv();

        _sendArgs.Completed += OnSendCompleted;
    }

    private void OnRecvCompleted(object? sender, SocketAsyncEventArgs args) {
        if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success) {
            try {
                Debug.Assert(args.Buffer != null, "args.Buffer != null");
                var recvData = Encoding.UTF8.GetString(args.Buffer, args.Offset, args.BytesTransferred);
                Console.WriteLine($"[From Client] : {recvData}");
            }
            catch (Exception e) {
                Console.WriteLine($"OnRecvCompleted failed {e}");
            }
        }
        else {
            Disconnect();
        }
    }

    private void RegisterRecv() {
        var pending = _socket.ReceiveAsync(_recvArgs);
        if (pending == false) OnRecvCompleted(null, _recvArgs);
    }

    public void Disconnect() {
        if (Interlocked.Exchange(ref _disconnected, 1) == 1) return;
        _socket.Shutdown(SocketShutdown.Both);
        _socket.Close();
    }

    public void Send(byte[] sendBuffer) {

        lock (_sendLock) {
            _sendQueue.Enqueue(sendBuffer);
            if (_pendingList.Count == 0) {
                RegisterSend();
            }
        }
    }

    private void RegisterSend() {

        while (_sendQueue.Count > 0) {
            byte[] sendBuffer = _sendQueue.Dequeue();
            _pendingList.Add(sendBuffer);
        }

        _sendArgs.BufferList = _pendingList;

        bool pending = _socket.SendAsync(_sendArgs);
        if (pending == false) {
            OnSendCompleted(null, _sendArgs);
        }
    }

    private void OnSendCompleted(object? value, SocketAsyncEventArgs args) {
        lock (_sendLock) {
            if (args.SocketError == SocketError.Success && args.BytesTransferred > 0) {
                try {
                    _sendArgs.BufferList = null;
                    _pendingList.Clear();

                    Console.WriteLine($"Transfered bytes: {args.BytesTransferred}");

                    if (_sendQueue.Count > 0) {
                        RegisterSend();
                    }
                }
                catch (Exception ex) {
                    Console.WriteLine($"OnSendCompleted failed {ex}");
                }

            }
            else {
                Disconnect();
            }
        }
    }


}