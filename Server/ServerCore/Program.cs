using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerCore;

internal class Program
{
    private static Listener _listener = new();

    private static void Main(string[] args)
    {
        var host = Dns.GetHostName();
        var hostEntry = Dns.GetHostEntry(host);
        var ipAddress = hostEntry.AddressList[0];
        var ipEndPoint = new IPEndPoint(ipAddress, 11000);

    }
}