using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace GameServer
{
    class ServerUpdate
    {
        public static readonly ServerUpdate instance = new ServerUpdate();
        public Thread serverUpdateThread;

        public void ThreadStart()
        {
            serverUpdateThread = new Thread(()=> {
                Update();
            });
            serverUpdateThread.Name = "serverUpdateThread";
            serverUpdateThread.Start();
        }

        public void Update()
        {
            while (true)
            {
                GameProcess.instance.UpdateMove();              
                Thread.Sleep(10);
            }
        }
    }
}
