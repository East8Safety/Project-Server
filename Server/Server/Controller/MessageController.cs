using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer
{
    public class MessageController
    {
        public static readonly MessageController instance = new MessageController();

        //消息分发
        public void ReceiveMsgControl(Message msg)
        {
            switch (msg.messageType)
            {
                case (int)messageType.C2SMove:
                    C2SMove c2SMove = SerializeFunc.instance.DeSerialize<C2SMove>(msg.msg);
                    EventManager.instance.AddEvent(() =>
                    {
                        int charId = Server.instance.GetCharId(msg.clientId);
                        GameProcess.instance.ClientMove(charId, c2SMove);
                    });
                    break;
                case (int)messageType.C2SAttack:
                    C2SAttack c2SAttack = SerializeFunc.instance.DeSerialize<C2SAttack>(msg.msg);
                    EventManager.instance.AddEvent(() =>
                    {
                        GameProcess.instance.ClientAttack(c2SAttack);
                    });
                    break;
                case (int)messageType.C2SChooseChar:
                    C2SChooseChar c2SChooseChar = SerializeFunc.instance.DeSerialize<C2SChooseChar>(msg.msg);
                    EventManager.instance.AddEvent(() =>
                    {
                        CharacterManager.instance.CreateCharacter(msg.clientId, c2SChooseChar.charType);
                        int charId = Server.instance.GetCharId(msg.clientId);
                        GameProcess.instance.SendCharId(msg.clientId, charId);
                        if (CharacterManager.instance.charDic.Keys.Count >= ReadJson.instance.charCountToStart)
                        {
                            GameProcess.instance.SendAllCharId();
                        }
                    });
                    break;
                case (int)messageType.C2SChooseLocation:
                    C2SChooseLocation c2SChooseLocation = SerializeFunc.instance.DeSerialize<C2SChooseLocation>(msg.msg);
                    EventManager.instance.AddEvent(() =>
                    {
                        Character character = CharacterManager.instance.GetCharacter(Server.instance.GetCharId(msg.clientId));
                        CharacterController.instance.SetLocation(character, c2SChooseLocation.x, c2SChooseLocation.z);
                        CharacterManager.instance.chooseLocationCount++;
                        if (CharacterManager.instance.chooseLocationCount >= ReadJson.instance.charCountToStart)
                        {
                            GameProcess.instance.SendAllLocation();
                            GameProcess.instance.GameStart();
                        }
                    });
                    break;

                default:
                    return;
            }
        }
    }
}
