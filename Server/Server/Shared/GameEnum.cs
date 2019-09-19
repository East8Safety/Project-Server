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
        S2CSyncItem =               1010,           //同步物品
        S2CSyncState =              1011,           //同步状态
        S2CChangeWeapon =           1012,           //转换武器

        S2CSendCharId =             1102,           //发送charId
        S2CSendClientId =           1103,           //发送客户端Id
        S2CAllCharId =              1104,           //发送所有charId
        S2CAllLocation =            1105,           //发送所有char位置
        S2CGameStart =              1106,           //游戏开始
        S2CSendWinner =             1107,           //发送获胜者
        S2CPlayerCount =            1108,           //发送服务器人数
        S2CBombNone =               1109,           //泡泡没了
        S2CBombRange =              1110,           //泡泡爆炸范围
        S2CCellBroken =             1111,           //墙裂开
        S2CInPortal =               1112,           //进传送门
        S2CGameOver =               1113,           //游戏结束
        S2CStart =                  1114,           //开始
        S2CChickenLoc =             1115,           //鸡的位置

        C2SMove =                   2001,           //客户端发送移动
        C2SAttack =                 2002,           //客户端攻击
        C2SUseItem =                2003,           //客户端使用道具
        C2SChooseChar =             2004,           //客户端选择角色
        C2SChooseLocation =         2005,           //客户端选位置
        C2SDeleteItem =             2006,           //客户端丢弃物品
        C2SChangeWeapon =           2007,           //客户端换武器
        C2SDeleteChicken =          2008,           //丢鸡
    }
}
