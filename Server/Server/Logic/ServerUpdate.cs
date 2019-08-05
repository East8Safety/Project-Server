﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace GameServer
{
    class ServerUpdate
    {
        public static readonly ServerUpdate instance = new ServerUpdate();

        S2CMove allCharLocation = new S2CMove() { allCharLocation = new Dictionary<int, Location>()};
        public static bool isSendLocation = false;

        //服务器update
        public void Update()
        {
            while (true)
            {
                if (isSendLocation == true)
                {
                    UpdateMove();
                }
                Thread.Sleep(30);
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

            GameProcess.instance.Broadcast((int)messageType.S2CMove, allCharLocation);
            //foreach (var item in Server.instance.clientPools)
            //{
            //    Client client = item.Value;
            //    if (client.socket != null)
            //    {
            //        SendData.instance.SendMessage(client.clientId, (int)messageType.S2CMove, allCharLocation);
            //    }
            //}
        }
    }
}
