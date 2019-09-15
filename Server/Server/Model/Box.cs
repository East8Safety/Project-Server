using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer
{
    public class Box
    {
        public int boxId { get; set; }
        public int x { get; set; }
        public int z { get; set; }
        public Dictionary<int,int> index2ItemId { get; set; }
    }
}
