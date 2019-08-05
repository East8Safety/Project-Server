using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer
{
    public class Bomb
    {
        public int weaponId { get; set; }
        public int x { get; set; }
        public int z { get; set; }
        public int damageX { get; set; }
        public int damageZ { get; set; }
        public int damage { get; set; }
    }
}
