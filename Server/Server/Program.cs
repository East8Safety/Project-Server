using System;

namespace GameServer
{
    public class Program
    {
        static void Main(string[] args)
        {
            ThreadManager.instance.AllThreadStart();
        }
    }
}
