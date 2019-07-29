﻿using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace GameServer
{
    public class SendData
    {
        public static readonly SendData instance = new SendData();

        //开启发送
        public void ServerSendStart()
        {
            while (true)
            {
                if(Server.instance.messageWaited.Count > 0)
                {
                    Send();
                }
            }
        }

        //发送数据
        public void Send()
        {
            //取出消息
            Message msg;
            lock (Server.instance.messageWaited)
            {
                msg = Server.instance.messageWaited.Dequeue();
                if(msg == null)
                {
                    return;
                }
            }

            //取出客户端实列
            Client client;
            if(!Server.instance.clientPools.TryGetValue(msg.clientId, out client))
            {
                return;
            }

            //客户端是否连接
            if (!client.socket.Connected)
            {
                return;
            }

            byte[] data = NetCode.Instance.Encode(msg.messageType, msg.msg);
            int count = data.Length / Client.size;
            int len = Client.size;
            for (int i = 0; i < count + 1; i++)
            {
                if (i == count)
                {
                    len = data.Length - i * Client.size;
                }
                client.socket.Send(data, i * Client.size, len, SocketFlags.None);
            }
        }

        //放入发送队列
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
    }
}
