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
                PlayerManager.instance.CreatePlayer(clientId);
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
            Player player = PlayerManager.instance.GetPlayer(playerId);
            if (player == null)
            {
                return;
            }
            player.x = model.x;
            player.z = model.z;
            int cellX = CDT2Cell.instance.CDT2X(player.locationX + model.x);
            int cellZ = CDT2Cell.instance.CDT2Z(player.locationZ + model.z);
            if (MoveCal.instance.IsCanMove(cellX, cellZ))
            {
                player.locationX += model.x;
                player.locationZ += model.z;
            }
        }

        //客户端攻击
        public void ClientAttack(C2SAttack data)
        {
            var weaponId = data.weaponId;
            var x = CDT2Cell.instance.CDT2X(data.locationX);
            var z = CDT2Cell.instance.CDT2Z(data.locationZ);
            GameMap gameMap = GameMapManager.instance.GetGameMap(0);
            gameMap.gameMap[x, z] = weaponId;

            Bomb bomb = BombController.instance.Create(weaponId, x, z);

            ConsoleLog.instance.Info(string.Format("角色攻击,武器Id: {0},炸弹位置: {1} {2}", weaponId, x, z));

            Timer myTimer = new Timer(new TimerCallback(BombController.instance.BombTrigger), bomb, 3 * 1000, Timeout.Infinite);

            S2CAttack s2CAttack = new S2CAttack();
            s2CAttack.weaponId = weaponId;
            s2CAttack.x = x;
            s2CAttack.z = z;
            SendAttack(s2CAttack);
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
            S2CAllLocation s2CAllLocation = new S2CAllLocation { allLocation = new Dictionary<int, Location>() };

            s2CAllLocation.allLocation.Clear(); 
            foreach (var item in PlayerManager.instance.playerDic)
            {
                Location location = new Location();
                var playerId = item.Key;
                var player = item.Value;
                location.locationX = player.locationX;
                location.locationZ = player.locationZ;
                s2CAllLocation.allLocation.TryAdd(playerId, location);
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
            s2CCellChange.nowHp = nowHp;

            SendData.instance.Broadcast((int)messageType.S2CCellChange, s2CCellChange);
        }

        //发送获胜者
        public void SendWinner(int playerId)
        {
            S2CSendWinner s2CSendWinner = new S2CSendWinner();
            s2CSendWinner.playerId = playerId;

            SendData.instance.Broadcast((int)messageType.S2CSendWinner, s2CSendWinner);
        }
    }
}
