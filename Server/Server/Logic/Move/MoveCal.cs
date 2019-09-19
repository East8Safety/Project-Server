using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer
{
    public class MoveCal
    {
        public static readonly MoveCal instance = new MoveCal();

        //计算是否能移动
        public bool IsCanMove(Player player, int x, int z)
        {
            GameMap gameMap;
            GameMapManager.instance.mapDic.TryGetValue(0, out gameMap);

            if (gameMap.gameMap[player.x, player.z] >= 100001)
            {
                if (gameMap.gameMap[x, z] == gameMap.gameMap[player.x, player.z])
                {
                    return true;
                }
                if (gameMap.gameMap[x, z] == 0 || gameMap.gameMap[x, z] == 3001 || gameMap.gameMap[x, z] == 3002 || gameMap.gameMap[x, z] == 3003)
                {
                    return true;
                }
                if (gameMap.gameMap[x, z] >= 1001 && gameMap.gameMap[x, z] <= 3000)
                {
                    return true;
                }
            }
            else
            {
                if (gameMap.gameMap[x, z] == 0 || gameMap.gameMap[x, z] == 3001 || gameMap.gameMap[x, z] == 3002 || gameMap.gameMap[x, z] == 3003)
                {
                    return true;
                }
                if (gameMap.gameMap[x, z] >= 1001 && gameMap.gameMap[x, z] <= 3000)
                {
                    return true;
                } 
            }
            return false;
        }

        public int CalToward(C2SMove model)
        {
            int toward = 1;

            if (model.x >= 0)
            {
                if (model.z >= 0)
                {
                    if (model.x >= model.z)
                    {
                        toward = 2;
                    }
                    else
                    {
                        toward = 1;
                    }
                }
                else if (model.z < 0)
                {
                    if (model.x >= -model.z)
                    {
                        toward = 2;
                    }
                    else
                    {
                        toward = 3;
                    }
                }
            }
            else if (model.x < 0)
            {
                if (model.z >= 0)
                {
                    if (-model.x >= model.z)
                    {
                        toward = 4;
                    }
                    else
                    {
                        toward = 1;
                    }
                }
                else if (model.z < 0)
                {
                    if (-model.x >= -model.z)
                    {
                        toward = 4;
                    }
                    else
                    {
                        toward = 3;
                    }
                } 
            }
            return toward;
        }
    }
}
