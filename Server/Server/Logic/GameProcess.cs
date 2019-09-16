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

        private Timer chooseLocationTimer;

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

            if (model.x >= 0)
            {
                if (model.z >= 0)
                {
                    if (model.x >= model.z)
                    {
                        player.toward = 2;
                    }
                    else
                    {
                        player.toward = 1;
                    }
                }
                else if (model.z < 0)
                {
                    if (model.x >= -model.z)
                    {
                        player.toward = 2;
                    }
                    else
                    {
                        player.toward = 3;
                    }
                }
            }
            else if (model.x < 0)
            {
                if (model.z >= 0)
                {
                    if (-model.x >= model.z)
                    {
                        player.toward = 4;
                    }
                    else
                    {
                        player.toward = 1;
                    }
                }
                else if (model.z < 0)
                {
                    if (-model.x >= -model.z)
                    {
                        player.toward = 4;
                    }
                    else
                    {
                        player.toward = 3;
                    }
                }
            }

            int cellX = CDT2Cell.instance.CDT2X(player.locationX + model.x);
            int cellZ = CDT2Cell.instance.CDT2Z(player.locationZ + model.z);
            if (MoveCal.instance.IsCanMove(player, cellX, cellZ))
            {
                if (gameMap.gameMap[cellX, cellZ] >= 2001 && gameMap.gameMap[cellX, cellZ] <= 3000)  // get item
                {
                    if (player.index2ItemId.Count < 6)
                    {
                        var itemId = gameMap.gameMap[cellX, cellZ];
                        ItemController.instance.AddItem(player, itemId, 1);
                        MapController.instance.SetMapValue(gameMap, cellX, cellZ, 0);
                        SyncItem(player);

                        ConsoleLog.instance.Info(string.Format("Player {0} 获得道具 {1}", player.playerId, itemId));
                    }
                }

                player.locationX += model.x;
                player.locationZ += model.z;
                player.x = cellX;
                player.z = cellZ;
            }
        }

        //客户端攻击
        public void ClientAttack(int clientId, C2SAttack data)
        {
            int playerId = Server.instance.GetPlayerId(clientId);
            var player = PlayerManager.instance.GetPlayer(playerId);
            var weaponId = data.weaponId;
            GameMap gameMap = GameMapManager.instance.GetGameMap(0);
            GroundMap groundMap = GameMapManager.instance.GetGroundMap(0);

            if (weaponId == -1)
            {
                switch (player.toward)
                {
                    case 1:
                        var x1 = player.x;
                        var z1 = player.z + 1;
                        if (z1 < 0 || z1 > 47)
                        {
                            return;
                        }
                        if (gameMap.gameMap[x1,z1] > 0 && gameMap.gameMap[x1, z1] <= 1000)
                        {
                            MapController.instance.Damage(groundMap, x1, z1, player.damageCommon1);
                        }
                        break;
                    case 2:
                        var x2 = player.x + 1;
                        var z2 = player.z;
                        if (x2 < 0 || x2 > 47)
                        {
                            return;
                        }
                        if (gameMap.gameMap[x2, z2] > 0 && gameMap.gameMap[x2, z2] <= 1000)
                        {
                            MapController.instance.Damage(groundMap, x2, z2, player.damageCommon1);
                        }
                        break;
                    case 3:
                        var x3 = player.x;
                        var z3 = player.z - 1;
                        if (z3 < 0 || z3 > 47)
                        {
                            return;
                        }
                        if (gameMap.gameMap[x3, z3] > 0 && gameMap.gameMap[x3, z3] <= 1000)
                        {
                            MapController.instance.Damage(groundMap, x3, z3, player.damageCommon1);
                        }
                        break;
                    case 4:
                        var x4 = player.x - 1;
                        var z4 = player.z;
                        if (x4 < 0 || x4 > 47)
                        {
                            return;
                        }
                        if (gameMap.gameMap[x4, z4] > 0 && gameMap.gameMap[x4, z4] <= 1000)
                        {
                            MapController.instance.Damage(groundMap, x4, z4, player.damageCommon1);
                        }
                        break;
                }
            }
            else if (weaponId == -2)
            {
                switch (player.toward)
                {
                    case 1:
                        var x1 = player.x;
                        var z1 = player.z + 1;
                        if (z1 < 0 || z1 > 47)
                        {
                            return;
                        }
                        if (gameMap.gameMap[x1, z1] > 0 && gameMap.gameMap[x1, z1] <= 1000)
                        {
                            MapController.instance.Damage(groundMap, x1, z1, player.damageCommon2);
                        }
                        break;
                    case 2:
                        var x2 = player.x + 1;
                        var z2 = player.z;
                        if (x2 < 0 || x2 > 47)
                        {
                            return;
                        }
                        if (gameMap.gameMap[x2, z2] > 0 && gameMap.gameMap[x2, z2] <= 1000)
                        {
                            MapController.instance.Damage(groundMap, x2, z2, player.damageCommon2);
                        }
                        break;
                    case 3:
                        var x3 = player.x;
                        var z3 = player.z - 1;
                        if (z3 < 0 || z3 > 47)
                        {
                            return;
                        }
                        if (gameMap.gameMap[x3, z3] > 0 && gameMap.gameMap[x3, z3] <= 1000)
                        {
                            MapController.instance.Damage(groundMap, x3, z3, player.damageCommon2);
                        }
                        break;
                    case 4:
                        var x4 = player.x - 1;
                        var z4 = player.z;
                        if (x4 < 0 || x4 > 47)
                        {
                            return;
                        }
                        if (gameMap.gameMap[x4, z4] > 0 && gameMap.gameMap[x4, z4] <= 1000)
                        {
                            MapController.instance.Damage(groundMap, x4, z4, player.damageCommon2);
                        }
                        break;
                }
            }
            else if (weaponId == -3)
            {
                if (player.bombCount <= 0)
                {
                    SendBombNone(playerId);
                    return;
                }

                player.bombCount--;


                var x = CDT2Cell.instance.CDT2X(data.locationX);
                var z = CDT2Cell.instance.CDT2Z(data.locationZ);

                Bomb bomb = BombManager.instance.CreateBomb(player, x, z);

                MapController.instance.SetMapValue(gameMap, x, z, bomb.id);

                ConsoleLog.instance.Info(string.Format("角色攻击,武器Id: {0},泡泡位置: {1} {2}", weaponId, x, z));

                bomb.timer = new Timer(new TimerCallback(BombController.instance.BombTrigger), bomb, 3 * 1000, Timeout.Infinite);

                S2CAttack s2CAttack = new S2CAttack();
                s2CAttack.weaponId = weaponId;
                s2CAttack.x = x;
                s2CAttack.z = z;
                SendAttack(s2CAttack);
            }
        }

        public void SendBombNone(int playerId)
        {
            S2CBombNone s2CBombNone = new S2CBombNone();
            s2CBombNone.playerId = playerId;
            SendData.instance.Broadcast((int)messageType.S2CBombNone, s2CBombNone);
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
                chooseLocationTimer = new Timer(new TimerCallback(GameStartTrigger), null, ReadConfig.instance.timeToChooseLocation * 1000, Timeout.Infinite);
                ConsoleLog.instance.Info(string.Format("所有人选人完毕"));
            }
        }

        //客户端位置
        public void ChooseLocation(int clientId, C2SChooseLocation c2SChooseLocation)
        {
            Player player = PlayerManager.instance.GetPlayer(Server.instance.GetPlayerId(clientId));
            PlayerController.instance.SetLocation(player, c2SChooseLocation.x, c2SChooseLocation.z);
            SendAllLocation();
            ConsoleLog.instance.Info(string.Format("Player {0} 选位置 {1},{2}", player.playerId, c2SChooseLocation.x, c2SChooseLocation.z));

            ReadConfig.instance.SetPlayerLocation(player);
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
                if (player.x == -1)
                {
                    continue;
                }
                initLocation.locatinX = player.locationX;
                initLocation.locatinZ = player.locationZ;
                initLocation.x = player.x;
                initLocation.z = player.z;
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
        public void SendHPChange(Player player)
        {
            S2CHPChange s2CHPChange = new S2CHPChange();
            s2CHPChange.playerId = player.playerId;
            s2CHPChange.nowHP = player.HP;
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

        public void GameStartTrigger(object state)
        {
            EventManager.instance.AddEvent(()=>
            {
                SetNoLocationPlayers();
                Server.instance.CreateMap();

                S2CGameStart s2CGameStart = new S2CGameStart();
                s2CGameStart.placeholder = 0;
                SendData.instance.Broadcast((int)messageType.S2CGameStart, s2CGameStart);
                ServerUpdate.isSendLocation = true;
                ConsoleLog.instance.Info("游戏开始");
            });
        }

        public void SetNoLocationPlayers()
        {
            foreach (var item in PlayerManager.instance.playerDic)
            {
                var player = item.Value;
                if (player.x == -1)
                {
                    while (true)
                    {
                        var x = GenerateItem.random.Next(0, 48);
                        var z = GenerateItem.random.Next(0, 48);
                        if (ReadConfig.instance.map1[x, z] > 0 && ReadConfig.instance.map1[x, z] <= 1000)
                        {
                            PlayerController.instance.SetLocation(player, x, z);
                            ReadConfig.instance.SetPlayerLocation(player);
                            break;
                        }
                    }
                }
            }

            SendAllLocation();
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

        public void SyncItem(Player player)
        {
            S2CSyncItem s2CSyncItem = new S2CSyncItem() { index2ItemId = new Dictionary<int, int>()};
            s2CSyncItem.playerId = player.playerId;
            s2CSyncItem.index2ItemId = player.index2ItemId;
            SendData.instance.Broadcast((int)messageType.S2CSyncItem, s2CSyncItem);
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
            if(ItemController.instance.IsHaveItem(player, c2SDeleteItem.index))
            {
                GameMap gameMap = GameMapManager.instance.GetGameMap(0);
                var ret = MapController.instance.GetEmptyCell(player, gameMap, c2SDeleteItem.itemId);
                SendMapChange(ret[0], ret[1], c2SDeleteItem.index);
                ItemController.instance.DeleteItem(player, c2SDeleteItem.index);
                SyncItem(player);
                //SendDeleteItem(ret[0], ret[1], player.playerId, c2SDeleteItem.index, 1);
                ConsoleLog.instance.Info(string.Format("Player {0} 丢弃道具 {1}, 位置 {2},{3}", player.playerId, c2SDeleteItem.index, ret[0], ret[1]));
            }
            else
            {
                ConsoleLog.instance.Info(string.Format("Player {0} 没有道具 {1}", player.playerId, c2SDeleteItem.index));
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
            ItemController.instance.UseItem(player, c2SUseItem.index, c2SUseItem.itemId);
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

        //同步状态
        public void SyncState(Player player)
        {
            S2CSyncState s2CSyncState = new S2CSyncState();
            s2CSyncState.playerId = player.playerId;
            s2CSyncState.hp = player.HP;
            s2CSyncState.speed = player.speed;
            s2CSyncState.shield = player.shield;
            s2CSyncState.bombCount = player.bombCount;
            SendData.instance.Broadcast((int)messageType.S2CSyncState, s2CSyncState);
        }
    }
}
