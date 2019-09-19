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

        public void GenerateChicken(GameMap map, int width, int hight)
        {
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

            int mapRandomIndex = random.Next(0, ReadConfig.instance.mapRandomChicken.Count);
            var x = ReadConfig.instance.mapRandomChicken[mapRandomIndex][0];
            var z = ReadConfig.instance.mapRandomChicken[mapRandomIndex][1];

            map.gameMap[x, z] = 3003;

            ReadConfig.instance.mapRandom.Remove(ReadConfig.instance.mapRandomChicken[mapRandomIndex]);
        }
    }
}
