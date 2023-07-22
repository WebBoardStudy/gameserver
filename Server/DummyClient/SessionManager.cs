﻿using DummyClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DummyClient {
    internal class SessionManager {
        static SessionManager _instance = new SessionManager();
        public static SessionManager Instance { get { return _instance; } }

        List<ServerSession> _sessions = new List<ServerSession>();

        
        object _lock = new object();

        public ServerSession Generate() {
            lock (_lock) {
                ServerSession session = new ServerSession();
                _sessions.Add(session);                
                return session;
            }
        }

        public void SendForEach() {
            lock ( _lock) {
                foreach (ServerSession session in _sessions) {
                    C_Chat chat = new C_Chat();
                    chat.chat = $"Hello Server!";
                    ArraySegment<byte> segment = chat.Write();
                    session.Send(segment);
                }
            }
        }
    }
}
