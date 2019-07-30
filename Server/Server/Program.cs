using System;

namespace GameServer
{
    public class Program
    {
        static void Main(string[] args)
        {
            GamMapManager.instance.CreateMap();
            ThreadManager.instance.AllThreadStart();
        }
    }
}
