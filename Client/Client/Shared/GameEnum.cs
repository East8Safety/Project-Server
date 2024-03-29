﻿using System;
using System.Collections.Generic;
using System.Text;

namespace GameClient
{
    public enum messageType
    {
        S2CMove = 1001,           //服务器发送移动
        S2CAttack = 1002,           //服务器发送攻击
        S2CUseProp = 1003,
        S2CGetItem = 1004,
        S2CDelItem = 1005,
        S2CRefreshItemNum = 1006,
        S2CHPChange = 1007,           //血量变化
        S2CDie = 1008,           //角色死亡

        S2CJoinNewPlayer = 1101,           //新玩家charId
        S2CSendCharId = 1102,           //发送客户端自己的charId
        S2CSendClientId = 1103,           //发送客户端Id
        S2CAllCharId = 1104,           //发送所有charId
        S2CAllLocation = 1105,           //发送所有char位置

        C2SMove = 2001,           //客户端发送移动
        C2SAttack = 2002,
        C2SUseItem = 2003,
        C2SChooseChar = 2004,           //客户端选择角色
        C2SChooseLocation = 2005,           //客户端选位置
    }
}