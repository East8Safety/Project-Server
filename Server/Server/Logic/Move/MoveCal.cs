using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer
{
    public class MoveCal
    {
        public static readonly MoveCal instance = new MoveCal();

        //计算是否能移动
        public bool IsCanMove(int x, int z)
        {
            GameMap gameMap;
            GameMapManager.instance.mapDic.TryGetValue(0, out gameMap);
            if (gameMap.gameMap[x,z] == 0)
            {
                return true;
            }
            if (gameMap.gameMap[x, z] >= 1001 && gameMap.gameMap[x, z] <= 2000)
            {
                return true;
            }
            return false;
        }
    }
}
