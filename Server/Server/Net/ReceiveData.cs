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
            try
            {
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
                    ReceiveMessage(msg);
                    BeginReceive(client);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void ReceiveMessage(Message msg)
        {
            switch (msg.messageType)
            {
                case (int)messageType.C2SMove:
                    Move move = SerializeFunc.instance.DeSerialize<Move>(msg.msg);
                    EventManager.instance.AddEvent(() =>
                    {
                        int charId;
                        Server.instance.clientId2CharId.TryGetValue(msg.clientId, out charId);
                        GameProcess.instance.ClientMove(charId, move);
                    });

                    return;
                default:
                    return;
            }
        }
    }
}
