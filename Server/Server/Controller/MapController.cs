using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer
{
    public class MapController
    {
        public static readonly MapController instance = new MapController();

        //地图自增Id
        public static int Guid = 0;
        public static int itemMapGuid = 0;
        public static int groundMapGuid = 0;

        //创建地图
        public GameMap Create(int width, int height)
        {
            GameMap gameMap = new GameMap();
            gameMap.mapId = Guid;
            gameMap.width = width;
            gameMap.height = height;
            Guid++;
            gameMap.gameMap = new int[width, height];
            return gameMap;
        }

        //创建道具地图
        public ItemMap CreateItemMap(int width, int height)
        {
            ItemMap itemMap = new ItemMap();
            itemMap.mapId = itemMapGuid;
            itemMap.width = width;
            itemMap.height = height;
            itemMapGuid++;
            itemMap.itemMap = new int[width, height];
            return itemMap;
        }

        public GroundMap CreateGroundMap(int width, int height)
        {
            GroundMap groundMap = new GroundMap();
            groundMap.mapId = groundMapGuid;
            groundMap.width = width;
            groundMap.height = height;
            groundMapGuid++;
            groundMap.groundMap = new int[width, height];
            return groundMap;
        }

        //初始化地图
        public void Init(GameMap gameMap, int width, int height, int[,] map)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    gameMap.gameMap[i, j] = map[i, j];
                }
            }
        }

        //初始化道具地图
        public void InitItemMap(ItemMap itemMap, int width, int height, int[,] map)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    itemMap.itemMap[i, j] = map[i, j];
                }
            }
        }

        public void InitGroundMap(GroundMap groundMap, int width, int height, int[,] map)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    groundMap.groundMap[i, j] = map[i, j];
                }
            }
        }

        //地形收到伤害
        public void Damage(GroundMap groundMap, int x, int z, int damage)
        {
            var gameMap = GameMapManager.instance.GetGameMap(0);
            if (groundMap.groundMap[x, z] - damage <= 0)
            {
                var itemMap = GameMapManager.instance.GetItemMap(0);
                if (itemMap.itemMap[x, z] >= 2001 && itemMap.itemMap[x, z] <= 3000)
                {
                    gameMap.gameMap[x, z] = itemMap.itemMap[x, z];
                }
                else
                {
                    gameMap.gameMap[x, z] = 0;
                }
                
                GameProcess.instance.SendCellChange(gameMap.mapId, x, z, gameMap.gameMap[x, z]);
                ConsoleLog.instance.Info(string.Format("地形破坏 位置:{0},{1}", x, z));
            }
            else
            {
                groundMap.groundMap[x, z] -= damage;
                //GameProcess.instance.SendCellChange(gameMap.mapId, x, z, gameMap.gameMap[x, z]);
                ConsoleLog.instance.Info(string.Format("地形收到伤害 位置:{0},{1} 伤害量{2}", x, z, damage));
            }
        }

        //地图值变化
        public void SetMapValue(GameMap gameMap, int x, int z, int value)
        {
            gameMap.gameMap[x, z] = value;
            GameProcess.instance.SendMapChange(x, z, value);
        }

        public int[] GetEmptyCell(Player player, GameMap gameMap, int itemId)
        {
            int[] ret = new int[2];
            int x = player.x;
            int z = player.z;

            int turns = 1;
            while (true)
            {
                x += 1;
                z += 1;
                for (int a = 0; a < turns * 2; a++)
                {
                    x += -1;
                    if (x < 0 || x > gameMap.width || z < 0 || z > gameMap.height)
                    {
                        continue;
                    }
                    if (gameMap.gameMap[x,z] == 0)
                    {
                        gameMap.gameMap[x, z] = itemId;
                        ret[0] = x; ret[1] = z;
                        return ret;
                    }
                }
                for (int b = 0; b < turns * 2; b++)
                {
                    z += -1;
                    if (x < 0 || x > gameMap.width || z < 0 || z > gameMap.height)
                    {
                        continue;
                    }
                    if (gameMap.gameMap[x, z] == 0)
                    {
                        gameMap.gameMap[x, z] = itemId;
                        ret[0] = x; ret[1] = z;
                        return ret;
                    }
                }
                for (int c = 0; c < turns * 2; c++)
                {
                    x += 1;
                    if (x < 0 || x > gameMap.width || z < 0 || z > gameMap.height)
                    {
                        continue;
                    }
                    if (gameMap.gameMap[x, z] == 0)
                    {
                        gameMap.gameMap[x, z] = itemId;
                        ret[0] = x; ret[1] = z;
                        return ret;
                    }
                }
                for (int d = 0; d < turns * 2; d++)
                {
                    z += 1;
                    if (x < 0 || x > gameMap.width || z < 0 || z > gameMap.height)
                    {
                        continue;
                    }
                    if (gameMap.gameMap[x, z] == 0)
                    {
                        gameMap.gameMap[x, z] = itemId;
                        ret[0] = x; ret[1] = z;
                        return ret;
                    }
                }
                turns++;
            }
        }
    }
}
