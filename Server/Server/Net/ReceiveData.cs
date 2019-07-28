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

        public void ServerReceiveStart()
        {
            while (true)
            {
                if (Server.instance.clientPools == null)
                {
                    Thread.Sleep(100);
                    continue;
                }
                foreach (var client in Server.instance.clientPools)
                {
                    if (client.Value.socket == null || client.Value.isBeginReceive == true)
                    {
                        continue;
                    }
                    BeginReceive(client.Value);
                }
                Thread.Sleep(10);
            }
        }
        
        public void BeginReceive(Client client)
        {
            try
            {
                client.socket.BeginReceive(client.buffer, 0, client.buffer.Length, SocketFlags.None,
                    EndReceive, client);
                client.isBeginReceive = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void EndReceive(IAsyncResult result)
        {
            try
            {
                Console.WriteLine(System.DateTime.Now.Millisecond + "  接受到信息开始  ");
                Message msg;
                Client client = result.AsyncState as Client;
                //获取消息的长度
                int len = client.socket.EndReceive(result);
                if (len > 0)
                {
                    byte[] data = new byte[len];
                    Buffer.BlockCopy(client.buffer, 0, data, 0, len);

                    client.receiveCache.AddRange(data);
                    msg = NetCode.Instance.Decode(ref client.receiveCache);
                    
                    if(msg == null)
                    {
                        BeginReceive(client);
                        return;
                    }
                    msg.clientId = client.clientId;
                    lock (Server.instance.messageReceived)
                    {
                        Server.instance.messageReceived.Enqueue(msg);
                        Console.WriteLine("收到了消息"+msg.messageType);
                    }
                    Console.WriteLine(System.DateTime.Now.Millisecond + "  接受到信息结束  ");
                    //尾递归，再次监听客户端消息
                    BeginReceive(client);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
