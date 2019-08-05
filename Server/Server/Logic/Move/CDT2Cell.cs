using System;
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
            float a = 2;
            int boxX = (int)(x / 2) + 1;
            return boxX;
        }

        //纵坐标转换为格子
        public int CDT2Z(float z)
        {
            float a = 2;
            int boxZ = (int)(z / 2) + 1;
            return boxZ;
        }
    }
}
