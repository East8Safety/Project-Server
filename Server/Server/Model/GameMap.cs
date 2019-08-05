using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer
{
    public class GameMap
    {
        public int mapId { get; set; }
        public int width {get;set;}
        public int height { get; set; }
        public int[,] gameMap { get; set; }
    }
}
