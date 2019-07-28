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
                    continue;
                }
                Thread.Sleep(10);
            }
        }

        public void Send()
        {
            Message msg;
            if(Client.instance.messageWaited.Count == 0)
            {
                return;
            }
            lock (Client.instance.messageWaited)
            {
                msg = Client.instance.messageWaited.Dequeue();
                if (msg == null)
                {
                    return;
                }
            }

            byte[] data = NetCode.Instance.Encode(msg.msg, msg.messageType);
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
    }
}
