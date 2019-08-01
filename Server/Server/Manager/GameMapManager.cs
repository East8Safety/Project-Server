using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer
{
    public class GameMapManager
    {
        public static readonly GameMapManager instance = new GameMapManager();

        public Dictionary<int, GameMap> mapDic = new Dictionary<int, GameMap>();

        public void CreateMap(int width, int height)
        {
            GameMap gameMap = MapController.instance.Create(width, height);
            MapController.instance.Init(gameMap, width, height);

            mapDic.TryAdd(gameMap.mapId, gameMap);

            ConsoleLog.instance.Info(string.Format("创建地图,地图Id: {0}", gameMap.mapId));
        }
    }
}
