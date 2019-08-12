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
            switch (msg.messageType)
            {
                case (int)messageType.C2SMove:
                    C2SMove c2SMove = SerializeFunc.instance.DeSerialize<C2SMove>(msg.msg);
                    EventManager.instance.AddEvent(() =>
                    {
                        int playerId = Server.instance.GetPlayerId(msg.clientId);
                        GameProcess.instance.ClientMove(playerId, c2SMove);
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
                        GameProcess.instance.ChooseChar(msg.clientId, c2SChooseChar);
                    });
                    break;
                case (int)messageType.C2SChooseLocation:
                    C2SChooseLocation c2SChooseLocation = SerializeFunc.instance.DeSerialize<C2SChooseLocation>(msg.msg);
                    EventManager.instance.AddEvent(() =>
                    {
                        Player player = PlayerManager.instance.GetPlayer(Server.instance.GetPlayerId(msg.clientId));
                        PlayerController.instance.SetLocation(player, c2SChooseLocation.x, c2SChooseLocation.z);
                        GameMap gameMap = GameMapManager.instance.GetGameMap(0);
                        MapController.instance.SetMapValue(gameMap, c2SChooseLocation.x, c2SChooseLocation.z, player.playerId);
                        GameProcess.instance.SendMapChange(c2SChooseLocation.x, c2SChooseLocation.z, player.playerId);
                        PlayerManager.instance.chooseLocationCount++;
                        if (PlayerManager.instance.chooseLocationCount >= ReadJson.instance.charCountToStart)
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
