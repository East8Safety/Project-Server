using System;

namespace GameServer
{
    public class Program
    {
        static void Main(string[] args)
        {
            ReadConfig.instance.Read();
            ThreadManager.instance.AllThreadStart();
        }
    }
}
