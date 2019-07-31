using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace GameServer
{
    class ServerUpdate
    {
        public static readonly ServerUpdate instance = new ServerUpdate();

        S2CMoveModel allCharLocation = new S2CMoveModel() { allCharLocation = new Dictionary<int, Location>()};

        //服务器update
        public void Update()
        {
            while (true)
            {
                UpdateMove();
                Thread.Sleep(10);
            }
        }

        //更新位置信息
        public void UpdateMove()
        {
            allCharLocation.allCharLocation.Clear();
            if (CharacterManager.instance.charDic.Count == 0)
            {
                return;
            }
            foreach (var item in CharacterManager.instance.charDic)
            {
                Location location = new Location();
                var character = item.Value;
                location.x = character.x;
                location.z = character.z;
                location.locationX = character.locationX;
                location.locationZ = character.locationZ;
                allCharLocation.allCharLocation.TryAdd(character.charId, location);
            }
            if (Server.instance.clientPools.Count == 0)
            {
                return;
            }
            foreach (var item in Server.instance.clientPools)
            {
                Client client = item.Value;
                if (client.socket != null)
                {
                    SendData.instance.SendMessage(client.clientId, (int)messageType.S2CMove, allCharLocation);
                }
            }
        }
    }
}
