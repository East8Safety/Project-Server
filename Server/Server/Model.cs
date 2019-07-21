using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer
{
    [ProtoContract]
    public class Model
    {
        [ProtoMember(1)]
        public int UserId;
        [ProtoMember(2)]
        public string UserName;
        [ProtoMember(3)]
        public string Message;
    }
}
