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
            player.x = model.x;
            player.z = model.z;
            int cellXBefore = CDT2Cell.instance.CDT2X(player.locationX);
            int cellZBefore = CDT2Cell.instance.CDT2Z(player.locationZ);
            int cellX = CDT2Cell.instance.CDT2X(player.locationX + model.x);
            int cellZ = CDT2Cell.instance.CDT2Z(player.locationZ + model.z);
            if (MoveCal.instance.IsCanMove(cellX, cellZ))
            {
                if (cellXBefore != cellX || cellZBefore != cellZ)
                {
                    if (gameMap.gameMap[cellXBefore, cellZBefore] >= 3001 && gameMap.gameMap[cellXBefore, cellZBefore] <= 4000)
                    {
                    }
                    else if (gameMap.gameMap[cellXBefore, cellZBefore] >= 1001 && gameMap.gameMap[cellXBefore, cellZBefore] <= 2000)
                    {
                    }
                    else
                    {
                        MapController.instance.SetMapValue(gameMap, cellXBefore, cellZBefore, 0);
                        SendMapChange(cellXBefore, cellZBefore, 0);
                    }
                }

                if (gameMap.gameMap[cellX, cellZ] >= 2001 && gameMap.gameMap[cellX, cellZ] <= 3000)
                {
                    ItemController.instance.ChangeItemCount(player, gameMap.gameMap[cellX, cellZ], 1);
                }

                player.locationX += model.x;
                player.locationZ += model.z;
                MapController.instance.SetMapValue(gameMap, cellX, cellZ, player.playerId);
                SendMapChange(cellX, cellZ, player.playerId);
            }
        }

        //客户端攻击
        public void ClientAttack(C2SAttack data)
        {
            var weaponId = data.weaponId;
            var x = CDT2Cell.instance.CDT2X(data.locationX);
            var z = CDT2Cell.instance.CDT2Z(data.locationZ);
            GameMap gameMap = GameMapManager.instance.GetGameMap(0);
            MapController.instance.SetMapValue(gameMap, x, z, weaponId);

            SendMapChange(x, z, weaponId);

            Bomb bomb = BombManager.instance.CreateBomb(weaponId, x, z);

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
            if (PlayerManager.instance.playerDic.Keys.Count >= ReadJson.instance.charCountToStart)
            {
                SendAllCharId();
            }
        }

        //客户端位置
        public void ChooseLocation(int clientId, C2SChooseLocation c2SChooseLocation)
        {
            Player player = PlayerManager.instance.GetPlayer(Server.instance.GetPlayerId(clientId));
            PlayerController.instance.SetLocation(player, c2SChooseLocation.x, c2SChooseLocation.z);
            GameMap gameMap = GameMapManager.instance.GetGameMap(0);
            MapController.instance.SetMapValue(gameMap, c2SChooseLocation.x, c2SChooseLocation.z, player.playerId);
            GameProcess.instance.SendMapChange(c2SChooseLocation.x, c2SChooseLocation.z, player.playerId);
            PlayerManager.instance.chooseLocationCount++;
            if (PlayerManager.instance.chooseLocationCount >= ReadJson.instance.charCountToStart)
            {
                GameProcess.instance.SendAllLocation();
                GameProcess.instance.GameStart();
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
            Timer timer = new Timer(new TimerCallback(GameStartTrigger), null, ReadJson.instance.gameStartDelay * 1000, Timeout.Infinite);
        }

        public void GameStartTrigger(object state)
        {
            EventManager.instance.AddEvent(()=>
            {
                S2CGameStart s2CGameStart = new S2CGameStart();
                s2CGameStart.placeholder = 0;
                SendData.instance.Broadcast((int)messageType.S2CGameStart, s2CGameStart);
                ServerUpdate.isSendLocation = true;
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
    }
}
