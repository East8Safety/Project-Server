using System;

namespace GameServer
{
    public class Program
    {
        static void Main(string[] args)
        {
            GameMapManager.instance.CreateMap();
            ThreadManager.instance.AllThreadStart();
        }
    }
}
