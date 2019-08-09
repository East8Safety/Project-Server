using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace GameServer
{
    public class BombController
    {
        public static readonly BombController instance = new BombController();

        public Dictionary<int, int> playerId2Damage = new Dictionary<int, int>();
        private int bombGuid = 0;

        public Bomb Create(int weaponId, int x, int z)
        {
            Bomb bomb = new Bomb();
            bomb.id = bombGuid;
            bomb.weaponId = weaponId;
            bomb.x = x;
            bomb.z = z;
            bombGuid++;

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
                BombController.instance.PlayerDamage(BombController.instance.playerId2Damage);

                ConsoleLog.instance.Info(string.Format("炸弹爆炸,武器Id: {0},炸弹位置: {1} {2}", bomb.weaponId, bomb.x, bomb.z));
            });
        }

        //计算伤害范围
        public void BombRange(Bomb bomb)
        {
            bomb.timer.Change(Timeout.Infinite, Timeout.Infinite);

            GameMap gameMap = GameMapManager.instance.GetGameMap(0);
            for (int i = bomb.x - bomb.damageX; i < bomb.x + 1 + bomb.damageX; i++)
            {
                if (i < 0 || i >= gameMap.width)
                {
                    continue;
                }
                if (gameMap.gameMap[i, bomb.z] >= 1001 && gameMap.gameMap[i, bomb.z] <= 2000)
                {
                    int playerId = gameMap.gameMap[i, bomb.z];

                    if (playerId2Damage.ContainsKey(playerId))
                    {
                        playerId2Damage[playerId] += bomb.damage;
                    }
                    else
                    {
                        playerId2Damage[playerId] = bomb.damage;
                    }
                }
                else if (gameMap.gameMap[i, bomb.z] >= 1 && gameMap.gameMap[i, bomb.z] <= 1000)
                {
                    MapController.instance.Damage(gameMap, i, bomb.z, bomb.damage);
                }
                else if (gameMap.gameMap[i, bomb.z] >= 3001 && gameMap.gameMap[i, bomb.z] <= 4000)
                {
                    var bombId = gameMap.gameMap[i, bomb.z];
                    Bomb newBomb = BombManager.instance.GetBomb(bombId);
                    BombRange(newBomb);
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
                    int playerId = gameMap.gameMap[bomb.x, i];

                    if (playerId2Damage.ContainsKey(playerId))
                    {
                        playerId2Damage[playerId] += bomb.damage;
                    }
                    else
                    {
                        playerId2Damage[playerId] = bomb.damage;
                    }
                }
                else if (gameMap.gameMap[bomb.x, i] >= 1 && gameMap.gameMap[bomb.x, i] <= 1000)
                {
                    MapController.instance.Damage(gameMap, bomb.x, i, bomb.damage);
                }
                else if (gameMap.gameMap[bomb.x, i] >= 3001 && gameMap.gameMap[bomb.x, i] <= 4000)
                {
                    var bombId = gameMap.gameMap[bomb.x, i];
                    Bomb newBomb = BombManager.instance.GetBomb(bombId);
                    BombRange(newBomb);
                }
            }

            bomb = null;
        }

        public void PlayerDamage(Dictionary<int, int> playerId2Damage)
        {
            foreach (var item in playerId2Damage)
            {
                var playerId = item.Key;
                var damage = item.Value;

                PlayerController.instance.Damage(PlayerManager.instance.GetPlayer(playerId), damage);
            }

            playerId2Damage.Clear();
        }
    }
}
