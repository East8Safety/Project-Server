using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer
{
    public class PlayerManager
    {
        public static readonly PlayerManager instance = new PlayerManager();

        public Dictionary<int, Player> playerDic = new Dictionary<int, Player>();
        public int chooseLocationCount = 0;

        //创建玩家
        public void CreatePlayer(int clientId)
        {
            var player = PlayerController.instance.Create();
            PlayerController.instance.Init(player);
            player.clientId = clientId;
            Server.instance.clientId2PlayerId.TryAdd(player.clientId, player.playerId);
            AddPlayer(player);
        }

        //增加玩家
        public void AddPlayer(Player player)
        {
            playerDic.TryAdd(player.playerId, player);
        }

        //获取player实例
        public Player GetPlayer(int playerId)
        {
            Player player;
            playerDic.TryGetValue(playerId, out player);
            return player;
        }
    }
}
