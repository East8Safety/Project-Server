using System;

namespace GameServer
{
    public class Program
    {
        static void Main(string[] args)
        {
            GameMapManager.instance.CreateMap(100, 100);
            ThreadManager.instance.AllThreadStart();
        }
    }
}
