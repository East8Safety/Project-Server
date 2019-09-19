using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace GameServer
{
    class EventManager
    {
        public static readonly EventManager instance = new EventManager();
        
        //等待执行的事件
        public Queue<Action> waitEvents = new Queue<Action>();
        
        //开启主逻辑线程
        public void Start()
        {
            Action ac;
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
                    }
                }
                else
                {
                    Thread.Sleep(2);
                }
            }
        }

        //加入事件
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
