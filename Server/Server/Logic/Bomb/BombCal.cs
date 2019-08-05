﻿using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer
{
    public class BombCal
    {
        public static readonly BombCal instance = new BombCal();

        //计算伤害范围
        public void BombRange(Bomb bomb)
        {
            GameMap gameMap = GameMapManager.instance.GetGameMap(0);
            for (int i = bomb.x - bomb.damageX; i < bomb.x + 1 + bomb.damageX ; i++)
            {
                if(i < 0 || i >= gameMap.width)
                {
                    continue;
                }
                if(gameMap.gameMap[i,bomb.z] >= 1001 && gameMap.gameMap[i, bomb.z] <= 2000)
                {
                    //int charId = gameMap.gameMap[i, bomb.z];
                    //CharacterController.instance.Damage(CharacterManager.instance.GetCharacter(charId), bomb.damage);
                }
                else if (gameMap.gameMap[i, bomb.z] >= 1 && gameMap.gameMap[i, bomb.z] <= 1000)
                {

                }
            }

            for (int i = bomb.z - bomb.damageZ; i < bomb.z + 1 + bomb.damageZ; i++)
            {
                if (i < 0 || i >= gameMap.height)
                {
                    continue;
                }
                if (gameMap.gameMap[bomb.x, i] >= 1001 && gameMap.gameMap[bomb.x, i] <= 2000)
                {
                    //int charId = gameMap.gameMap[bomb.x, i];
                    //CharacterController.instance.Damage(CharacterManager.instance.GetCharacter(charId), bomb.damage);
                }
                else if (gameMap.gameMap[bomb.x, i] >= 1 && gameMap.gameMap[bomb.x, i] <= 1000)
                {

                }
            }
        }
    }
}
