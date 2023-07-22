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
            connector.Connect(remoteEp, () => { return SessionManager.Instance.Generate(); }, 100);
        }
        catch (Exception e) {
            Console.WriteLine(e);
            throw;
        }

        while (true) {
            try {
                SessionManager.Instance.SendForEach();

            } catch (Exception e) {
                Console.WriteLine(e);
            }
            Thread.Sleep(250);
        }

    }
}