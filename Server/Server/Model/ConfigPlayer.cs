using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Model
{
    public class ConfigPlayer
    {
        public int charid { get; set; }
        public int hp { get; set; }
        public int hpMax { get; set; }
        public int speed { get; set; }
        public int speedMax { get; set; }
        public float damage { get; set; }
        public float damageMax { get; set; }
    }
}
