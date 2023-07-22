using ServerCore;
using System.Net;

namespace Server;

internal class Program {

    private static Listener _listener = new();
    public static GameRoom gameRoom = new GameRoom();

    static void FlushRoom() {
        gameRoom.Push(() => gameRoom.Flush());
        JobTimer.Instance.Push(FlushRoom, 250);
    }

    private static void Main(string[] args) {        
        var host = Dns.GetHostName();
        var hostEntry = Dns.GetHostEntry(host);
        var ipAddress = hostEntry.AddressList[0];
        var ipEndPoint = new IPEndPoint(ipAddress, 11000);

        _listener.Init(ipEndPoint, SessionManager.Instance.Generate);
        Console.WriteLine("Listening...");

        JobTimer.Instance.Push(FlushRoom);

        while (true) {
            
            JobTimer.Instance.Flush();
        }
    }
}