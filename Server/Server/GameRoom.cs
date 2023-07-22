using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server {
    class GameRoom : IJobQueue {
        List<ClientSession> _sessions = new List<ClientSession>();
        JobQueue _jobQueue = new JobQueue();
        List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();

        public void Push(Action job) {
            _jobQueue.Push(job);
        }

        public void Enter(ClientSession session) {
            _sessions.Add(session);
            session.Room = this;
        }

        public void Leave(ClientSession session) {
            _sessions.Remove(session);
        }

        public void Flush() {
            foreach (ClientSession session in _sessions) {
                session.Send(_pendingList);
            }

            Console.WriteLine($"!!!!Flushed {_pendingList.Count} items");
            _pendingList.Clear();
        }

        internal void Broadcast(ClientSession clientSession, string chat) {
            S_Chat pk = new S_Chat();
            pk.playerId = clientSession.SessionID;
            pk.chat = chat + $" I am {pk.playerId}";

            var segment = pk.Write();
            _pendingList.Add(segment);
        }
    }
}
