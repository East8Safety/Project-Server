using System;
using System.Collections.Generic;
using System.Text;

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
            Player player = new Player { itemId2Count = new Dictionary<int, int>() };
            playerGuid++;
            player.playerId = playerGuid;
            player.HP = 100;
            return player;
        }

        //初始化玩家
        public void Init(Player player)
        {
            player.x = 0;
            player.z = 0;
            player.locationX = 0;
            player.locationZ = 0;
            player.HP = 100;
            player.speed = 1;
        }

        //玩家受到伤害
        public void Damage(Player player, int damage)
        {
            player.HP -= damage;

            //角色死亡
            if (player.HP <= 0)
            {
                S2CDie s2CDie = new S2CDie();
                s2CDie.playerId = player.playerId;
                GameProcess.instance.SendCharDie(s2CDie);
                var cellX = CDT2Cell.instance.CDT2X(player.locationX);
                var cellZ = CDT2Cell.instance.CDT2Z(player.locationZ);
                GameMap gameMap = GameMapManager.instance.GetGameMap(0);
                MapController.instance.SetMapValue(gameMap, cellX, cellZ, 0);
                GameProcess.instance.SendMapChange(cellX, cellZ, 0);
                PlayerManager.instance.DeletePlayer(player.playerId);
                ConsoleLog.instance.Info(string.Format("玩家死亡 playerId:{0}", player.playerId));
                int winnerId = PlayerManager.instance.GetWinner();
                if (winnerId != 0)
                {
                    GameProcess.instance.SendWinner(winnerId);
                }
                return;
            }

            S2CHPChange s2CHPChange = new S2CHPChange();
            s2CHPChange.playerId = player.playerId;
            s2CHPChange.nowHP = player.HP;
            GameProcess.instance.SendHPChange(s2CHPChange);
            ConsoleLog.instance.Info(string.Format("玩家收到伤害 playerId:{0} 伤害值:{1}", player.playerId, damage));
        }

        //设置玩家位置
        public void SetLocation(Player player, float x, float z)
        {
            player.locationX = x;
            player.locationZ = z;
        }

        //设置玩家
        public void SetCharId(Player player, int charId)
        {
            player.charId = charId;
        }
    }
}
