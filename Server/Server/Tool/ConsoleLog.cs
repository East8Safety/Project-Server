using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer
{
    public class ConsoleLog
    {
        public static readonly ConsoleLog instance = new ConsoleLog();

        public void Info(string info)
        {
            string s = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss:fff");
            s += " ";
            s += info;
            Console.WriteLine(s);
        }
    }
}