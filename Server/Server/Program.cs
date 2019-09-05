using System;

namespace GameServer
{
    public class Program
    {
        static void Main(string[] args)
        {
            ReadConfig.instance.Read();
            Server.instance.Init();
            ThreadManager.instance.AllThreadStart();
        }
    }
}
