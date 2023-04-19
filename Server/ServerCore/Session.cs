using System.Diagnostics;
using System.Net.Sockets;
using System.Text;

namespace ServerCore;

public class Session
{
    private Socket _socket;
    private int _disconnected = 0;

    public void Start(Socket socket)
    {
        _socket = socket;
        var recvArgs = new SocketAsyncEventArgs();
        recvArgs.Completed += OnRecvCompleted;
        recvArgs.SetBuffer(new byte[1024], 0, 1024);
        RegisterRecv(recvArgs);
    }

    public void Send(byte[] sendBuffer)
    {
        _socket.Send(sendBuffer);
    }

    private void OnRecvCompleted(object? sender, SocketAsyncEventArgs args)
    {
        if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
        {
            try
            {
                Debug.Assert(args.Buffer != null, "args.Buffer != null");
                var recvData = Encoding.UTF8.GetString(args.Buffer, args.Offset, args.BytesTransferred);
                Console.WriteLine($"[From Client] : {recvData}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"OnRecvCompleted failed {e}");
            }
        }
        else
        {
            // disconnect
        }
    }

    private void RegisterRecv(SocketAsyncEventArgs args)
    {
        var pending = _socket.ReceiveAsync(args);
        if (pending == false) OnRecvCompleted(null, args);
    }

    public void Disconnect()
    {
        if (Interlocked.Exchange(ref _disconnected, 1) == 1) return;
        _socket.Shutdown(SocketShutdown.Both);
        _socket.Close();
    }
}