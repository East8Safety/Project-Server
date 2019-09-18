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
        public List<BombLoc> bombLocList = new List<BombLoc>();
        private int bombGuid = 100001;

        public Bomb Create(Player player, int x, int z)
        {
            Bomb bomb = new Bomb();
            bomb.id = bombGuid;
            bomb.x = x;
            bomb.z = z;
            bomb.damageX = 3;
            bomb.damageZ = 3;
            bomb.damage = player.damage + 100;
            bombGuid++;

            return bomb;
        }

        //炸弹触发
        public void BombTrigger(object state)
        {
            EventManager.instance.AddEvent(() =>
            {
                Bomb bomb = (Bomb)state;

                BombRange(bomb);
                PlayerDamage();
                BombRange();
            });
        }

        //计算伤害范围
        public void BombRange(Bomb bomb)
        {
            bomb.timer.Change(Timeout.Infinite, Timeout.Infinite);
            GameMap gameMap = GameMapManager.instance.GetGameMap(0);
            MapController.instance.SetMapValue(gameMap, bomb.x, bomb.z, 0);
            ConsoleLog.instance.Info(string.Format("泡泡爆炸,武器Id: {0},泡泡位置: {1} {2}", bomb.weaponId, bomb.x, bomb.z));

            for (int i = bomb.x - 1; i >= bomb.x - bomb.damageX; i--)
            {
                if (i < 0 || i >= gameMap.width)
                {
                    continue;
                }

                foreach (var item in PlayerManager.instance.playerDic)
                {
                    var playerId = item.Key;
                    var player = item.Value;

                    if (player.x == i && player.z == bomb.z)
                    {
                        if (playerId2Damage.ContainsKey(playerId))
                        {
                            playerId2Damage[playerId] += bomb.damage;
                        }
                        else
                        {
                            playerId2Damage[playerId] = bomb.damage;
                        }
                    }
                }
                if (gameMap.gameMap[i, bomb.z] >= 1 && gameMap.gameMap[i, bomb.z] <= 1000)
                {
                    GroundMap groundMap = GameMapManager.instance.GetGroundMap(0);
                    MapController.instance.Damage(groundMap, i, bomb.z, bomb.damage);
                    AddBombRange(i, bomb.z);
                    break;
                }
                else if (gameMap.gameMap[i, bomb.z] >= 100001)
                {
                    AddBombRange(i, bomb.z);
                    var bombId = gameMap.gameMap[i, bomb.z];
                    Bomb newBomb = BombManager.instance.GetBomb(bombId);
                    BombRange(newBomb);
                    break;
                }
                else if (gameMap.gameMap[i, bomb.z] >= 10001 && gameMap.gameMap[i, bomb.z] <= 99999)
                {
                    break;
                }
                else if (gameMap.gameMap[i, bomb.z] >= 2001 && gameMap.gameMap[i, bomb.z] <= 3000)
                {
                    AddBombRange(i, bomb.z);
                }
                else if(gameMap.gameMap[i, bomb.z] == 0)
                {
                    AddBombRange(i, bomb.z);
                }
            }
            for (int i = bomb.x + 1; i <= bomb.x + bomb.damageX; i++)
            {
                if (i < 0 || i >= gameMap.width)
                {
                    continue;
                }

                foreach (var item in PlayerManager.instance.playerDic)
                {
                    var playerId = item.Key;
                    var player = item.Value;

                    if (player.x == i && player.z == bomb.z)
                    {
                        if (playerId2Damage.ContainsKey(playerId))
                        {
                            playerId2Damage[playerId] += bomb.damage;
                        }
                        else
                        {
                            playerId2Damage[playerId] = bomb.damage;
                        }
                    }
                }
                if (gameMap.gameMap[i, bomb.z] >= 1 && gameMap.gameMap[i, bomb.z] <= 1000)
                {
                    GroundMap groundMap = GameMapManager.instance.GetGroundMap(0);
                    MapController.instance.Damage(groundMap, i, bomb.z, bomb.damage);
                    AddBombRange(i, bomb.z);
                    break;
                }
                else if (gameMap.gameMap[i, bomb.z] >= 100001)
                {
                    AddBombRange(i, bomb.z);
                    var bombId = gameMap.gameMap[i, bomb.z];
                    Bomb newBomb = BombManager.instance.GetBomb(bombId);
                    BombRange(newBomb);
                    break;
                }
                else if (gameMap.gameMap[i, bomb.z] >= 10001 && gameMap.gameMap[i, bomb.z] <= 99999)
                {
                    break;
                }
                else if (gameMap.gameMap[i, bomb.z] >= 2001 && gameMap.gameMap[i, bomb.z] <= 3000)
                {
                    AddBombRange(i, bomb.z);
                }
                else if (gameMap.gameMap[i, bomb.z] == 0)
                {
                    AddBombRange(i, bomb.z);
                }
            }
            for (int i = bomb.z - 1; i >= bomb.z - bomb.damageZ; i--)
            {
                if (i < 0 || i >= gameMap.height)
                {
                    continue;
                }

                foreach (var item in PlayerManager.instance.playerDic)
                {
                    var playerId = item.Key;
                    var player = item.Value;

                    if (player.x == bomb.x && player.z == i)
                    {
                        if (playerId2Damage.ContainsKey(playerId))
                        {
                            playerId2Damage[playerId] += bomb.damage;
                        }
                        else
                        {
                            playerId2Damage[playerId] = bomb.damage;
                        }
                    }
                }
                if (gameMap.gameMap[bomb.x, i] >= 1 && gameMap.gameMap[bomb.x, i] <= 1000)
                {
                    GroundMap groundMap = GameMapManager.instance.GetGroundMap(0);
                    MapController.instance.Damage(groundMap, bomb.x, i, bomb.damage);
                    AddBombRange(bomb.x, i);
                    break;
                }
                else if (gameMap.gameMap[bomb.x, i] >= 100001)
                {
                    AddBombRange(bomb.x, i);
                    var bombId = gameMap.gameMap[bomb.x, i];
                    Bomb newBomb = BombManager.instance.GetBomb(bombId);
                    BombRange(newBomb);
                    break;
                }
                else if (gameMap.gameMap[bomb.x, i] >= 10001 && gameMap.gameMap[bomb.x, i] <= 99999)
                {
                    break;
                }
                else if (gameMap.gameMap[bomb.x, i] >= 2001 && gameMap.gameMap[bomb.x, i] <= 3000)
                {
                    AddBombRange(bomb.x, i);
                }
                else if (gameMap.gameMap[bomb.x, i] == 0)
                {
                    AddBombRange(bomb.x, i);
                }
            }
            for (int i = bomb.z + 1; i <= bomb.z + bomb.damageZ; i++)
            {
                if (i < 0 || i >= gameMap.height)
                {
                    continue;
                }

                foreach (var item in PlayerManager.instance.playerDic)
                {
                    var playerId = item.Key;
                    var player = item.Value;

                    if (player.x == bomb.x && player.z == i)
                    {
                        if (playerId2Damage.ContainsKey(playerId))
                        {
                            playerId2Damage[playerId] += bomb.damage;
                        }
                        else
                        {
                            playerId2Damage[playerId] = bomb.damage;
                        }
                    }
                }
                if (gameMap.gameMap[bomb.x, i] >= 1 && gameMap.gameMap[bomb.x, i] <= 1000)
                {
                    GroundMap groundMap = GameMapManager.instance.GetGroundMap(0);
                    MapController.instance.Damage(groundMap, bomb.x, i, bomb.damage);
                    AddBombRange(bomb.x, i);
                    break;
                }
                else if (gameMap.gameMap[bomb.x, i] >= 100001)
                {
                    AddBombRange(bomb.x, i);
                    var bombId = gameMap.gameMap[bomb.x, i];
                    Bomb newBomb = BombManager.instance.GetBomb(bombId);
                    BombRange(newBomb);
                    break;
                }
                else if (gameMap.gameMap[bomb.x, i] >= 10001 && gameMap.gameMap[bomb.x, i] <= 99999)
                {
                    break;
                }
                else if (gameMap.gameMap[bomb.x, i] >= 2001 && gameMap.gameMap[bomb.x, i] <= 3000)
                {
                    AddBombRange(bomb.x, i);
                }
                else if (gameMap.gameMap[bomb.x, i] == 0)
                {
                    AddBombRange(bomb.x, i);
                }
            }

            //for (int i = bomb.x - bomb.damageX; i < bomb.x + 1 + bomb.damageX; i++)
            //{
            //    if (i < 0 || i >= gameMap.width)
            //    {
            //        continue;
            //    }
            //    foreach (var item in PlayerManager.instance.playerDic)
            //    {
            //        var playerId = item.Key;
            //        var player = item.Value;

            //        if (player.x == i && player.z == bomb.z)
            //        {
            //            if (playerId2Damage.ContainsKey(playerId))
            //            {
            //                playerId2Damage[playerId] += bomb.damage;
            //            }
            //            else
            //            {
            //                playerId2Damage[playerId] = bomb.damage;
            //            }
            //        }
            //    }
            //    if (gameMap.gameMap[i, bomb.z] >= 1 && gameMap.gameMap[i, bomb.z] <= 1000)
            //    {
            //        GroundMap groundMap = GameMapManager.instance.GetGroundMap(0);
            //        MapController.instance.Damage(groundMap, i, bomb.z, bomb.damage);
            //    }
            //    else if (gameMap.gameMap[i, bomb.z] >= 10001)
            //    {
            //        var bombId = gameMap.gameMap[i, bomb.z];
            //        Bomb newBomb = BombManager.instance.GetBomb(bombId);
            //        BombRange(newBomb);
            //    }
            //}

            //for (int i = bomb.z - bomb.damageZ; i < bomb.z + 1 + bomb.damageZ; i++)
            //{
            //    if (i < 0 || i >= gameMap.height)
            //    {
            //        continue;
            //    }
            //    foreach (var item in PlayerManager.instance.playerDic)
            //    {
            //        var playerId = item.Key;
            //        var player = item.Value;

            //        if (player.x == bomb.x && player.z == i)
            //        {
            //            if (playerId2Damage.ContainsKey(playerId))
            //            {
            //                playerId2Damage[playerId] += bomb.damage;
            //            }
            //            else
            //            {
            //                playerId2Damage[playerId] = bomb.damage;
            //            }
            //        }
            //    }
            //    if (gameMap.gameMap[bomb.x, i] >= 1 && gameMap.gameMap[bomb.x, i] <= 1000)
            //    {
            //        GroundMap groundMap = GameMapManager.instance.GetGroundMap(0);
            //        MapController.instance.Damage(groundMap, bomb.x, i, bomb.damage);
            //    }
            //    else if (gameMap.gameMap[bomb.x, i] >= 10001)
            //    {
            //        var bombId = gameMap.gameMap[bomb.x, i];
            //        Bomb newBomb = BombManager.instance.GetBomb(bombId);
            //        BombRange(newBomb);
            //    }
            //}

            BombManager.instance.DeleteBomb(bomb.id);
            bomb = null;
        }

        public void PlayerDamage()
        {
            foreach (var item in playerId2Damage)
            {
                var playerId = item.Key;
                var damage = item.Value;

                PlayerController.instance.Damage(PlayerManager.instance.GetPlayer(playerId), damage);
            }

            playerId2Damage.Clear();
        }

        public void BombRange()
        {
            S2CBombRange s2CBombRange = new S2CBombRange(){BombLocList = bombLocList };
            SendData.instance.Broadcast((int)messageType.S2CBombRange, s2CBombRange);

            bombLocList.Clear();
        }

        public void AddBombRange(int x, int z)
        {
            BombLoc bombLoc = new BombLoc(){x = x, z = z};
            bombLocList.Add(bombLoc);
        }
    }
}
