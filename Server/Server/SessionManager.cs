using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server {
    internal class SessionManager {
        static SessionManager _instance = new SessionManager();
        public static SessionManager Instance { get { return _instance; } }

        int _sessionId = 0;
        Dictionary<int, ClientSession> _sessionMap = new Dictionary<int, ClientSession>();
        object _lock = new object();

        public ClientSession Generate() {
            lock (_lock) {
                ClientSession session = new ClientSession();
                session.SessionID = _sessionId++;
                _sessionMap.Add(session.SessionID, session);
                Console.WriteLine($"Generateed : {session.SessionID}");
                return session;
            }
        }

        public ClientSession Find(int id) {
            lock (_lock) {
                ClientSession session = null;
                _sessionMap.TryGetValue(id, out session);
                return session;
            }
        }

        public void Remove(ClientSession session) {
            lock (_lock) {
                _sessionMap.Remove(session.SessionID);
            }
        }
    }
}
