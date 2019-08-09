using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace GameServer
{
    public class Bomb
    {
        public int id { get; set; }
        public Timer timer { get; set; }
        public int weaponId { get; set; }
        public int x { get; set; }
        public int z { get; set; }
        public int damageX { get; set; }
        public int damageZ { get; set; }
        public int damage { get; set; }
    }
}
