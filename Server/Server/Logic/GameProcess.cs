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
        private Timer GameEndTimer;
        private Timer GameInitTimer;
        private Timer chickenGameTimer;

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
            SendData.instance.SendMessage(clientId, (int)messageType.S2CSendClientId, s2CSendClientId);
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
                    else if(player.index2ItemId.Count == 6)
                    {
                        var itemId = gameMap.gameMap[cellX, cellZ];
                        if (itemId >= 2007 && itemId <= 2015)
                        {
                            ItemController.instance.AddItem(player, itemId, 1);
                            MapController.instance.SetMapValue(gameMap, cellX, cellZ, 0);
                            SyncItem(player);

                            ConsoleLog.instance.Info(string.Format("Player {0} 获得道具 {1}", player.playerId, itemId));
                        }
                    }
                }
                else if(gameMap.gameMap[cellX, cellZ] == 3001 && gameMap.gameMap[player.x, player.z] != 3001)
                {
                    player.timer = new Timer(new TimerCallback(PlayerWin), player.playerId, ReadConfig.instance.portalTime * 1000, Timeout.Infinite);
                    SendBeginTimer(playerId, ReadConfig.instance.portalTime);
                }
                else if (gameMap.gameMap[cellX, cellZ] != 3001 && gameMap.gameMap[player.x, player.z] == 3001)
                {
                    if (player.timer != null)
                    {
                        player.timer.Change(Timeout.Infinite, Timeout.Infinite);
                        SendEndTimer(playerId);
                    }
                }
                else if (Server.instance.whichGame == 3)
                {
                    if (gameMap.gameMap[cellX, cellZ] == 3003 && gameMap.gameMap[player.x, player.z] != 3003)
                    {
                        if (player.isHaveChicken == false)
                        {
                            ItemController.instance.AddItem(player, 3003, 1);
                            MapController.instance.SetMapValue(gameMap, cellX, cellZ, 0);
                            SyncItem(player);

                            ConsoleLog.instance.Info(string.Format("Player {0} 捡起鸡 {1}", player.playerId, 3003));
                        }
                    }
                    else if (!(cellX == player.x && cellZ == player.z))
                    {
                        if (player.isHaveChicken == false && gameMap.gameMap[cellX, cellZ] == 3003 && gameMap.gameMap[player.x, player.z] == 3003)
                        {
                            ItemController.instance.AddItem(player, 3003, 1);
                            MapController.instance.SetMapValue(gameMap, cellX, cellZ, 0);
                            SyncItem(player);

                            ConsoleLog.instance.Info(string.Format("Player {0} 捡起鸡 {1}", player.playerId, 3003));
                        }
                    }
                    else if (gameMap.gameMap[cellX, cellZ] == 3002 && player.isHaveChicken == true)
                    {
                        PlayerFinalWin(player);
                    }
                }

                player.locationXB = player.locationX;
                player.locationZB = player.locationZ;
                player.locationX += model.x;
                player.locationZ += model.z;
                player.x = cellX;
                player.z = cellZ;
            }
        }

        public void PlayerWin(object state)
        {
            EventManager.instance.AddEvent(() =>
            {
                if (GameEndTimer == null)
                {
                    GameEndTimer = new Timer(new TimerCallback(GameOver), null, ReadConfig.instance.gameEndTime * 1000, Timeout.Infinite);
                }

                int playerId = (int)state;
                Player player = PlayerManager.instance.GetPlayer(playerId);
                SendInPortal(playerId);

                PlayerManager.instance.nextPlayers.TryAdd(playerId, player);
                ResetPlayer(player);
                
                if (PlayerManager.instance.nextPlayers.Count >= ReadConfig.instance.portalPlayerCount)
                {
                    GameOver();
                }
            });
        }

        public void PlayerFinalWin(Player player)
        {
            var playerId = player.playerId;

            SendFinalWin(playerId);
        }

        public void SendFinalWin(int playerId)
        {
            S2CFinalWin s2CFinalWin = new S2CFinalWin();
            s2CFinalWin.playerId = playerId;
            SendData.instance.Broadcast((int)messageType.S2CFinalWin, s2CFinalWin);
        }

        public void SendInPortal(int playerId)
        {
            S2CInPortal s2CInPortal = new S2CInPortal();
            s2CInPortal.playerId = playerId;
            SendData.instance.Broadcast((int)messageType.S2CInPortal, s2CInPortal);
        }

        public void GameOver(object state = null)
        {
            if (Server.instance.isGaming == false)
            {
                return;
            }

            Server.instance.isGaming = false;
            ServerUpdate.isSendLocation = false;

            if (GameEndTimer != null)
            {
                GameEndTimer.Change(Timeout.Infinite, Timeout.Infinite);
            }
            GameEndTimer = null;

            foreach (var item in BombManager.instance.bombDic)
            {
                item.Value.timer.Change(Timeout.Infinite, Timeout.Infinite);
            }
            BombManager.instance.bombDic.Clear();
            GameMapManager.instance.boxDic.Clear();
            PlayerManager.instance.playerPool.Clear();

            foreach (var item in PlayerManager.instance.playerDic)
            {
                item.Value.timer.Change(Timeout.Infinite, Timeout.Infinite);
                if (!PlayerManager.instance.nextPlayers.ContainsKey(item.Key))
                {
                    SendCharDie(item.Key);
                    PlayerManager.instance.DeletePlayer(item.Key);
                }
            }

            PlayerManager.instance.chooseCharCount = 0;
            PlayerManager.instance.chooseLocationCount = 0;
            PlayerManager.instance.playerMove.Clear();

            SendGameOver();

            if (Server.instance.whichGame == 1)
            {
                Server.instance.whichGame = 2;
                GameInitTimer = new Timer(new TimerCallback(GameInit), null, ReadConfig.instance.gameInitTime * 1000, Timeout.Infinite);
                return;
            }
            else if (Server.instance.whichGame == 2)
            {
                Server.instance.whichGame = 3;
                GameInitTimer = new Timer(new TimerCallback(GameInit), null, ReadConfig.instance.gameInitTime * 1000, Timeout.Infinite);
                return;
            }
        }

        public void SendGameOver()
        {
            S2CGameOver s2CGameOver = new S2CGameOver();
            s2CGameOver.overGame = Server.instance.whichGame;
            SendData.instance.Broadcast((int)messageType.S2CGameOver, s2CGameOver);
        }

        public void GameInit(object state)
        {
            SetNoLocationPlayers();

            if (Server.instance.whichGame == 2)
            {
                GenerateItem.Generate(ReadConfig.instance.map2, ReadConfig.instance.itemCount2, ReadConfig.map2Width, ReadConfig.map2Hight, 2);
                GameMapManager.instance.CreateMap(ReadConfig.map2Width, ReadConfig.map2Hight, ReadConfig.instance.map2, ReadConfig.instance.itemMap2, ReadConfig.instance.groundMap2);
            }
            else if (Server.instance.whichGame == 3)
            {
                GenerateItem.Generate(ReadConfig.instance.map3, ReadConfig.instance.itemCount3, ReadConfig.map3Width, ReadConfig.map3Hight, 3);
                GameMapManager.instance.CreateMap(ReadConfig.map3Width, ReadConfig.map3Hight, ReadConfig.instance.map3, ReadConfig.instance.itemMap3, ReadConfig.instance.groundMap3);
            }

            Server.instance.isGaming = true;
            ServerUpdate.isSendLocation = true;
            SendStartAgain(Server.instance.whichGame);

            if (Server.instance.whichGame == 3)
            {
                chickenGameTimer = new Timer(new TimerCallback(Generate), null, ReadConfig.instance.chickenGameTime * 1000, Timeout.Infinite);
            }
        }

        public void Generate(object state)
        {
            S2CChickenLoc s2CChickenLoc = new S2CChickenLoc() { chickenLocList = new List<ChickenLoc>()};

            GameMap gameMap = GameMapManager.instance.GetGameMap(0);
            GenerateItem.GenerateChicken(gameMap, 36, 36, ref s2CChickenLoc);
            SendChickenLoc(s2CChickenLoc);
        }

        public void SendChickenLoc(S2CChickenLoc s2CChickenLoc)
        {
            SendData.instance.Broadcast((int)messageType.S2CChickenLoc, s2CChickenLoc);
        }

        public void SendStartAgain(int witchGame)
        {
            S2CStart s2CStart = new S2CStart();
            s2CStart.witchGame = witchGame;
            SendData.instance.Broadcast((int)messageType.S2CStart, s2CStart);
        }

        public void ResetPlayer(Player player)
        {
            player.x = -1;
            player.z = -1;
            player.locationX = 0;
            player.locationZ = 0;
            player.mapValueBefore = -1;
            player.xBefore = -1;
            player.zBefore = -1;
            player.toward = 1;
            player.damageCommon1 = 99;
            player.damageCommon2 = 98;
            player.bombCount = 20;

            int playerCount = PlayerManager.instance.playerPool.Count;
            for (int i = 0; i < playerCount; i++)
            {
                var mPlayerId = PlayerManager.instance.playerPool.Dequeue();
                if (mPlayerId == player.playerId)
                {
                    break;
                }
                PlayerManager.instance.playerPool.Enqueue(mPlayerId);
            }

            if (PlayerManager.instance.playerPool.Count <= 0)
            {
                GameOver();
            }
        }

        //客户端攻击
        public void ClientAttack(int clientId, C2SAttack data)
        {
            int playerId = Server.instance.GetPlayerId(clientId);
            var player = PlayerManager.instance.GetPlayer(playerId);
            if (player == null) return;
            var weaponId = data.weaponId;
            GameMap gameMap = GameMapManager.instance.GetGameMap(0);
            GroundMap groundMap = GameMapManager.instance.GetGroundMap(0);

            S2CAttack s2CAttack = new S2CAttack();
            s2CAttack.playerId = playerId;
            SendAttack(s2CAttack);

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

                if (gameMap.gameMap[x, z] == 3001 || gameMap.gameMap[x, z] == 3002 || gameMap.gameMap[x, z] == 3003)
                {
                    return;
                }

                Bomb bomb = BombManager.instance.CreateBomb(player, x, z);

                MapController.instance.SetMapValue(gameMap, x, z, bomb.id);

                ConsoleLog.instance.Info(string.Format("角色攻击,武器Id: {0},泡泡位置: {1} {2}", weaponId, x, z));

                bomb.timer = new Timer(new TimerCallback(BombController.instance.BombTrigger), bomb, ReadConfig.instance.bombTime * 1000, Timeout.Infinite);
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
            GameProcess.instance.SyncState(player);
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
            return;
            Player player = PlayerManager.instance.GetPlayer(Server.instance.GetPlayerId(clientId));
            PlayerController.instance.SetLocation(player, c2SChooseLocation.x, c2SChooseLocation.z);
            SendAllLocation();
            ConsoleLog.instance.Info(string.Format("Player {0} 选位置 {1},{2}", player.playerId, c2SChooseLocation.x, c2SChooseLocation.z));

            ReadConfig.instance.SetPlayerLocation(player, ref ReadConfig.instance.map1);
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

                SendMapChange(player.x, player.z, 0);
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
        public void SendCharDie(int playerId)
        {
            S2CDie s2CDie = new S2CDie();
            s2CDie.playerId = playerId;
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

                foreach (var item in PlayerManager.instance.playerDic.Keys)
                {
                    ThreadManager.instance.PlayerThread(item);
                }
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
                        int x = 0; int z = 0;
                        if (Server.instance.whichGame == 1 || Server.instance.whichGame == 2)
                        {
                            x = GenerateItem.random.Next(0, 48);
                            z = GenerateItem.random.Next(0, 48);
                        }
                        else if(Server.instance.whichGame == 3)
                        {
                            x = GenerateItem.random.Next(0, 36);
                            z = GenerateItem.random.Next(0, 36);
                        }
                        
                        if (ReadConfig.instance.map1[x, z] > 0 && ReadConfig.instance.map1[x, z] <= 1000)
                        {
                            PlayerController.instance.SetLocation(player, x, z);
                            switch (Server.instance.whichGame)
                            {
                                case 1:
                                    ReadConfig.instance.SetPlayerLocation(player, ref ReadConfig.instance.map1);
                                    break;
                                case 2:
                                    ReadConfig.instance.SetPlayerLocation(player, ref ReadConfig.instance.map2);
                                    break;
                                case 3:
                                    ReadConfig.instance.SetPlayerLocation(player, ref ReadConfig.instance.map3);
                                    break;
                                default:
                                    break;
                            }
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

        public void ClientChangeWeapon(int clientId, C2SChangeWeapon c2SChangeWeapon)
        {
            SendChangeWeapon(c2SChangeWeapon.playerId, c2SChangeWeapon.weaponId);
        }

        public void SendChangeWeapon(int playerId, int weaponId)
        {
            S2CChangeWeapon s2CChangeWeapon = new S2CChangeWeapon();
            s2CChangeWeapon.playerId = playerId;
            s2CChangeWeapon.weaponId = weaponId;
            SendData.instance.Broadcast((int)messageType.S2CChangeWeapon, s2CChangeWeapon);
        }

        public void ClientDeleteChicken(int clientId, C2SDeleteChicken c2SDeleteChicken)
        {
            GameMap gameMap = GameMapManager.instance.GetGameMap(0);
            int playerId = c2SDeleteChicken.playerId;
            Player player = PlayerManager.instance.GetPlayer(playerId);
            var x = player.x;
            var z = player.z;
            player.isHaveChicken = false;
            player.debuff = 0;
            MapController.instance.SetMapValue(gameMap, x, z, 3003);
        }

        public void ClientAction(int clientId, C2SAction c2SAction)
        {
            int playerId = Server.instance.GetPlayerId(clientId);
            SendClientAction(playerId);
        }

        public void SendClientAction(int playerId)
        {
            S2CAction s2CAction = new S2CAction();
            s2CAction.playerId = playerId;
            SendData.instance.Broadcast((int)messageType.S2CAction, s2CAction);
        }

        public void SendBeginTimer(int playerId, int second)
        {
            S2CBeginTimer s2CBeginTimer = new S2CBeginTimer();
            s2CBeginTimer.playerId = playerId;
            s2CBeginTimer.second = second;
            SendData.instance.Broadcast((int)messageType.S2CBeginTimer, s2CBeginTimer);
        }

        public void SendEndTimer(int playerId)
        {
            S2CEndTimer s2CEndTimer = new S2CEndTimer();
            s2CEndTimer.playerId = playerId;
            SendData.instance.Broadcast((int)messageType.S2CEndTimer, s2CEndTimer);
        }
    }
}
