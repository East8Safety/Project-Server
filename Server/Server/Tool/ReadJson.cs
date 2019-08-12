﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GameServer
{
    public class ReadJson
    {
        public static readonly ReadJson instance = new ReadJson();

        public int[,] map = new int[10, 10];
        public int charCountToStart = 4;
        public int gameStartDelay = 3;
        public float cellSize = 2;

        //读取配置文件
        public void ReadConfig()
        {
            ReadMap();

            ConsoleLog.instance.Info("配置读取完毕");
        }

        //读取地图配置
        public void ReadMap()
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
    }
}
