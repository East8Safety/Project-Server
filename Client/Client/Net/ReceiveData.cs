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

        //开始接收数据
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

        //接收完毕
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

        //开始取出缓存数据
        public void ClientReceiveStart()
        {
            while (true)
            {
                ReceiveMessage();
            }
        }

        //取出数据还原包
        public void ReceiveMessage()
        {
            Message msg;
            lock (Client.instance.receiveCache)
            {
                msg = NetCode.Instance.Decode(ref Client.instance.receiveCache);
            }

            if (msg == null)
            {
                return;
            }

            switch (msg.messageType)
            {
                case (int)messageType.S2CMove:
                    AllCharLocation allCharLocation = SerializeFunc.instance.DeSerialize<AllCharLocation>(msg.msg);
                    foreach (var item in allCharLocation.allCharLocation)
                    {
                        var location = item.Value;
                        Console.WriteLine(string.Format("Char:{0} location: {1} {2} {3}:{4}", item.Key, location.locationX, location.locationZ,
                            DateTime.Now.ToString(), DateTime.Now.Millisecond.ToString()));
                    }
                    break;
                case (int)messageType.S2CSendCharId:
                    CharId data = SerializeFunc.instance.DeSerialize<CharId>(msg.msg);
                    Client.instance.clientId = data.clientId;
                    Program.canSend = true;
                    Console.WriteLine(string.Format("This client's charId: {0}", data.charId));
                    break;
                case (int)messageType.S2CJoinNewPlayer:
                    NewCharId newCharId = SerializeFunc.instance.DeSerialize<NewCharId>(msg.msg);
                    Console.WriteLine(string.Format("New Char: {0}", newCharId.charId));
                    break;
                default:
                    break;
            }
        }
    }
}
