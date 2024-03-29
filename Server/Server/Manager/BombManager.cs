﻿using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer
{
    public class BombManager
    {
        public static readonly BombManager instance = new BombManager();

        public Dictionary<int, Bomb> bombDic = new Dictionary<int, Bomb>();

        public Bomb CreateBomb(Player player, int x, int z)
        {
            Bomb bomb = BombController.instance.Create(player, x, z);
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

        public void DeleteBomb(int id)
        {
            bombDic.Remove(id);
        }
    }
}
