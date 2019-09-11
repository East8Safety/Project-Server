using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using GameServer.Model;

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
        public int gameStartDelay = 3;
        public float cellSize = 10;
        public List<int[]> mapRandom = new List<int[]>();
        public Dictionary<int, int> itemCount = new Dictionary<int, int>();
        public Dictionary<int, ConfigPlayer> configPlayers = new Dictionary<int, ConfigPlayer>();

        //读取配置文件
        public void Read()
        {
            Init();
            ReadMap();
            ReadOther();
            ReadXML();

            ConsoleLog.instance.Info("配置读取完毕");
        }

        public void Init()
        {
            itemMap1 = new int[map1Width, map1Hight];
            itemMap2 = new int[map2Width, map2Hight];
            itemMap3 = new int[map3Width, map3Hight];

            groundMap1 = new int[map1Width, map1Hight];
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
                    map3[i, j] = int.Parse(mapArray3[x3]);
                    x3++;
                }
            }
        }

        public void ReadOther()
        {
            Table.ItemId2Damage.TryAdd(2001, 30);
        }

        public void SetPlayerLocation(Dictionary<int, Player> playerDic)
        {
            foreach (var item in playerDic)
            {
                var player = item.Value;
                var x = player.x;
                var z = player.z;
                map1[x, z] = 0;
            }
        }

        public static void ReadXML()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("../../../Config/Config.xml");

            XmlElement rootElem = doc.DocumentElement;
            XmlNodeList ItemMapNodes = rootElem.GetElementsByTagName("ItemMap");
            XmlNodeList CharNodes = rootElem.GetElementsByTagName("Char");

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
        }
    }
}
