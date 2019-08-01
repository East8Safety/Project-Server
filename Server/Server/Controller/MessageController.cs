using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer
{
    public class MessageController
    {
        public static readonly MessageController instance = new MessageController();

        public void ReceiveMsgControl(Message msg)
        {
            switch (msg.messageType)
            {
                case (int)messageType.C2SMove:
                    C2SMoveModel move = SerializeFunc.instance.DeSerialize<C2SMoveModel>(msg.msg);
                    EventManager.instance.AddEvent(() =>
                    {
                        int charId;
                        Server.instance.clientId2CharId.TryGetValue(msg.clientId, out charId);
                        GameProcess.instance.ClientMove(charId, move);
                    });
                    break;

                case (int)messageType.C2SAttack:
                    C2SAttackModel data = SerializeFunc.instance.DeSerialize<C2SAttackModel>(msg.msg);
                    EventManager.instance.AddEvent(() =>
                    {
                        GameProcess.instance.ClientAttack(data);
                    });
                    break;

                default:
                    return;
            }
        }
    }
}
