using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace GameServer
{
    public class PlayerController
    {
        public static readonly PlayerController instance = new PlayerController();

        //玩家递增Id
        public static int playerGuid = 1000;

        //创建玩家
        public Player Create()
        {
            Player player = new Player { index2ItemId = new Dictionary<int, int>() };
            playerGuid++;
            player.playerId = playerGuid;
            return player;
        }

        //初始化玩家
        public void Init(Player player)
        {
            player.x = -1;
            player.z = -1;
            player.locationX = 0;
            player.locationZ = 0;
            player.mapValueBefore = -1;
            player.xBefore = -1;
            player.zBefore = -1;
            player.toward = 1;
            player.damageCommon1 = 14;
            player.damageCommon2 = 14;
            player.bombCount = 20;
            player.isHaveChicken = false;
            player.debuff = 0;
        }

        //玩家受到伤害
        public void Damage(Player player, int damage)
        {
            if ((player.shield - damage) > 0)
            {
                player.shield -= damage;
                return;
            }
            else if ((player.shield - damage) <= 0)
            {
                player.shield = 0;
                damage -= player.shield;
            }

            player.HP -= damage;

            //角色死亡
            if (player.HP <= 0)
            {
                GameProcess.instance.SendCharDie(player.playerId);
                PlayerManager.instance.DeletePlayer(player.playerId);
                ConsoleLog.instance.Info(string.Format("玩家死亡 playerId:{0}", player.playerId));

                Box box = new Box() { index2ItemId = new Dictionary<int, int>() };
                box.boxId = player.playerId;
                box.x = player.x;
                box.z = player.z;
                box.index2ItemId = player.index2ItemId;
                GameMapManager.instance.boxDic.Add(box.boxId, box);
                GameMap gameMap = GameMapManager.instance.GetGameMap(0);
                MapController.instance.SetMapValue(gameMap, player.x, player.z, box.boxId);

                if (PlayerManager.instance.playerDic.Count == 1)
                {
                    foreach (var item in PlayerManager.instance.playerDic)
                    {
                        GameProcess.instance.SendWinner(item.Key);
                    }
                }
                return;
            }

            GameProcess.instance.SyncState(player);
            //GameProcess.instance.SendHPChange(player);
            ConsoleLog.instance.Info(string.Format("玩家收到伤害 playerId:{0} 伤害值:{1}", player.playerId, damage));
        }

        //设置玩家位置
        public void SetLocation(Player player, int x, int z)
        {
            player.x = x;
            player.z = z;
            player.locationX = CDT2Cell.instance.CDX2T(x);
            player.locationZ = CDT2Cell.instance.CDZ2T(z);
            player.locationXB = player.locationX;
            player.locationZB = player.locationZ;
        }

        //设置玩家
        public void SetCharId(Player player, int charId)
        {
            player.charId = charId;

            ConfigPlayer configPlayer = ReadConfig.instance.configPlayers[charId];
            player.HP = configPlayer.hp;
            player.HPMax = configPlayer.hpMax;
            player.speed = configPlayer.speed;
            player.speedMax = configPlayer.speedMax;
            player.damage = configPlayer.damage;
            player.damageMax = configPlayer.damageMax;
        }

        public void SendMoveTimes(int playerId)
        {
            while (true)
            {
                Player player = PlayerManager.instance.GetPlayer(playerId);
                int speed = 0;
                lock (PlayerManager.instance.playerMove)
                {
                    C2SMove c2SMove;
                    if (PlayerManager.instance.playerMove.TryGetValue(playerId, out c2SMove))
                    {
                        int toward = MoveCal.instance.CalToward(c2SMove);

                        switch (toward)
                        {
                            case 1:
                                c2SMove.z = 0.1F;
                                c2SMove.x = 0;
                                break;
                            case 2:
                                c2SMove.x = 0.1F;
                                c2SMove.z = 0;
                                break;
                            case 3:
                                c2SMove.z = -0.1F;
                                c2SMove.x = 0;
                                break;
                            case 4:
                                c2SMove.x = -0.1F;
                                c2SMove.z = 0;
                                break;
                            default:
                                break;
                        }

                        EventManager.instance.AddEvent(() =>
                        {
                            GameProcess.instance.ClientMove(playerId, c2SMove);
                        });

                        PlayerManager.instance.playerMove.Remove(playerId);
                        continue;
                    }
                    if (player != null)
                    {
                        player.locationXB = player.locationX;
                        player.locationZB = player.locationZ;
                        speed = 40 - player.speed;
                        speed += player.debuff;
                    }
                    else
                    {
                        speed = 30;
                    }
                }

                Thread.Sleep(speed);
            }
        }
    }
}
