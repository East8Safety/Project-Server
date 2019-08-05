using Newtonsoft.Json;
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

        public void ReadConfig()
        {
            ReadMap();
        }

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
