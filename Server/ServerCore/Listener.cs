using System.Net;
using System.Net.Sockets;

namespace ServerCore;

public class Listener
{
    private Socket? _listenSocket;

    public void Init(IPEndPoint ipEndPoint)
    {
        _listenSocket = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        _listenSocket.Bind(ipEndPoint);
        _listenSocket.Listen(10);
    }

    public Socket Accept()
    {
        return _listenSocket?.Accept()!;
    }
}