using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer
{
    public class Character
    {
        public int typeId { get; set; }
        public int charId { get; set; }
        public int clientId { get; set; }
        public float x { get; set; }
        public float z { get; set; }
        public float locationX { get; set; }
        public float locationZ { get; set; }

        public int HP { get; set; }
        public float speed { get; set; }
        public int weaponInHand { get; set; }
    }
}
