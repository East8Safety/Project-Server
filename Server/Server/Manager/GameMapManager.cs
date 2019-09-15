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
        public Dictionary<int, GroundMap> groundMapDic = new Dictionary<int, GroundMap>();
        public Dictionary<int, Box> boxDic = new Dictionary<int, Box>();

        //创建地图
        public void CreateMap(int width, int height, int[,] configMap, int[,] configItemMap, int[,] configGroundMap)
        {
            //if (mapDic.Count > 0)
            {
                GameMap gameMap = MapController.instance.Create(width, height);
                ItemMap itemMap = MapController.instance.CreateItemMap(width, height);
                GroundMap groundMap = MapController.instance.CreateGroundMap(width, height);

                mapDic.TryAdd(gameMap.mapId, gameMap);
                itemMapDic.TryAdd(itemMap.mapId, itemMap);
                groundMapDic.TryAdd(groundMap.mapId, groundMap);
            }

            MapController.instance.Init(mapDic[0], width, height, configMap);
            MapController.instance.InitItemMap(itemMapDic[0], width, height, configItemMap);
            MapController.instance.InitGroundMap(groundMapDic[0], width, height, configGroundMap);

            ConsoleLog.instance.Info(string.Format("创建地图,地图Id: {0}", 0));
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

        public GroundMap GetGroundMap(int mapId)
        {
            GroundMap groundMap;
            groundMapDic.TryGetValue(mapId, out groundMap);
            return groundMap;
        }
    }
}
