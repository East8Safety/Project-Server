using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer
{
    public enum messageType
    {
        S2CMove =                   1001,           //服务器发送移动
        S2CAttack =                 1002,           //服务器发送攻击
        S2CUseItem =                1003,           //服务器使用道具
        S2CGetItem =                1004,           //获得物品
        S2CDeleteItem =             1005,           //服务器丢弃道具
        S2CRefreshItemNum =         1006,
        S2CHPChange =               1007,           //血量变化
        S2CDie =                    1008,           //角色死亡
        S2CCellChange =             1009,           //地形变化

        S2CSendCharId =             1102,           //发送charId
        S2CSendClientId =           1103,           //发送客户端Id
        S2CAllCharId =              1104,           //发送所有charId
        S2CAllLocation =            1105,           //发送所有char位置
        S2CGameStart =              1106,           //游戏开始
        S2CSendWinner =             1107,           //发送获胜者

        C2SMove =                   2001,           //客户端发送移动
        C2SAttack =                 2002,           //客户端攻击
        C2SUseItem =                2003,           //客户端使用道具
        C2SChooseChar =             2004,           //客户端选择角色
        C2SChooseLocation =         2005,           //客户端选位置
        C2SDeleteItem =             2006,           //客户端丢弃物品
    }
}
