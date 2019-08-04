using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer
{
    public class MapController
    {
        public static readonly MapController instance = new MapController();

        public static int Guid = 0;

        //创建地图
        public GameMap Create(int width, int height)
        {
            GameMap gameMap = new GameMap();
            gameMap.mapId = Guid;
            gameMap.gameMap = new int[width, height];
            Guid++;
            return gameMap;
        }

        //初始化地图
        public void Init(GameMap gameMap, int width, int height)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    gameMap.gameMap[i, j] = ReadJson.instance.map[i,j];
                }
            }
        }
    }
}
