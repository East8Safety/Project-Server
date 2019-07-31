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

    //S2CMove用 所有人位置信息
    [ProtoContract]
    public class S2CMoveModel
    {
        [ProtoMember(1)]
        public Dictionary<int, Location> allCharLocation { get; set; }
    }

    //S2CJoinNewPlayer用 新玩家charId
    [ProtoContract]
    public class S2CJoinNewPlayerModel
    {
        [ProtoMember(1)]
        public int charId { get; set; }
    }

    //S2CSendCharId用 分配给客户端的charId
    [ProtoContract]
    public class S2CSendCharIdModel
    {
        [ProtoMember(1)]
        public int clientId { get; set; }
        [ProtoMember(2)]
        public int charId { get; set; }
    }

    //C2SMove用 客户端发来的移动信息
    [ProtoContract]
    public class C2SMoveModel
    {
        [ProtoMember(1)]
        public float x { get; set; }
        [ProtoMember(2)]
        public float z { get; set; }
    }

    [ProtoContract]
    //C2SAttack用 客户端发来的攻击消息
    public class C2SAttackModel
    {
        [ProtoMember(1)]
        public int weaponId { get; set; }
        [ProtoMember(2)]
        public float locationX { get; set; }
        [ProtoMember(3)]
        public float locationZ { get; set; }
    }
}
