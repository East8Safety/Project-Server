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
        public int charId { get; set; }
    }

    [ProtoContract]
    public class S2CSendClientId
    {
        [ProtoMember(1)]
        public int clientId { get; set; }
    }

    [ProtoContract]
    public class S2CHPChange
    {
        [ProtoMember(1)]
        public int charId { get; set; }
        [ProtoMember(2)]
        public int nowHP { get; set; }
    }

    [ProtoContract]
    public class S2CDie
    {
        [ProtoMember(1)]
        public int charId { get; set; }
    }

    [ProtoContract]
    public class S2CAllCharId
    {
        [ProtoMember(1)]
        public Dictionary<int, int> charId2CharType { get; set; }
    }

    [ProtoContract]
    public class S2CAllLocation
    {
        [ProtoMember(1)]
        public Dictionary<int, Location> allLocation { get; set; }
    }

    [ProtoContract]
    public class S2CAttack
    {
        [ProtoMember(1)]
        public int weaponId { get; set; }
        [ProtoMember(2)]
        public int x { get; set; }
        [ProtoMember(3)]
        public int z { get; set; }
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
        public int nowHp { get; set; }
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
        public int charType { get; set; }
    }

    [ProtoContract]
    public class C2SChooseLocation
    {
        [ProtoMember(1)]
        public float x { get; set; }
        [ProtoMember(2)]
        public float z { get; set; }
    }

    #endregion
}
