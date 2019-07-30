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

        AllCharLocation allCharLocation = new AllCharLocation() { allCharLocation = new Dictionary<int, Location>() };

        //加入新玩家
        public void JoinNewPlayer(int charId)
        {
            NewCharId data = new NewCharId();
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
            CharId mCharId = new CharId();
            mCharId.clientId = clientId;
            mCharId.charId = charId;
            SendData.instance.SendMessage(clientId, (int)messageType.S2CSendCharId, mCharId);
        }
        
        //客户端移动
        public void ClientMove(int charId, Move model)
        {
            Character character;
            CharacterManager.instance.charDic.TryGetValue(charId, out character);
            if(character == null)
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
            GamMapManager.instance.mapDic.TryGetValue(0, out gameMap);
            gameMap.gameMap[(int)x, (int)z] = weaponId;
        }
    }
}
