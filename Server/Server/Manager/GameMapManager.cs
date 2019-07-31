using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer
{
    public class GameMapManager
    {
        public static readonly GameMapManager instance = new GameMapManager();

        public Dictionary<int, GameMap> mapDic = new Dictionary<int, GameMap>();
        public static int mapGuid = 0;

        public void CreateMap()
        {
            GameMap gameMap = new GameMap();
            gameMap.gameMap = new int[100, 100];
            gameMap.mapId = mapGuid;

            for (int i = 0; i < 100; i++)
            {
                for (int j = 0; j < 100; j++)
                {
                    gameMap.gameMap[i, j] = 0;
                }
            }

            mapDic.TryAdd(gameMap.mapId, gameMap);
            mapGuid++;

            ConsoleLog.instance.Info(string.Format("创建地图,地图d: {0}", gameMap.mapId));
        }
    }
}
