using ServerCore;
using System.Net;

namespace Server;

internal class Program {

    private static Listener _listener = new();
    public static GameRoom gameRoom = new GameRoom();

    private static void Main(string[] args) {        
        var host = Dns.GetHostName();
        var hostEntry = Dns.GetHostEntry(host);
        var ipAddress = hostEntry.AddressList[0];
        var ipEndPoint = new IPEndPoint(ipAddress, 11000);

        _listener.Init(ipEndPoint, SessionManager.Instance.Generate);
        Console.WriteLine("Listening...");

        while (true) ;
    }
}