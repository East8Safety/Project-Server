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

            //System.Threading.Timer timer = new System.Threading.Timer(new System.Threading.TimerCallback(t),null,3*1000, System.Threading.Timeout.Infinite);
            //ConsoleLog.instance.Info("start");

            //Console.ReadKey();
        }

        public static void t(object state)
        {
            ConsoleLog.instance.Info("trigger");
        }
    }
}
