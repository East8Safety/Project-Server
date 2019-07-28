using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace GameServer
{
    public class GameProcess
    {
        public static readonly GameProcess instance = new GameProcess();

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
            AllCharLocation allCharLocation = new AllCharLocation();
            allCharLocation.allCharLocation = new Dictionary<int, Location>();
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
                location.times = character.times;
                location.locationX = character.locationX;
                location.locationZ = character.locationZ;
                allCharLocation.allCharLocation.Add(character.charId, location);
            }

            if(Server.instance.clientPools == null)
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

        public void ClientMove(int charId, Move model)
        {
            Character character;
            CharacterManager.instance.charDic.TryGetValue(charId, out character);
            character.x = model.x;
            character.z = model.z;
            character.times = model.times;
            character.locationX += model.x;
            character.locationZ += model.z;
            Console.WriteLine(string.Format("Deal Message: {0} {1}:{2}", model.times, DateTime.Now.ToString(), DateTime.Now.Millisecond.ToString()));
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
                    Console.WriteLine(string.Format("Receive Message: {0} {1}:{2}", move.times, DateTime.Now.ToString(), DateTime.Now.Millisecond.ToString()));
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
