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

        //初始化地图
        public void Init(GameMap gameMap, int width, int height)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    gameMap.gameMap[i, j] = 0;
                }
            }
        }

        //初始化道具地图
        public void InitItemMap(ItemMap itemMap, int width, int height)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    itemMap.itemMap[i, j] = 0;
                }
            }
        }

        //地形收到伤害
        public void Damage(GameMap gameMap, int x, int z, int damage)
        {
            if(gameMap.gameMap[x, z] - damage <= 0)
            {
                var itemMap = GameMapManager.instance.GetItemMap(gameMap.mapId);
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
                gameMap.gameMap[x, z] -= damage;
                GameProcess.instance.SendCellChange(gameMap.mapId, x, z, gameMap.gameMap[x, z]);
                ConsoleLog.instance.Info(string.Format("地形收到伤害 位置:{0},{1} 伤害量{2}", x, z, damage));
            }
        }

        //地图值变化
        public void SetMapValue(GameMap gameMap, int x, int z, int value)
        {
            gameMap.gameMap[x, z] = value;
        }
    }
}
