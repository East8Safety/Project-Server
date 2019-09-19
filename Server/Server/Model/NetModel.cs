using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer
{
    //位置坐标
    [ProtoContract]
    public class Location
    {
        [ProtoMember(1)]
        public float x { get; set; }
        [ProtoMember(2)]
        public float z { get; set; }
        [ProtoMember(3)]
        public float locationX { get; set; }
        [ProtoMember(4)]
        public float locationZ { get; set; }
        [ProtoMember(5)]
        public int toward { get; set; }
        [ProtoMember(6)]
        public bool isMove { get; set; }
    }

    //初始位置坐标
    [ProtoContract]
    public class InitLocation
    {
        [ProtoMember(1)]
        public float locatinX { get; set; }
        [ProtoMember(2)]
        public float locatinZ { get; set; }
        [ProtoMember(3)]
        public int x { get; set; }
        [ProtoMember(4)]
        public int z { get; set; }
    }

    [ProtoContract]
    public class BombLoc
    {
        [ProtoMember(1)]
        public int x { get; set; }
        [ProtoMember(2)]
        public int z { get; set; }
    }

    [ProtoContract]
    public class ChickenLoc
    {
        [ProtoMember(1)]
        public int x { get; set; }
        [ProtoMember(2)]
        public int z { get; set; }
    }

    #region S2CMessage

    //S2CMove用 所有人位置信息
    [ProtoContract]
    public class S2CMove
    {
        [ProtoMember(1)]
        public Dictionary<int, Location> allCharLocation { get; set; }
    }

    //S2CSendCharId用 分配给客户端的charId
    [ProtoContract]
    public class S2CSendCharId
    {
        [ProtoMember(1)]
        public int playerId { get; set; }
        [ProtoMember(2)]
        public int charId { get; set; }
    }

    [ProtoContract]
    public class S2CSendClientId
    {
        [ProtoMember(1)]
        public int clientId { get; set; }
        [ProtoMember(2)]
        public int playerId { get; set; }
    }

    [ProtoContract]
    public class S2CHPChange
    {
        [ProtoMember(1)]
        public int playerId { get; set; }
        [ProtoMember(2)]
        public int nowHP { get; set; }
    }

    [ProtoContract]
    public class S2CDie
    {
        [ProtoMember(1)]
        public int playerId { get; set; }
    }

    [ProtoContract]
    public class S2CAllCharId
    {
        [ProtoMember(1)]
        public Dictionary<int, int> playerId2CharId { get; set; }
    }

    [ProtoContract]
    public class S2CAllLocation
    {
        [ProtoMember(1)]
        public Dictionary<int, InitLocation> allLocation { get; set; }
    }

    [ProtoContract]
    public class S2CAttack
    {
        [ProtoMember(1)]
        public int playerId { get; set; }
    }

    [ProtoContract]
    public class S2CCellChange
    {
        [ProtoMember(1)]
        public int mapId { get; set; }
        [ProtoMember(2)]
        public int x { get; set; }
        [ProtoMember(3)]
        public int z { get; set; }
        [ProtoMember(4)]
        public int value { get; set; }
    }

    [ProtoContract]
    public class S2CGameStart
    {
        [ProtoMember(1)]
        public int placeholder { get; set; }
    }

    [ProtoContract]
    public class S2CSendWinner
    {
        [ProtoMember(1)]
        public int playerId { get; set; }
    }

    [ProtoContract]
    public class S2CGetItem
    {
        [ProtoMember(1)]
        public int playerId { get; set; }
        [ProtoMember(2)]
        public int itemId { get; set; }
        [ProtoMember(3)]
        public int count { get; set; }
    }

    [ProtoContract]
    public class S2CDeleteItem
    {
        [ProtoMember(1)]
        public int x { get; set; }
        [ProtoMember(2)]
        public int z { get; set; }
        [ProtoMember(3)]
        public int playerId { get; set; }
        [ProtoMember(4)]
        public int itemId { get; set; }
        [ProtoMember(5)]
        public int count { get; set; }
    }

    [ProtoContract]
    public class S2CUseItem
    {
        [ProtoMember(1)]
        public int playerId { get; set; }
        [ProtoMember(2)]
        public int itemId { get; set; }
    }

    [ProtoContract]
    public class S2CPlayerCount
    {
        [ProtoMember(1)]
        public int playerCount { get; set; }
    }

    [ProtoContract]
    public class S2CBombNone
    {
        [ProtoMember(1)]
        public int playerId { get; set; }
    }

    [ProtoContract]
    public class S2CSyncItem
    {
        [ProtoMember(1)]
        public int playerId { get; set; }
        [ProtoMember(2)]
        public Dictionary<int, int> index2ItemId { get; set; }
    }

    [ProtoContract]
    public class S2CSyncState
    {
        [ProtoMember(1)]
        public int playerId { get; set; }
        [ProtoMember(2)]
        public int hp { get; set; }
        [ProtoMember(3)]
        public int speed { get; set; }
        [ProtoMember(4)]
        public int shield { get; set; }
        [ProtoMember(5)]
        public int bombCount { get; set; }
    }

    [ProtoContract]
    public class S2CChangeWeapon
    {
        [ProtoMember(1)]
        public int playerId { get; set; }
        [ProtoMember(2)]
        public int weaponId { get; set; }
    }

    [ProtoContract]
    public class S2CBombRange
    {
        [ProtoMember(1)]
        public List<BombLoc> BombLocListW { get; set; }
        [ProtoMember(2)]
        public List<BombLoc> BombLocListH { get; set; }
    }

    [ProtoContract]
    public class S2CCellBroken
    {
        [ProtoMember(1)]
        public int x { get; set; }
        [ProtoMember(2)]
        public int z { get; set; }
    }

    [ProtoContract]
    public class S2CInPortal
    {
        [ProtoMember(1)]
        public int playerId { get; set; }
    }

    [ProtoContract]
    public class S2CGameOver
    {
        [ProtoMember(1)]
        public int overGame { get; set; }
    }

    [ProtoContract]
    public class S2CStart
    {
        [ProtoMember(1)]
        public int witchGame { get; set; }
    }

    [ProtoContract]
    public class S2CChickenLoc
    {
        [ProtoMember(1)]
        public List<ChickenLoc> chickenLocList { get; set; }
    }

    [ProtoContract]
    public class S2CFinalWin
    {
        [ProtoMember(1)]
        public int playerId { get; set; }
    }

    [ProtoContract]
    public class S2CAction
    {
        [ProtoMember(1)]
        public int playerId { get; set; }
    }

    #endregion

    #region C2SMessage

    //C2SMove用 客户端发来的移动信息
    [ProtoContract]
    public class C2SMove
    {
        [ProtoMember(1)]
        public float x { get; set; }
        [ProtoMember(2)]
        public float z { get; set; }
    }

    [ProtoContract]
    //C2SAttack用 客户端发来的攻击消息
    public class C2SAttack
    {
        [ProtoMember(1)]
        public int weaponId { get; set; }
        [ProtoMember(2)]
        public float locationX { get; set; }
        [ProtoMember(3)]
        public float locationZ { get; set; }
    }

    [ProtoContract]
    public class C2SChooseChar
    {
        [ProtoMember(1)]
        public int charId { get; set; }
    }

    [ProtoContract]
    public class C2SChooseLocation
    {
        [ProtoMember(1)]
        public int x { get; set; }
        [ProtoMember(2)]
        public int z { get; set; }
    }

    [ProtoContract]
    public class C2SDeleteItem
    {
        [ProtoMember(1)]
        public int index { get; set; }
        [ProtoMember(2)]
        public int itemId { get; set; }
        [ProtoMember(3)]
        public int count { get; set; }
    }

    [ProtoContract]
    public class C2SUseItem
    {
        [ProtoMember(1)]
        public int itemId { get; set; }
        [ProtoMember(2)]
        public int index { get; set; }
    }

    [ProtoContract]
    public class C2SChangeWeapon
    {
        [ProtoMember(1)]
        public int playerId { get; set; }
        [ProtoMember(2)]
        public int weaponId { get; set; }
    }

    [ProtoContract]
    public class C2SDeleteChicken
    {
        [ProtoMember(1)]
        public int playerId { get; set; }
    }

    [ProtoContract]
    public class C2SAction
    {
        [ProtoMember(1)]
        public int playerId { get; set; }
    }

    #endregion
}
