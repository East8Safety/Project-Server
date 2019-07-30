using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer
{
    public enum messageType
    {
        S2CMove = 1001,                 //服务器发送移动
        S2CAttack = 1002,
        S2CUseProp = 1003,
        S2CGetItem = 1004,
        S2CDelItem = 1005,
        S2CRefreshItemNum = 1006,

        S2CJoinNewPlayer = 1101,        //新玩家charId
        S2CSendCharId = 1102,           //发送客户端自己的charId

        C2SMove = 2001,                 //客户端发送移动
        C2SAttack = 2002,
        C2SUseItem = 2003,
    }
}
