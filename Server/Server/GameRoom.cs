using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server {
    class GameRoom {
        List<ClientSession> _sessions = new List<ClientSession>();
        object _lock = new object();

        public void Enter(ClientSession session) {
            lock (_lock) {
                _sessions.Add(session);
                session.Room = this;
            }
        }

        public void Leave(ClientSession session) {
            lock (_lock) {
                _sessions.Remove(session);
            }
        }

        internal void Broadcast(ClientSession clientSession, string chat) {
            S_Chat pk = new S_Chat();
            pk.playerId = clientSession.SessionID;
            pk.chat = chat + $" I am {pk.playerId}";            
            
            var segment = pk.Write();
            lock (_lock) {
                foreach (ClientSession session in _sessions) {
                    session.Send(segment);
                }
            }
        }
    }
}
