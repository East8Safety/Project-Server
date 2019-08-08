using System;

namespace GameServer
{
    public class Program
    {
        static void Main(string[] args)
        {
            ReadJson.instance.ReadConfig();
            Server.instance.Init();
            ThreadManager.instance.AllThreadStart();
        }
    }
}
