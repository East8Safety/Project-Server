using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace GameClient
{
    public class SendData
    {
        public static readonly SendData instance = new SendData();

        public void ClientSendStart()
        {
            while (true)
            {
                if (Client.instance.messageWaited.Count > 0)
                {
                    Send();
                }
            }
        }

        public void Send()
        {
            try
            {
                Message msg;
                lock (Client.instance.messageWaited)
                {
                    msg = Client.instance.messageWaited.Dequeue();
                    if (msg == null)
                    {
                        return;
                    }
                }

                byte[] data = NetCode.Instance.Encode(msg.clientId, msg.messageType, msg.msg);
                int count = data.Length / Client.size;
                int len = Client.size;
                for (int i = 0; i < count + 1; i++)
                {
                    if (i == count)
                    {
                        len = data.Length - i * Client.size;
                    }
                    Client.instance.socket.Send(data, i * Client.size, len, SocketFlags.None);
                }
            }
            catch (Exception)
            {

            }
        }

        //放入发送队列
        public void SendMessage<T>(int clientId, int messageType, T model)
        {
            Message msg = new Message();
            msg.clientId = clientId;
            msg.messageType = messageType;
            msg.msg = SerializeFunc.instance.Serialize(model);
            lock (Client.instance.messageWaited)
            {
                Client.instance.messageWaited.Enqueue(msg);
            }
        }
    }
}
