using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameClient
{
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
    public class AllCharLocation
    {
        [ProtoMember(1)]
        public Dictionary<int, Location> allCharLocation { get; set; }
    }

    //S2CJoinNewPlayer用 新玩家charId
    [ProtoContract]
    public class NewCharId
    {
        [ProtoMember(1)]
        public int charId { get; set; }
    }

    //S2CSendCharId用 分配给客户端的charId
    [ProtoContract]
    public class CharId
    {
        [ProtoMember(1)]
        public int charId { get; set; }
    }

    //C2SMove用 客户端发来的移动信息
    [ProtoContract]
    public class Move
    {
        [ProtoMember(1)]
        public float x { get; set; }
        [ProtoMember(2)]
        public float z { get; set; }
    }
}
