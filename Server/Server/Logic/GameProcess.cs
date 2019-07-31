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

        //计时器
        Timer myTimer;

        //加入新玩家
        public void JoinNewPlayer(int charId)
        {
            S2CJoinNewPlayerModel data = new S2CJoinNewPlayerModel();
            data.charId = charId;
            if (Server.instance.clientPools.Count == 0)
            {
                return;
            }
            foreach (var item in Server.instance.clientPools)
            {
                Client client = item.Value;
                if (client.socket != null)
                {
                    SendData.instance.SendMessage(client.clientId, (int)messageType.S2CJoinNewPlayer, data);
                }
            }
        }

        //发送客户端自己的charId
        public void SendCharId(int clientId, int charId)
        {
            S2CSendCharIdModel mCharId = new S2CSendCharIdModel();
            mCharId.clientId = clientId;
            mCharId.charId = charId;
            SendData.instance.SendMessage(clientId, (int)messageType.S2CSendCharId, mCharId);
        }

        //客户端移动
        public void ClientMove(int charId, C2SMoveModel model)
        {
            Character character;
            CharacterManager.instance.charDic.TryGetValue(charId, out character);
            if (character == null)
            {
                return;
            }
            character.x = model.x;
            character.z = model.z;
            character.locationX += model.x;
            character.locationZ += model.z;
        }

        public void ClientAttack(C2SAttackModel data)
        {
            var weaponId = data.weaponId;
            var x = data.locationX;
            var z = data.locationZ;
            GameMap gameMap;
            GameMapManager.instance.mapDic.TryGetValue(0, out gameMap);
            gameMap.gameMap[(int)x, (int)z] = weaponId;

            Bomb bomb = new Bomb();
            bomb.weaponId = weaponId; bomb.x = (int)x;bomb.z = (int)z;

            ConsoleLog.instance.Info(string.Format("角色攻击,武器Id: {0},炸弹位置: {1} {2}", weaponId, x, z));

            myTimer = new Timer(new TimerCallback(BombTrigger), bomb, 3 * 1000, Timeout.Infinite);
        }

        public void BombTrigger(object state)
        {
            Bomb bomb = (Bomb)state;
            GameMap gameMap;
            GameMapManager.instance.mapDic.TryGetValue(0, out gameMap);
            gameMap.gameMap[bomb.x, bomb.z] = 0;

            ConsoleLog.instance.Info(string.Format("炸弹爆炸,武器Id: {0},炸弹位置: {1} {2}", bomb.weaponId, bomb.x, bomb.z));
        }
    }
}
