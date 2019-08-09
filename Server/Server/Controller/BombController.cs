using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer
{
    public class BombController
    {
        public static readonly BombController instance = new BombController();

        public Bomb Create(int weaponId, int x, int z)
        {
            Bomb bomb = new Bomb();
            bomb.weaponId = weaponId;
            bomb.x = x;
            bomb.z = z;

            return bomb;
        }

        //炸弹触发
        public void BombTrigger(object state)
        {
            EventManager.instance.AddEvent(() =>
            {
                Bomb bomb = (Bomb)state;
                GameMap gameMap = GameMapManager.instance.GetGameMap(0);
                gameMap.gameMap[bomb.x, bomb.z] = 0;

                BombRange(bomb);

                ConsoleLog.instance.Info(string.Format("炸弹爆炸,武器Id: {0},炸弹位置: {1} {2}", bomb.weaponId, bomb.x, bomb.z));
            });
        }

        //计算伤害范围
        public void BombRange(Bomb bomb)
        {
            GameMap gameMap = GameMapManager.instance.GetGameMap(0);
            for (int i = bomb.x - bomb.damageX; i < bomb.x + 1 + bomb.damageX; i++)
            {
                if (i < 0 || i >= gameMap.width)
                {
                    continue;
                }
                if (gameMap.gameMap[i, bomb.z] >= 1001 && gameMap.gameMap[i, bomb.z] <= 2000)
                {
                    int charId = gameMap.gameMap[i, bomb.z];
                    PlayerController.instance.Damage(PlayerManager.instance.GetPlayer(charId), bomb.damage);
                }
                else if (gameMap.gameMap[i, bomb.z] >= 1 && gameMap.gameMap[i, bomb.z] <= 1000)
                {
                    MapController.instance.Damage(gameMap, i, bomb.z, bomb.damage);
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
                    int charId = gameMap.gameMap[bomb.x, i];
                    PlayerController.instance.Damage(PlayerManager.instance.GetPlayer(charId), bomb.damage);
                }
                else if (gameMap.gameMap[bomb.x, i] >= 1 && gameMap.gameMap[bomb.x, i] <= 1000)
                {
                    MapController.instance.Damage(gameMap, bomb.x, i, bomb.damage);
                }
            }
        }
    }
}
