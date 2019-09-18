using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace GameServer
{
    public class PlayerManager
    {
        public static readonly PlayerManager instance = new PlayerManager();

        public Dictionary<int, Player> playerDic = new Dictionary<int, Player>();
        public Queue<int> playerPool = new Queue<int>();
        public int chooseCharCount = 0;
        public int chooseLocationCount = 0;
        public Dictionary<int, C2SMove> playerMove = new Dictionary<int, C2SMove>();
        public Dictionary<int, Player> nextPlayers = new Dictionary<int, Player>();

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
            playerPool.Enqueue(player.playerId);
        }

        //减少玩家
        public void DeletePlayer(int playerId)
        {
            playerDic.Remove(playerId);

            int playerCount = playerPool.Count;
            for (int i = 0; i < playerCount; i++)
            {
                var mPlayerId = playerPool.Dequeue();
                if (mPlayerId == playerId)
                {
                    break;
                }
                playerPool.Enqueue(mPlayerId);
            }

            playerMove.Remove(playerId);

            int clientId = 0;
            foreach (var item in Server.instance.clientId2PlayerId)
            {
                if (item.Value == playerId)
                {
                    clientId = item.Key;
                    break;
                }
            }
            Server.instance.clientPools.Remove(clientId);

            Server.instance.clientId2PlayerId.Remove(clientId);

            if (playerPool.Count <= 0)
            {
                GameProcess.instance.GameOver();
            }
        }

        public int GetWinner()
        {
            int playerId;
            if (playerPool.Count == 1)
            {
                playerPool.TryDequeue(out playerId);
                return playerId;
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
