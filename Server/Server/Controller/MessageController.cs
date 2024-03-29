﻿using System;
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
            if (Server.instance.isGaming == false)
            {
                return;
            }
            switch (msg.messageType)
            {
                case (int)messageType.C2SMove:
                    C2SMove c2SMove = SerializeFunc.instance.DeSerialize<C2SMove>(msg.msg);
                    int playerId = Server.instance.GetPlayerId(msg.clientId);
                    lock (PlayerManager.instance.playerMove)
                    {
                        if (!PlayerManager.instance.playerMove.ContainsKey(playerId))
                        {
                            PlayerManager.instance.playerMove.Add(playerId, c2SMove);
                        }
                        else
                        {
                            PlayerManager.instance.playerMove[playerId] = c2SMove;
                        }
                    }
                    
                    break;
                case (int)messageType.C2SAttack:
                    C2SAttack c2SAttack = SerializeFunc.instance.DeSerialize<C2SAttack>(msg.msg);
                    EventManager.instance.AddEvent(() =>
                    {
                        GameProcess.instance.ClientAttack(msg.clientId, c2SAttack);
                    });
                    break;
                case (int)messageType.C2SChooseChar:
                    C2SChooseChar c2SChooseChar = SerializeFunc.instance.DeSerialize<C2SChooseChar>(msg.msg);
                    EventManager.instance.AddEvent(() =>
                    {
                        GameProcess.instance.ChooseChar(msg.clientId, c2SChooseChar);
                    });
                    break;
                case (int)messageType.C2SChooseLocation:
                    C2SChooseLocation c2SChooseLocation = SerializeFunc.instance.DeSerialize<C2SChooseLocation>(msg.msg);
                    EventManager.instance.AddEvent(() =>
                    {
                        GameProcess.instance.ChooseLocation(msg.clientId, c2SChooseLocation);
                    });
                    break;
                case (int)messageType.C2SDeleteItem:
                    C2SDeleteItem c2SDeleteItem = SerializeFunc.instance.DeSerialize<C2SDeleteItem>(msg.msg);
                    EventManager.instance.AddEvent(() =>
                    {
                        GameProcess.instance.DeleteItem(msg.clientId, c2SDeleteItem);
                    });
                    break; 
                case (int)messageType.C2SUseItem:
                    C2SUseItem c2SUseItem = SerializeFunc.instance.DeSerialize<C2SUseItem>(msg.msg);
                    EventManager.instance.AddEvent(() =>
                    {
                        GameProcess.instance.UseItem(msg.clientId, c2SUseItem);
                    });
                    break; 
                case (int)messageType.C2SChangeWeapon:
                    C2SChangeWeapon c2SChangeWeapon = SerializeFunc.instance.DeSerialize<C2SChangeWeapon>(msg.msg);
                    EventManager.instance.AddEvent(() =>
                    {
                        GameProcess.instance.ClientChangeWeapon(msg.clientId, c2SChangeWeapon);
                    });
                    break;
                case (int)messageType.C2SDeleteChicken:
                    C2SDeleteChicken c2SDeleteChicken = SerializeFunc.instance.DeSerialize<C2SDeleteChicken>(msg.msg);
                    EventManager.instance.AddEvent(() =>
                    {
                        GameProcess.instance.ClientDeleteChicken(msg.clientId, c2SDeleteChicken);
                    });
                    break;
                case (int)messageType.C2SAction:
                    C2SAction c2SAction = SerializeFunc.instance.DeSerialize<C2SAction>(msg.msg);
                    EventManager.instance.AddEvent(() =>
                    {
                        GameProcess.instance.ClientAction(msg.clientId, c2SAction);
                    });
                    break;

                default:
                    return;
            }
        }
    }
}
