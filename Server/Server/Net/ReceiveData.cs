using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace GameServer
{
    public class ReceiveData
    {
        public static readonly ReceiveData instance = new ReceiveData();

        //开始接收
        public void BeginReceive(Client client)
        {
            try
            {
                client.socket.BeginReceive(client.buffer, 0, client.buffer.Length, SocketFlags.None,
                    EndReceive, client);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        //接收完回调
        public void EndReceive(IAsyncResult result)
        {
            Client client = result.AsyncState as Client;
            try
            {
                //获取消息的长度
                int len = client.socket.EndReceive(result);
                if (len > 0)
                {
                    byte[] data = new byte[len];
                    Buffer.BlockCopy(client.buffer, 0, data, 0, len);
                    
                    lock (Server.instance.receiveCache)
                    {
                        Server.instance.receiveCache.AddRange(data);
                    }
                    
                    BeginReceive(client);
                }
            }
            catch (Exception ex)
            {
                ConsoleLog.instance.Info(string.Format("客户端掉线: clientId {0}", client.clientId));
                DeleteClient(client);
            }
        }

        public void DeleteClient(Client client)
        {
            lock (PlayerManager.instance)
            {
                int playerId = Server.instance.clientId2PlayerId[client.clientId];
                PlayerManager.instance.playerDic.Remove(playerId);
                PlayerManager.instance.DeletePlayer(playerId);
            }
            lock (Server.instance)
            {
                Server.instance.clientId2PlayerId.Remove(client.clientId);
                Server.instance.clientPools.Remove(client.clientId);
            }
            client.socket.Close();
            client = null;
        }

        //读取接收的消息
        public void ReceiveMessage()
        {
            Message msg;
            lock (Server.instance.receiveCache)
            {
                msg = NetCode.Instance.Decode(ref Server.instance.receiveCache);
            }

            if (msg == null)
            {
                Thread.Sleep(2);
                return;
            }

            MessageController.instance.ReceiveMsgControl(msg);
        }

        public void Start()
        {
            while (true)
            {
                ReceiveMessage();
            }
        }
    }
}
