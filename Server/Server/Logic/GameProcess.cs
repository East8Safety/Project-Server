using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace GameServer
{
    public class GameProcess
    {
        public static readonly GameProcess instance = new GameProcess();

        //客户端连接之后执行任务
        public void AfterConnect(int clientId)
        {
            EventManager.instance.AddEvent(() =>
            {
                //创建玩家
                PlayerManager.instance.CreatePlayer(clientId);
                //发送clientId和playerId
                SendClientId(clientId);
                SendPlayerCount(PlayerManager.instance.playerDic.Count);
            });
        }

        //发送服务器分配的clientId
        public void SendClientId(int clientId)
        {
            int playerId = Server.instance.GetPlayerId(clientId);
            S2CSendClientId s2CSendClientId = new S2CSendClientId();
            s2CSendClientId.clientId = clientId;
            s2CSendClientId.playerId = playerId;
            SendData.instance.Broadcast((int)messageType.S2CSendClientId, s2CSendClientId);
        }

        //发送服务器分配的charId
        public void SendCharId(int clientId, int charId)
        {
            int playerId = Server.instance.GetPlayerId(clientId);
            S2CSendCharId s2CSendCharId = new S2CSendCharId();
            s2CSendCharId.playerId = playerId;
            s2CSendCharId.charId = charId;
            SendData.instance.SendMessage(clientId, (int)messageType.S2CSendCharId, s2CSendCharId);
        }

        //客户端移动
        public void ClientMove(int playerId, C2SMove model)
        {
            GameMap gameMap = GameMapManager.instance.GetGameMap(0);

            Player player = PlayerManager.instance.GetPlayer(playerId);
            if (player == null)
            {
                return;
            }
            int cellX = CDT2Cell.instance.CDT2X(player.locationX + model.x);
            int cellZ = CDT2Cell.instance.CDT2Z(player.locationZ + model.z);
            if (MoveCal.instance.IsCanMove(cellX, cellZ))
            {
                if (gameMap.gameMap[cellX, cellZ] >= 2001 && gameMap.gameMap[cellX, cellZ] <= 3000)
                {
                    ItemController.instance.ChangeItemCount(player, gameMap.gameMap[cellX, cellZ], 1);
                    MapController.instance.SetMapValue(gameMap, cellX, cellZ, 0);
                    SendGetItem(playerId, gameMap.gameMap[cellX, cellZ], 1);

                    ConsoleLog.instance.Info(string.Format("Player {0} 获得道具 {1}", player.playerId, gameMap.gameMap[cellX, cellZ]));
                }

                player.locationX += model.x;
                player.locationZ += model.z;
                player.x = cellX;
                player.z = cellZ;
            }
        }

        //客户端攻击
        public void ClientAttack(C2SAttack data)
        {
            var weaponId = data.weaponId;
            var x = CDT2Cell.instance.CDT2X(data.locationX);
            var z = CDT2Cell.instance.CDT2Z(data.locationZ);

            Bomb bomb = BombManager.instance.CreateBomb(weaponId, x, z);

            GameMap gameMap = GameMapManager.instance.GetGameMap(0);
            MapController.instance.SetMapValue(gameMap, x, z, bomb.id);

            ConsoleLog.instance.Info(string.Format("角色攻击,武器Id: {0},泡泡位置: {1} {2}", weaponId, x, z));

            bomb.timer = new Timer(new TimerCallback(BombController.instance.BombTrigger), bomb, 3 * 1000, Timeout.Infinite);

            S2CAttack s2CAttack = new S2CAttack();
            s2CAttack.weaponId = weaponId;
            s2CAttack.x = x;
            s2CAttack.z = z;
            SendAttack(s2CAttack);
        }

        //客户端选人
        public void ChooseChar(int clientId, C2SChooseChar c2SChooseChar)
        {
            int playerId = Server.instance.GetPlayerId(clientId);
            var player = PlayerManager.instance.GetPlayer(playerId);
            PlayerController.instance.SetCharId(player, c2SChooseChar.charId);
            SendCharId(clientId, c2SChooseChar.charId);
            ConsoleLog.instance.Info(string.Format("Player {0} 选择角色 {1}", player.playerId, c2SChooseChar.charId));
            PlayerManager.instance.chooseCharCount++;
            if (PlayerManager.instance.chooseCharCount >= ReadConfig.instance.charCountToStart)
            {
                SendAllCharId();
                ConsoleLog.instance.Info(string.Format("所有人选人完毕"));
            }
        }

        //客户端位置
        public void ChooseLocation(int clientId, C2SChooseLocation c2SChooseLocation)
        {
            Player player = PlayerManager.instance.GetPlayer(Server.instance.GetPlayerId(clientId));
            PlayerController.instance.SetLocation(player, c2SChooseLocation.x, c2SChooseLocation.z);
            ConsoleLog.instance.Info(string.Format("Player {0} 选位置 {1},{2}", player.playerId, c2SChooseLocation.x, c2SChooseLocation.z));
            PlayerManager.instance.chooseLocationCount++;
            if (PlayerManager.instance.chooseLocationCount >= ReadConfig.instance.charCountToStart)
            {
                SendAllLocation();
                ConsoleLog.instance.Info("所有人选位置完毕");
                GameStart();
            }
        }

        //发送所有charId
        public void SendAllCharId()
        {
            S2CAllCharId s2CAllCharId = new S2CAllCharId { playerId2CharId = new Dictionary<int, int>() };

            foreach (var item in PlayerManager.instance.playerDic)
            {
                var playerId = item.Key;
                var player = item.Value;
                s2CAllCharId.playerId2CharId.TryAdd(playerId, player.charId);
            }

            SendData.instance.Broadcast((int)messageType.S2CAllCharId, s2CAllCharId);
            //foreach (var item in Server.instance.clientPools)
            //{
            //    var clientId = item.Key;
            //    SendData.instance.SendMessage(clientId, (int)messageType.S2CAllCharId, s2CAllCharId);
            //}
        }

        //发送所有位置信息
        public void SendAllLocation()
        {
            S2CAllLocation s2CAllLocation = new S2CAllLocation { allLocation = new Dictionary<int, InitLocation>() };

            s2CAllLocation.allLocation.Clear(); 
            foreach (var item in PlayerManager.instance.playerDic)
            {
                InitLocation initLocation = new InitLocation();
                var playerId = item.Key;
                var player = item.Value;
                initLocation.x = (int)player.locationX;
                initLocation.z = (int)player.locationZ;
                s2CAllLocation.allLocation.TryAdd(playerId, initLocation);
            }

            SendData.instance.Broadcast((int)messageType.S2CAllLocation, s2CAllLocation);
            //foreach (var item in Server.instance.clientPools)
            //{
            //    var clientId = item.Key;
            //    SendData.instance.SendMessage(clientId, (int)messageType.S2CAllLocation, s2CAllLocation);
            //}
        }

        //发送攻击消息
        public void SendAttack(S2CAttack s2CAttack)
        {
            SendData.instance.Broadcast((int)messageType.S2CAttack, s2CAttack);
            //foreach (var item in Server.instance.clientPools)
            //{
            //    var clientId = item.Key;
            //    SendData.instance.SendMessage(clientId, (int)messageType.S2CAttack, s2CAttack);
            //}
        }

        //发送血量变化
        public void SendHPChange(S2CHPChange s2CHPChange)
        {
            SendData.instance.Broadcast((int)messageType.S2CHPChange, s2CHPChange);
            //foreach (var item in Server.instance.clientPools)
            //{
            //    var clientId = item.Key;
            //    SendData.instance.SendMessage(clientId, (int)messageType.S2CHPChange, s2CHPChange);
            //}
        }

        //发送角色死亡
        public void SendCharDie(S2CDie s2CDie)
        {
            SendData.instance.Broadcast((int)messageType.S2CDie, s2CDie);
            //foreach (var item in Server.instance.clientPools)
            //{
            //    var clientId = item.Key;
            //    SendData.instance.SendMessage(clientId, (int)messageType.S2CDie, s2CDie);
            //}
        }

        //游戏开始
        public void GameStart()
        {
            //Timer timer = new Timer(new TimerCallback(GameStartTrigger), null, ReadConfig.instance.gameStartDelay * 1000, Timeout.Infinite);
            GameStartTrigger(null);
        }

        public void GameStartTrigger(object state)
        {
            EventManager.instance.AddEvent(()=>
            {
                S2CGameStart s2CGameStart = new S2CGameStart();
                s2CGameStart.placeholder = 0;
                SendData.instance.Broadcast((int)messageType.S2CGameStart, s2CGameStart);
                ServerUpdate.isSendLocation = true;
                ConsoleLog.instance.Info("游戏开始");
            });
        }

        //发送格子血量变化
        public void SendCellChange(int mapId, int x, int z, int nowHp)
        {
            S2CCellChange s2CCellChange = new S2CCellChange();
            s2CCellChange.mapId = 0;
            s2CCellChange.x = x;
            s2CCellChange.z = z;
            s2CCellChange.value = nowHp;

            SendData.instance.Broadcast((int)messageType.S2CCellChange, s2CCellChange);
        }

        //发送获胜者
        public void SendWinner(int playerId)
        {
            S2CSendWinner s2CSendWinner = new S2CSendWinner();
            s2CSendWinner.playerId = playerId;

            SendData.instance.Broadcast((int)messageType.S2CSendWinner, s2CSendWinner);
        }

        //发送地图变更
        public void SendMapChange(int x, int z, int value)
        {
            S2CCellChange s2CCellChange = new S2CCellChange();
            s2CCellChange.mapId = 0;
            s2CCellChange.x = x;
            s2CCellChange.z = z;
            s2CCellChange.value = value;

            SendData.instance.Broadcast((int)messageType.S2CCellChange, s2CCellChange);
        }

        //发送获得物品
        public void SendGetItem(int playerId, int itemId, int count)
        {
            S2CGetItem s2CGetItem = new S2CGetItem();
            s2CGetItem.playerId = playerId;
            s2CGetItem.itemId = itemId;
            s2CGetItem.count = count;

            SendData.instance.Broadcast((int)messageType.S2CGetItem, s2CGetItem);
        }

        //客户端丢弃物品
        public void DeleteItem(int clientId, C2SDeleteItem c2SDeleteItem)
        {
            Player player = PlayerManager.instance.GetPlayer(Server.instance.GetPlayerId(clientId));
            if(ItemController.instance.IsHaveItem(player, c2SDeleteItem.itemId))
            {
                GameMap gameMap = GameMapManager.instance.GetGameMap(0);
                var ret = MapController.instance.GetEmptyCell(player, gameMap, c2SDeleteItem.itemId);
                SendMapChange(ret[0], ret[1], c2SDeleteItem.itemId);
                ItemController.instance.ChangeItemCount(player, c2SDeleteItem.itemId, -1);
                SendDeleteItem(ret[0], ret[1], player.playerId, c2SDeleteItem.itemId, 1);
                ConsoleLog.instance.Info(string.Format("Player {0} 丢弃道具 {1}, 位置 {2},{3}", player.playerId, c2SDeleteItem.itemId, ret[0], ret[1]));
            }
            else
            {
                ConsoleLog.instance.Info(string.Format("Player {0} 没有道具 {1}", player.playerId, c2SDeleteItem.itemId));
            }
        }

        public void SendDeleteItem(int x, int z, int playerId, int itemId, int count)
        {
            S2CDeleteItem s2CDeleteItem = new S2CDeleteItem();
            s2CDeleteItem.x = x;
            s2CDeleteItem.z = z;
            s2CDeleteItem.playerId = playerId;
            s2CDeleteItem.itemId = itemId;
            s2CDeleteItem.count = count;
            SendData.instance.Broadcast((int)messageType.S2CDeleteItem, s2CDeleteItem);
        }

        public void UseItem(int clientId, C2SUseItem c2SUseItem)
        {
            Player player = PlayerManager.instance.GetPlayer(Server.instance.GetPlayerId(clientId));
            ItemController.instance.UseItem(player, c2SUseItem.itemId);
            ConsoleLog.instance.Info(string.Format("Player {0} 使用道具 {1}", player.playerId, c2SUseItem.itemId));
            SendUseItem(player.playerId, c2SUseItem.itemId);
        }

        public void SendUseItem(int playerId, int itemId)
        {
            S2CUseItem s2CUseItem = new S2CUseItem();
            s2CUseItem.playerId = playerId;
            s2CUseItem.itemId = itemId;
            SendData.instance.Broadcast((int)messageType.S2CUseItem, s2CUseItem);
        }

        //发送服务器人数
        public void SendPlayerCount(int playerCount)
        {
            S2CPlayerCount s2CPlayerCount = new S2CPlayerCount();
            s2CPlayerCount.playerCount = playerCount;
            SendData.instance.Broadcast((int)messageType.S2CPlayerCount, s2CPlayerCount);

            ConsoleLog.instance.Info(string.Format("服务器人数:{0}", playerCount));
        }
    }
}
