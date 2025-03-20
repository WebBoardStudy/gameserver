using Server.Data;
using Server.Game.Room;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Net;

namespace Server
{
    class Program
    {
        static Listener _listener = new Listener();
        static List<System.Timers.Timer> _timers = new List<System.Timers.Timer>();

        static void TickRoom(GameRoom room, int tick = 50)
        {
            var timer = new System.Timers.Timer();
            timer.Interval = tick;
            timer.Elapsed += (s, e) => room.Update();
            timer.AutoReset = true;
            timer.Enabled = true;
            _timers.Add(timer);
        }

        private static void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            throw new NotImplementedException();
        }

        static void Main(string[] args)
        {
            ConfigManager.LoadConfig();
            DataManager.LoadData();
            var gameRoom = RoomManager.Instance.Add(1);
            TickRoom(gameRoom, 50);

            // DNS (Domain Name System)
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            _listener.Init(endPoint, () => { return SessionManager.Instance.Generate(); });
            Console.WriteLine("Listening...");

            while (true)
            {
                var t = Console.ReadKey();
                if (t.Key == ConsoleKey.Escape)
                    break;
            }
        }
    }
}
