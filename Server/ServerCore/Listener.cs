using System.Net;
using System.Net.Sockets;

namespace ServerCore;

public class Listener
{
    private Socket? _listenSocket;
    private Action<Socket>? _acceptHandler;

    public void Init(IPEndPoint ipEndPoint, Action<Socket>? acceptHandler)
    {
        _listenSocket = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        _listenSocket.Bind(ipEndPoint);
        _listenSocket.Listen(10);

        _acceptHandler += acceptHandler;

        var args = new SocketAsyncEventArgs();
        args.Completed += OnAcceptCompleted;
        RegisterAccept(args);
    }

    private void RegisterAccept(SocketAsyncEventArgs args)
    {
        args.AcceptSocket = null;

        var pending = _listenSocket?.AcceptAsync(args);
        if (pending == false) OnAcceptCompleted(null, args);
    }

    private void OnAcceptCompleted(object? sender, SocketAsyncEventArgs args)
    {
        if (args.SocketError == SocketError.Success)
            _acceptHandler?.Invoke(args.AcceptSocket ?? throw new InvalidOperationException());
        else
            Console.WriteLine(args.SocketError.ToString());
        RegisterAccept(args);
    }
}