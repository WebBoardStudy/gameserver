using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Server {
    struct JobTimerElem : IComparable<JobTimerElem> {
        public long execTick;
        public Action action;
        public int CompareTo(JobTimerElem other) {
            return (int)(other.execTick - this.execTick);            
        }
    }

    internal class JobTimer {
        PriorityQueue<JobTimerElem> _queue = new PriorityQueue<JobTimerElem>();
        object _lock = new object();

        public static JobTimer Instance { get; } = new JobTimer();

        public void Push(Action action, int tickAfter = 0) {
            JobTimerElem job;
            job.action = action;
            job.execTick = System.Environment.TickCount64 + tickAfter;
            lock (_lock) {
                _queue.Push(job);
            }
        }

        public void Flush() {
            while(true) {
                long now = System.Environment.TickCount64;
                JobTimerElem job;
                lock (_lock) {
                    if (_queue.Count == 0)
                        break;

                    job = _queue.Peek();
                    if (job.execTick > now) {
                        break;
                    }

                    _queue.Pop();
                }

                job.action.Invoke();
            }
        }
    }
}
