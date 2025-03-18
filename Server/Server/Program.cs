using Server.Data;
using Server.Game.Room;
using ServerCore;
using System;
using System.Net;
using System.Threading;

namespace Server
{
    class Program
    {
        static Listener _listener = new Listener();

        static void FlushRoom()
        {
            JobTimer.Instance.Push(FlushRoom, 250);
        }

        static void Main(string[] args)
        {
            ConfigManager.LoadConfig();
            DataManager.LoadData();
            RoomManager.Instance.Add(1);

            // DNS (Domain Name System)
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            _listener.Init(endPoint, () => { return SessionManager.Instance.Generate(); });
            Console.WriteLine("Listening...");


            while (true)
            {
                var gameRoom = RoomManager.Instance.Find(1);
                gameRoom.Push(gameRoom.Update);
                Thread.Sleep(100);
            }
        }
    }
}
