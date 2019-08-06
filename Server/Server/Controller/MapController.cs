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

        //初始化地图
        public void Init(GameMap gameMap, int width, int height)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    gameMap.gameMap[i, j] = ReadJson.instance.map[i, j];
                }
            }
        }

        //地形收到伤害
        public void Damage(GameMap gameMap, int x, int z, int damage)
        {
            if(gameMap.gameMap[x, z] - damage <= 0)
            {
                gameMap.gameMap[x, z] = 0;
                GameProcess.instance.SendCellChange(gameMap.mapId, x, z, gameMap.gameMap[x, z]);
            }
            else
            {
                gameMap.gameMap[x, z] -= damage;
                GameProcess.instance.SendCellChange(gameMap.mapId, x, z, gameMap.gameMap[x, z]);
            }
        }
    }
}
