using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace GameServer
{
    class EventManager
    {
        public static readonly EventManager instance = new EventManager();

        public Queue<Action> waitEvents = new Queue<Action>();
        public Thread evebtManagerThread;

        public void ThreadStart()
        {
            Action ac;
            evebtManagerThread = new Thread(() =>
            {
                while (true)
                {
                    if (waitEvents.Count > 0)
                    {
                        lock (waitEvents)
                        {
                            ac = waitEvents.Dequeue();
                            if (ac != null)
                            {
                                ac();
                            }
                            continue;
                        }
                    }
                    Thread.Sleep(10);
                }
            });
            evebtManagerThread.Name = "evebtManagerThread";
            evebtManagerThread.Start();
        }

        public void AddEvent(Action ac)
        {
            if (ac == null)
            {
                return;
            }
            lock (waitEvents)
            {
                waitEvents.Enqueue(ac);
            }
        }
    }
}
