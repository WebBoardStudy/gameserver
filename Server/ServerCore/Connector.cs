using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace ServerCore;

public class Connector
{
    private Func<Session>? _sessionFactory;

    public void Connect(IPEndPoint remoteEndPoint, Func<Session>? sessionFactory)
    {
        var socket = new Socket(remoteEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        _sessionFactory = sessionFactory;
        var args = new SocketAsyncEventArgs();
        args.Completed += OnConnectCompleted;
        args.RemoteEndPoint = remoteEndPoint;
        args.UserToken = socket;
        RegisterConnect(args);
    }

    private void RegisterConnect(SocketAsyncEventArgs args)
    {
        var socket = args.UserToken as Socket;
        if (socket == null) return;

        var pending = socket.ConnectAsync(args);
        if (pending == false) OnConnectCompleted(null, args);
    }

    private void OnConnectCompleted(object? sender, SocketAsyncEventArgs args)
    {
        if (args.SocketError != SocketError.Success)
        {
            Console.WriteLine($"onConnect Complete Failed! {args.SocketError}");
            return;
        }

        var session = _sessionFactory?.Invoke();

        Debug.Assert(args.ConnectSocket != null, "args.ConnectSocket != null");
        session?.Start(args.ConnectSocket);
        session?.OnConnected(args.RemoteEndPoint);
    }
}