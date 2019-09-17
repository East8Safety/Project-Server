﻿using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer
{
    public class CDT2Cell
    {
        public static readonly CDT2Cell instance = new CDT2Cell();

        //横坐标转换为格子
        public int CDT2X(float x)
        {
            GameMap gameMap = GameMapManager.instance.GetGameMap(0);
            int boxX = (int)x;
            if (boxX < 0)
            {
                return boxX = 0;
            }
            else if (boxX > gameMap.width)
            {
                return boxX = gameMap.width;
            }

            return boxX;
        }

        //纵坐标转换为格子
        public int CDT2Z(float z)
        {
            GameMap gameMap = GameMapManager.instance.GetGameMap(0);
            int boxZ = (int)z;
            if (boxZ < 0)
            {
                return boxZ = 0;
            }
            else if (boxZ > gameMap.height)
            {
                return boxZ = gameMap.height;
            }

            return boxZ;
        }

        public float CDX2T(int x)
        {
            return x * ReadConfig.instance.cellSize + (int)ReadConfig.instance.cellSize / 2;
        }

        public float CDZ2T(int z)
        {
            return z * ReadConfig.instance.cellSize + (int)ReadConfig.instance.cellSize / 2;
        }
    }
}
