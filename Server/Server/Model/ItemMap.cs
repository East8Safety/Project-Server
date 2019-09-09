using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer
{
    public class ItemMap
    {
        public int mapId { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public int[,] itemMap { get; set; }
    }

    public class GroundMap
    {
        public int mapId { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public int[,] groundMap { get; set; }
    }
}
