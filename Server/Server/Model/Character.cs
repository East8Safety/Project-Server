using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer
{
    public class Character
    {
        public int charId { get; set; }
        public int clientId { get; set; }
        public float x { get; set; }
        public float z { get; set; }
        public float locationX { get; set; }
        public float locationZ { get; set; }
        public int times { get; set; }
    }
}
