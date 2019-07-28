using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer
{
    public enum messageType
    {
        S2CMove = 1001,                 //服务器发送移动
        S2CUpdateChar = 1101,           //更新所有人的charId
        S2CSendCharId = 1102,           //发送客户端自己的charId

        C2SMove = 2001,                 //客户端发送移动
    }
}
