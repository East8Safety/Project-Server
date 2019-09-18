using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace GameServer
{
    public class ReadConfig
    {
        public static readonly ReadConfig instance = new ReadConfig();

        public static int map1Width = 48;
        public static int map1Hight = 48;
        public static int map2Width = 48;
        public static int map2Hight = 48;
        public static int map3Width = 48;
        public static int map3Hight = 48;
        public int[,] map1;
        public int[,] map2;
        public int[,] map3;
        public int[,] itemMap1;
        public int[,] itemMap2;
        public int[,] itemMap3;
        public int[,] groundMap1;
        public int[,] groundMap2;
        public int[,] groundMap3;

        public int charCountToStart = 1;
        public List<int[]> mapRandom = new List<int[]>();
        public Dictionary<int, int> itemCount = new Dictionary<int, int>();
        public Dictionary<int, ConfigPlayer> configPlayers = new Dictionary<int, ConfigPlayer>();
        public int timeToChooseLocation = 5;
        public Dictionary<int, int> ItemId2Value = new Dictionary<int, int>();
        public int portalTime = 5;
        public int bombTime = 3;
        public int portalPlayerCount = 8;

        //读取配置文件
        public void Read()
        {
            Init();
            ReadMap();
            ReadXML();

            ConsoleLog.instance.Info("配置读取完毕");
        }

        public void Init()
        {
            itemMap1 = new int[map1Width, map1Hight];
            itemMap2 = new int[map2Width, map2Hight];
            itemMap3 = new int[map3Width, map3Hight];

            groundMap1 = new int[map1Width, map1Hight];
            for (int i = 0; i < map1Width; i++)
            {
                for (int j = 0; j < map1Hight; j++)
                {
                    groundMap1[i, j] = 100;
                }
            }
            groundMap2 = new int[map2Width, map2Hight];
            groundMap3 = new int[map3Width, map3Hight];
        }

        public void ReadMap()
        {
            map1 = new int[map1Width, map1Hight];
            string path1 = "../../../Config/map1.txt";
            StreamReader reader1 = new StreamReader(path1);
            string line1 = "";
            string mapStr1 = "";
            while ((line1 = reader1.ReadLine()) != null)
            {
                mapStr1 += line1;
            }
            var mapArray1 = mapStr1.Split('|');
            int x1 = 0;
            for (int i = 0; i < map1Width; i++)
            {
                for (int j = 0; j < map1Hight; j++)
                {
                    map1[i, j] = int.Parse(mapArray1[x1]);
                    x1++;
                }
            }
            //for (int i = map1Width - 1; i >= 0; i--)
            //{
            //    for (int j = 0; j < map1Hight; j++)
            //    {
            //        map1[j, i] = int.Parse(mapArray1[x1]);
            //        x1++;
            //    }
            //}

            map2 = new int[map2Width, map2Hight];
            string path2 = "../../../Config/map2.txt";
            StreamReader reader2 = new StreamReader(path2);
            string line2 = "";
            string mapStr2 = "";
            while ((line2 = reader2.ReadLine()) != null)
            {
                mapStr2 += line2;
            }
            var mapArray2 = mapStr2.Split('|');
            int x2 = 0;
            for (int i = 0; i < map2Width; i++)
            {
                for (int j = 0; j < map2Hight; j++)
                {
                    map2[i, j] = int.Parse(mapArray2[x2]);
                    x2++;
                }
            }
            //for (int i = map2Width - 1; i >= 0; i--)
            //{
            //    for (int j = 0; j < map2Hight; j++)
            //    {
            //        map2[j, i] = int.Parse(mapArray2[x2]);
            //        x2++;
            //    }
            //}

            map3 = new int[map3Width, map3Hight];
            string path3 = "../../../Config/map3.txt";
            StreamReader reader3 = new StreamReader(path3);
            string line3 = "";
            string mapStr3 = "";
            while ((line3 = reader3.ReadLine()) != null)
            {
                mapStr3 += line3;
            }
            var mapArray3 = mapStr3.Split('|');
            int x3 = 0;
            for (int i = 0; i < map3Width; i++)
            {
                for (int j = 0; j < map3Hight; j++)
                {
                    map1[i, j] = int.Parse(mapArray3[x3]);
                    x3++;
                }
            }
            //for (int i = map3Width - 1; i >= 0; i--)
            //{
            //    for (int j = 0; j < map3Hight; j++)
            //    {
            //        map3[j, i] = int.Parse(mapArray3[x3]);
            //        x3++;
            //    }
            //}
        }

        public void SetPlayerLocation(Player player)
        {
            var x = player.x;
            var z = player.z;

            if (player.xBefore == x && player.zBefore == z)
            {
                return;
            }

            if (player.mapValueBefore == -1)
            {
                player.xBefore = x;
                player.zBefore = z;
                player.mapValueBefore = map1[x, z];
                map1[x, z] = 0;
                return;
            }
            
            map1[player.xBefore, player.zBefore] = player.mapValueBefore;
            player.xBefore = x;
            player.zBefore = z;
            player.mapValueBefore = map1[x, z];
            map1[x, z] = 0;
        }

        public static void ReadXML()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("../../../Config/Config.xml");

            XmlElement rootElem = doc.DocumentElement;
            XmlNodeList ItemMapNodes = rootElem.GetElementsByTagName("ItemMap");
            XmlNodeList CharNodes = rootElem.GetElementsByTagName("Char");
            XmlNodeList ItemNodes = rootElem.GetElementsByTagName("Item");
            XmlNodeList damageCommonNodes = rootElem.GetElementsByTagName("damageCommon");

            foreach (var item in ItemMapNodes)
            {
                int id = int.Parse(((XmlElement)item).GetAttribute("id"));
                int count = int.Parse(((XmlElement)item).GetAttribute("count"));
                ReadConfig.instance.itemCount.Add(id, count);
            }

            foreach (var item in CharNodes)
            {
                int charid = int.Parse(((XmlElement)item).GetAttribute("charid"));
                int hp = int.Parse(((XmlElement)item).GetAttribute("hp"));
                int hpMax = int.Parse(((XmlElement)item).GetAttribute("hpMax"));
                int speed = int.Parse(((XmlElement)item).GetAttribute("speed"));
                int speedMax = int.Parse(((XmlElement)item).GetAttribute("speedMax"));
                int damage = int.Parse(((XmlElement)item).GetAttribute("damage"));
                int damageMax = int.Parse(((XmlElement)item).GetAttribute("damageMax"));

                ConfigPlayer configPlayer = new ConfigPlayer();
                configPlayer.charid = charid;
                configPlayer.hp = hp;
                configPlayer.hpMax = hpMax;
                configPlayer.speed = speed;
                configPlayer.speedMax = speedMax;
                configPlayer.damage = damage;
                configPlayer.damageMax = damageMax;
                ReadConfig.instance.configPlayers.Add(charid, configPlayer);
            }

            foreach (var item in ItemNodes)
            {
                int id = int.Parse(((XmlElement)item).GetAttribute("id"));
                int value = int.Parse(((XmlElement)item).GetAttribute("value"));
                ReadConfig.instance.ItemId2Value.Add(id, value);
            }
        }
    }
}
