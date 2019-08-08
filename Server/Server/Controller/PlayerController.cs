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
            Player player = new Player();
            playerGuid++;
            player.playerId = playerGuid;
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
                PlayerManager.instance.DeletePlayer(player.playerId);
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
