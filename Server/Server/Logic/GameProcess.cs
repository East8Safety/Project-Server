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

        public void CreateCharacter(int clientId)
        {
            var character = CharacterController.instance.Create();
            character.clientId = clientId;
            Server.instance.clientId2CharId.TryAdd(character.clientId, character.charId);
            CharacterController.instance.Init(character, 0, 0);
            CharacterManager.instance.AddCharacter(character);
        }
        
        public void UpdateMove()
        {
            //allCharLocation.allCharLocation.Clear();
            //if (CharacterManager.instance.charDic.Count == 0)
            //{
            //    return;
            //}
            //foreach (var item in CharacterManager.instance.charDic)
            //{
            //    Location location = new Location();
            //    var character = item.Value;
            //    location.x = character.x;
            //    location.z = character.z;
            //    location.locationX = character.locationX;
            //    location.locationZ = character.locationZ;
            //    allCharLocation.allCharLocation.Add(character.charId, location);
            //}
            //if (Server.instance.clientPools == null)
            //{
            //    return;
            //}
            //foreach (var item in Server.instance.clientPools)
            //{
            //    Client client = item.Value;
            //    if (client.socket != null)
            //    {
            //        GameProcess.instance.SendMessage(client.clientId, (int)messageType.S2CMove, allCharLocation);
            //    }
            //}
        }

        public void UpdateCharacters()
        {
            AllCharIds allCharIds = new AllCharIds();
            allCharIds.charIds = new List<int>();
            foreach (var item in CharacterManager.instance.charDic)
            {
                var character = item.Value;
                allCharIds.charIds.Add(character.charId);
            }

            foreach (var item in Server.instance.clientPools)
            {
                Client client = item.Value;
                if (client.socket != null)
                {
                    GameProcess.instance.SendMessage(client.clientId, (int)messageType.S2CUpdateChar, allCharIds);
                }
            }
        }

        //发送客户端自己的charId
        public void SendCharId(int clientId, int charId)
        {
            CharId mCharId = new CharId();
            mCharId.charId = charId;
            GameProcess.instance.SendMessage(clientId, (int)messageType.S2CSendCharId, mCharId);
        }
        int a = 1;
        public void ClientMove(int charId, Move model)
        {
            Character character;
            CharacterManager.instance.charDic.TryGetValue(charId, out character);
            character.x = model.x;
            character.z = model.z;
            character.locationX += model.x;
            character.locationZ += model.z;
            Console.WriteLine(System.DateTime.Now.Millisecond +"  adasdas  "+a);


            allCharLocation.allCharLocation.Clear();
            if (CharacterManager.instance.charDic.Count == 0)
            {
                return;
            }
            foreach (var item in CharacterManager.instance.charDic)
            {
                Location location = new Location();
                var character2 = item.Value;
                location.x = character2.x;
                location.z = character2.z;
                location.locationX = character2.locationX;
                location.locationZ = character2.locationZ;
                allCharLocation.allCharLocation.Add(character2.charId, location);
            }
            if (Server.instance.clientPools == null)
            {
                return;
            }
            foreach (var item in Server.instance.clientPools)
            {
                Client client = item.Value;
                if (client.socket != null)
                {
                    GameProcess.instance.SendMessage(client.clientId, (int)messageType.S2CMove, allCharLocation);
                }
            }
            Console.WriteLine(System.DateTime.Now.Millisecond + "  adasdas  " + a);
            a++;
        }

        public void SendMessage<T>(int clientId, int messageType, T model)
        {
            Message msg = new Message();
            msg.clientId = clientId;
            msg.messageType = messageType;
            msg.msg = SerializeFunc.instance.Serialize(model);
            lock (Server.instance.messageWaited)
            {
                Server.instance.messageWaited.Enqueue(msg);
            }
            SendData.instance.ServerSendStart();
        }

        public void ReceiveMessage()
        {
            Message msg;
            lock (Server.instance.messageReceived)
            {
                if (!Server.instance.messageReceived.TryDequeue(out msg))
                {
                    return;
                }
            }

            switch (msg.messageType)
            {
                case (int)messageType.C2SMove:
                    Move move = SerializeFunc.instance.DeSerialize<Move>(msg.msg);
                    EventManager.instance.AddEvent(() =>
                    {
                        int charId;
                        Server.instance.clientId2CharId.TryGetValue(msg.clientId, out charId);
                        GameProcess.instance.ClientMove(charId, move);
                    });

                    return;
                default:
                    return;
            }
        }
    }
}
