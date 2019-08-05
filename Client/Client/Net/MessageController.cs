using System;
using System.Collections.Generic;
using System.Text;

namespace GameClient
{
    public class MessageController
    {
        public static readonly MessageController instance = new MessageController();

        //消息分发
        public void ReceiveMsgControl(Message msg)
        {
            switch (msg.messageType)
            {
                case (int)messageType.S2CMove:
                    Console.WriteLine(string.Format("start {0}:{1}", DateTime.Now.ToString(), DateTime.Now.Millisecond.ToString()));
                    S2CMove allCharLocation = SerializeFunc.instance.DeSerialize<S2CMove>(msg.msg);
                    Console.WriteLine(string.Format("end {0}:{1}", DateTime.Now.ToString(), DateTime.Now.Millisecond.ToString()));
                    foreach (var item in allCharLocation.allCharLocation)
                    {
                        var location = item.Value;
                        Console.WriteLine(string.Format("Char:{0} location: {1} {2} {3}:{4}", item.Key, location.locationX, location.locationZ,
                            DateTime.Now.ToString(), DateTime.Now.Millisecond.ToString()));
                    }
                    break;
                case (int)messageType.S2CSendCharId:
                    S2CSendCharId data = SerializeFunc.instance.DeSerialize<S2CSendCharId>(msg.msg);
                    Program.canSend = true;
                    Console.WriteLine(string.Format("This client's charId: {0}", data.charId));
                    break;
                case (int)messageType.S2CSendClientId:
                    S2CSendClientId s2CSendClientId = SerializeFunc.instance.DeSerialize<S2CSendClientId>(msg.msg);
                    Client.instance.clientId = s2CSendClientId.clientId;
                    Program.canSend = true;
                    Console.WriteLine(string.Format("This client's clientId: {0}", s2CSendClientId.clientId));
                    break;
                case (int)messageType.S2CJoinNewPlayer:
                    S2CJoinNewPlayer newCharId = SerializeFunc.instance.DeSerialize<S2CJoinNewPlayer>(msg.msg);
                    Console.WriteLine(string.Format("New Char: {0}", newCharId.charId));
                    break;
                default:
                    break;
            }
        }
    }
}
