using System;

namespace GameServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Server.instance.ThreadStart();
            Server.instance.ThreadSendStart();
            Server.instance.ThreadReceiveStart();

            ServerUpdate.instance.ThreadStart();
            ThreadManager.instance.ThreadStart();

            EventManager.instance.ThreadStart();
        }
    }
}
