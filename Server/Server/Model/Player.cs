﻿using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer
{
    public class Player
    {
        public int charId { get; set; }
        public int playerId { get; set; }
        public int clientId { get; set; }
        public int x { get; set; }
        public int z { get; set; }
        public float locationX { get; set; }
        public float locationZ { get; set; }

        public int HP { get; set; }
        public float speed { get; set; }
        public int weaponInHand { get; set; }

        public Dictionary<int, int> itemId2Count { get; set; }
    }
}
