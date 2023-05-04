using System.Net;
using System.Net.Sockets;

namespace ServerCore;

public class Listener
{
    private Socket? _listenSocket;
    private Func<Session>? _sessionFactory;

    public void Init(IPEndPoint ipEndPoint, Func<Session> factoryFunc)
    {
        _listenSocket = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        _listenSocket.Bind(ipEndPoint);
        _listenSocket.Listen(10);

        _sessionFactory = factoryFunc;

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
            try
            {
                var newSession = _sessionFactory?.Invoke() ??
                                 throw new NullReferenceException("Session Factory output null");
                newSession.Start(args.AcceptSocket ?? throw new NullReferenceException("args.AcceptSocket is null"));
                newSession.OnConnected(args.AcceptSocket.RemoteEndPoint ??
                                       throw new NullReferenceException("args.AcceptSocket.RemoteEndPoint is null"));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"OnAcceptCompleted : {ex}");
            }
        else
            Console.WriteLine(args.SocketError.ToString());

        RegisterAccept(args);
    }
}