using ServerCore;
using System.Net;

namespace DummyClient;

internal class Program {

    private static void Main(string[] args) {
        var hostName = Dns.GetHostName();
        var ipHostEntry = Dns.GetHostEntry(hostName);
        var remoteEp = new IPEndPoint(ipHostEntry.AddressList[0], 11000);

        try {
            var connector = new Connector();
            connector.Connect(remoteEp, () => new ServerSession());
        }
        catch (Exception e) {
            Console.WriteLine(e);
            throw;
        }

        while (true) {

        }

    }
}