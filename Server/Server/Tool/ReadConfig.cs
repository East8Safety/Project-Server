using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace GameServer
{
    public class ReadConfig
    {
        public static readonly ReadConfig instance = new ReadConfig();

        public int[,] map = new int[10, 10];
        public int charCountToStart = 1;
        public int gameStartDelay = 3;
        public float cellSize = 10;

        //读取配置文件
        public void Read()
        {
            ReadMap();
            ReadOther();

            ConsoleLog.instance.Info("配置读取完毕");
        }

        public void ReadMap()
        {
            string path = "../../../Config/map.txt";
            StreamReader reader = new StreamReader(path);
            string line = "";
            string mapStr = "";
            while ((line = reader.ReadLine()) != null)
            {
                mapStr += line;
            }

            var mapArray = mapStr.Split('|');
            int x = 0;
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    map[i, j] = int.Parse(mapArray[x]);
                    x++;
                }
            }
        }

        //读取地图配置
        public void ReadMap1()
        {
            string jsonfile = "../../../Config/map.json";
            JObject o = (JObject)JToken.ReadFrom(new JsonTextReader(File.OpenText(jsonfile)));
            var str = o["Map"].ToString().Replace("\r\n", "");
            JArray array1 = JArray.Parse(str);
            for (int i = 0; i < array1.Count; ++i)
            {
                JArray array2 = JArray.Parse(array1[i].ToString());
                for (int j = 0; j < array2.Count; ++j)  //遍历JArray  
                {
                    map[i,j] = int.Parse(array2[j].ToString());
                }
            }
        }

        public void ReadOther()
        {
            Table.ItemId2Damage.TryAdd(2001, 30);
        }
    }
}
