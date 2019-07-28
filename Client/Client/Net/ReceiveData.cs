using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace GameClient
{
    public class ReceiveData
    {
        public static readonly ReceiveData instance = new ReceiveData();

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

        public void EndReceive(IAsyncResult result)
        {
            try
            {
                Client client = result.AsyncState as Client;
                //获取消息的长度
                int len = client.socket.EndReceive(result);
                if (len > 0)
                {
                    byte[] data = new byte[len];
                    Buffer.BlockCopy(client.buffer, 0, data, 0, len);
                    //用户接受消息
                    client.Receive(data);
                    //尾递归，再次监听客户端消息
                    BeginReceive(client);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void ClientReceiveStart(Client client)
        {
            if (!client.socket.Connected)
            {
                return;
            }

            ReceiveData.instance.BeginReceive(client);

            while (true)
            {
                if (!client.socket.Connected)
                {
                    return;
                }
                Thread.Sleep(10);
            }
        }
    }
}
