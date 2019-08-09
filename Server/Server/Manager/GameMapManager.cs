using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer
{
    public class GameMapManager
    {
        public static readonly GameMapManager instance = new GameMapManager();

        public Dictionary<int, GameMap> mapDic = new Dictionary<int, GameMap>();
        public Dictionary<int, ItemMap> itemMapDic = new Dictionary<int, ItemMap>();

        //创建地图
        public void CreateMap(int width, int height)
        {
            GameMap gameMap = MapController.instance.Create(width, height);
            ItemMap itemMap = MapController.instance.CreateItemMap(width, height);

            MapController.instance.Init(gameMap, width, height);
            MapController.instance.InitItemMap(itemMap, width, height);

            mapDic.TryAdd(gameMap.mapId, gameMap);
            itemMapDic.TryAdd(itemMap.mapId, itemMap);

            ConsoleLog.instance.Info(string.Format("创建地图,地图Id: {0}", gameMap.mapId));
        }

        //获取GameMap
        public GameMap GetGameMap(int mapId)
        {
            GameMap gameMap;
            mapDic.TryGetValue(mapId, out gameMap);
            return gameMap;
        }

        //获取ItemMap
        public ItemMap GetItemMap(int mapId)
        {
            ItemMap itemMap;
            itemMapDic.TryGetValue(mapId, out itemMap);
            return itemMap;
        }
    }
}
