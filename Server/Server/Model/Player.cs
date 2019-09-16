using System;
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
        public int HPMax { get; set; }
        public int speed { get; set; }
        public int speedMax { get; set; }
        public int damage { get; set; }
        public int damageMax { get; set; }
        public int bombCount { get; set; }
        public int xBefore { get; set; }
        public int zBefore { get; set; }
        public int mapValueBefore { get; set; }

        public int shield { get; set; }
        public int toward { get; set; }
        public int damageCommon1 { get; set; }
        public int damageCommon2 { get; set; }

        public Dictionary<int, int> index2ItemId { get; set; }
    }
}
