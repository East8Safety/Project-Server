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
        public int chooseCharCount = 0;
        public int chooseLocationCount = 0;
        public Dictionary<int, C2SMove> playerMove = new Dictionary<int, C2SMove>();

        //创建玩家
        public void CreatePlayer(int clientId)
        {
            var player = PlayerController.instance.Create();
            PlayerController.instance.Init(player);
            player.clientId = clientId;
            Server.instance.clientId2PlayerId.TryAdd(player.clientId, player.playerId);
            AddPlayer(player);

            ConsoleLog.instance.Info(string.Format("创建玩家 playerId:{0}", player.playerId));
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
            int playerCount = playerPool.Count;
            for (int i = 0; i < playerCount; i++)
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
