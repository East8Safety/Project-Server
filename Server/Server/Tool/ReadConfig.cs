﻿using Newtonsoft.Json;
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
        public static int map3Width = 36;
        public static int map3Hight = 36;
        public int[,] map1;
        public int[,] map2;
        public int[,] map3;
        public int[,] itemMap1;
        public int[,] itemMap2;
        public int[,] itemMap3;
        public int[,] groundMap1;
        public int[,] groundMap2;
        public int[,] groundMap3;

        public int charCountToStart = 5;
        public List<int[]> mapRandom = new List<int[]>();
        public List<int[]> mapRandomChicken = new List<int[]>();
        public List<int[]> mapRandomPortal = new List<int[]>();
        public Dictionary<int, int> itemCount1 = new Dictionary<int, int>();
        public Dictionary<int, int> itemCount2 = new Dictionary<int, int>();
        public Dictionary<int, int> itemCount3 = new Dictionary<int, int>();
        public Dictionary<int, ConfigPlayer> configPlayers = new Dictionary<int, ConfigPlayer>();
        public int timeToChooseLocation = 2;
        public Dictionary<int, int> ItemId2Value = new Dictionary<int, int>();
        public int portalTime = 3;
        public int bombTime = 3;
        public int portalPlayerCount = 5;
        public int gameEndTime = 2 * 60;
        public int gameInitTime = 5;
        public int chickenGameTime = 180;
        public int portalGameTime = 180;
        public int canChickenDis = 8;
        public int deBuffNumber = 10;

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
                    groundMap1[i, j] = 15;
                }
            }
            groundMap2 = new int[map2Width, map2Hight];
            for (int i = 0; i < map2Width; i++)
            {
                for (int j = 0; j < map2Hight; j++)
                {
                    groundMap2[i, j] = 15;
                }
            }
            groundMap3 = new int[map3Width, map3Hight];
            for (int i = 0; i < map3Width; i++)
            {
                for (int j = 0; j < map3Hight; j++)
                {
                    groundMap3[i, j] = 15;
                }
            }
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

        public void SetPlayerLocation(Player player, ref int[,] map)
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
                player.mapValueBefore = map[x, z];
                map[x, z] = 0;
                return;
            }

            map[player.xBefore, player.zBefore] = player.mapValueBefore;
            player.xBefore = x;
            player.zBefore = z;
            player.mapValueBefore = map[x, z];
            map[x, z] = 0;
        }

        public static void ReadXML()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("../../../Config/Config.xml");

            XmlElement rootElem = doc.DocumentElement;
            XmlNodeList ItemMapNodes1 = rootElem.GetElementsByTagName("ItemMap1");
            XmlNodeList ItemMapNodes2 = rootElem.GetElementsByTagName("ItemMap2");
            XmlNodeList ItemMapNodes3 = rootElem.GetElementsByTagName("ItemMap3");
            XmlNodeList CharNodes = rootElem.GetElementsByTagName("Char");
            XmlNodeList ItemNodes = rootElem.GetElementsByTagName("Item");

            foreach (var item in ItemMapNodes1)
            {
                int id = int.Parse(((XmlElement)item).GetAttribute("id"));
                int count = int.Parse(((XmlElement)item).GetAttribute("count"));
                ReadConfig.instance.itemCount1.Add(id, count);
            }

            foreach (var item in ItemMapNodes2)
            {
                int id = int.Parse(((XmlElement)item).GetAttribute("id"));
                int count = int.Parse(((XmlElement)item).GetAttribute("count"));
                ReadConfig.instance.itemCount2.Add(id, count);
            }

            foreach (var item in ItemMapNodes3)
            {
                int id = int.Parse(((XmlElement)item).GetAttribute("id"));
                int count = int.Parse(((XmlElement)item).GetAttribute("count"));
                ReadConfig.instance.itemCount3.Add(id, count);
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
