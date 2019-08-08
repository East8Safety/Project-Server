using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer
{
    public class PlayerManager
    {
        public static readonly PlayerManager instance = new PlayerManager();

        public Dictionary<int, Player> playerDic = new Dictionary<int, Player>();
        public Stack<int> playerPool = new Stack<int>();
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
            playerPool.Push(player.playerId);
        }

        //减少玩家
        public void DeletePlayer(int playerId)
        {
            for (int i = 0; i < playerPool.Count; i++)
            {
                var mPlayerId = playerPool.Pop();
                if (mPlayerId == playerId)
                {
                    break;
                }
                playerPool.Push(mPlayerId);
            }
        }

        public int GetWinner()
        {
            if (playerPool.Count == 1)
            {
                return playerPool.Pop();
            }
            return 0;
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
