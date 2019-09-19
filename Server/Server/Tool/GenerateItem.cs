using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer
{
    public class GenerateItem
    {
        public static Random random = new Random();

        public static void Generate(int[,] map, Dictionary<int, int> itemCount, int width, int hight, int mapId)
        {
            ReadConfig.instance.mapRandom.Clear();
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < hight; j++)
                {
                    if (map[i, j] > 0 && map[i, j] <= 1000)
                    {
                        int[] mapXZ = new int[2];
                        mapXZ[0] = i;
                        mapXZ[1] = j;
                        ReadConfig.instance.mapRandom.Add(mapXZ);
                    }
                }
            }

            foreach (var itemId in itemCount.Keys)
            {
                for (int i = 0; i < itemCount[itemId]; i++)
                {
                    if (ReadConfig.instance.mapRandom.Count == 0)
                    {
                        return;
                    }

                    int mapRandomIndex = random.Next(0, ReadConfig.instance.mapRandom.Count);
                    var x = ReadConfig.instance.mapRandom[mapRandomIndex][0];
                    var z = ReadConfig.instance.mapRandom[mapRandomIndex][1];
                    switch (mapId)
                    {
                        case 1:
                            ReadConfig.instance.itemMap1[x, z] = itemId;
                            break;
                        case 2:
                            ReadConfig.instance.itemMap2[x, z] = itemId;
                            break;
                        case 3:
                            ReadConfig.instance.itemMap3[x, z] = itemId;
                            break;
                        default:
                            break;
                    }
                    ReadConfig.instance.mapRandom.Remove(ReadConfig.instance.mapRandom[mapRandomIndex]);
                }
            }
        }

        public static void GenerateChicken(GameMap map, int width, int hight, ref S2CChickenLoc s2CChickenLoc)
        {
            int mid = width / 2;
            int chickenCount = 0;

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < hight; j++)
                {
                    if (map.gameMap[i, j] == 0)
                    {
                        int[] mapXZ = new int[2];
                        mapXZ[0] = i;
                        mapXZ[1] = j;
                        ReadConfig.instance.mapRandomChicken.Add(mapXZ);
                    }
                }
            }

            while (true)
            {
                if (chickenCount >= 4)
                {
                    break;
                }

                ChickenLoc chickenLoc = new ChickenLoc();

                int mapRandomIndex = random.Next(0, ReadConfig.instance.mapRandomChicken.Count);
                var x = ReadConfig.instance.mapRandomChicken[mapRandomIndex][0];
                var z = ReadConfig.instance.mapRandomChicken[mapRandomIndex][1];

                if (x < mid && x - ReadConfig.instance.canChickenDis < 0 )
                {
                    if (z < mid && z - ReadConfig.instance.canChickenDis < 0)
                    {
                        setMapvalue(map, x, z, mapRandomIndex);
                        chickenLoc.x = x;
                        chickenLoc.z = z;
                        s2CChickenLoc.chickenLocList.Add(chickenLoc);
                        chickenCount++;
                    }
                    else if (z >= mid && z + ReadConfig.instance.canChickenDis >= hight)
                    {
                        setMapvalue(map, x, z, mapRandomIndex);
                        chickenLoc.x = x;
                        chickenLoc.z = z;
                        s2CChickenLoc.chickenLocList.Add(chickenLoc);
                        chickenCount++;
                    }
                }
                else if (x >= mid && x + ReadConfig.instance.canChickenDis >= width)
                {
                    if (z < mid && z - ReadConfig.instance.canChickenDis < 0)
                    {
                        setMapvalue(map, x, z, mapRandomIndex);
                        chickenLoc.x = x;
                        chickenLoc.z = z;
                        s2CChickenLoc.chickenLocList.Add(chickenLoc);
                        chickenCount++;
                    }
                    else if (z >= mid && z + ReadConfig.instance.canChickenDis >= hight)
                    {
                        setMapvalue(map, x, z, mapRandomIndex);
                        chickenLoc.x = x;
                        chickenLoc.z = z;
                        s2CChickenLoc.chickenLocList.Add(chickenLoc);
                        chickenCount++;
                    }
                }
            }
        }

        private static void setMapvalue(GameMap map, int x, int z, int mapRandomIndex)
        {
            map.gameMap[x, z] = 3003;
            ReadConfig.instance.mapRandom.Remove(ReadConfig.instance.mapRandomChicken[mapRandomIndex]);
        }
    }
}
