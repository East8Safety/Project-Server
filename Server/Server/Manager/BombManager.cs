using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer
{
    public class BombManager
    {
        public static readonly BombManager instance = new BombManager();

        private Dictionary<int, Bomb> bombDic = new Dictionary<int, Bomb>();

        public Bomb CreateBomb(int weaponId, int x, int z)
        {
            Bomb bomb = BombController.instance.Create(weaponId, x, z);
            AddBomb(bomb.id, bomb);
            return bomb;
        }

        public void AddBomb(int id, Bomb bomb)
        {
            bombDic.TryAdd(id, bomb);
        }

        public Bomb GetBomb(int id)
        {
            Bomb bomb;
            bombDic.TryGetValue(id, out bomb);
            return bomb;
        }
    }
}
