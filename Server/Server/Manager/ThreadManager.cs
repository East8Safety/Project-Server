using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace GameServer
{
    public class ThreadManager
    {
        public static readonly ThreadManager instance = new ThreadManager();

        public Thread dealReceiveMsgThread;

        public void ThreadStart()
        {
            dealReceiveMsgThread = new Thread(()=>
            {
                while (true)
                {
                    if(Server.instance.messageReceived.Count > 0)
                    {
                        GameProcess.instance.ReceiveMessage();
                        continue;
                    }
                    Thread.Sleep(10);
                }
            });
            dealReceiveMsgThread.Name = "dealReceiveMsgThread";
            dealReceiveMsgThread.Start();
        }
    }
}
