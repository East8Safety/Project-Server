using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace GameServer
{
    public class Client
    {
        //连接客户端的Socket
        public Socket socket;
        //用于存放接收数据
        public byte[] buffer;
        //每次接受和发送数据的大小
        private const int size = 1024;

        //接收数据池
        private List<byte> receiveCache;
        //发送数据池
        private Queue<byte[]> sendCache;

        //接收到消息之后的回调
        public Action<Model> receiveCallBack;

        public Client()
        {
            buffer = new byte[size];
            receiveCache = new List<byte>();
            sendCache = new Queue<byte[]>();
        }

        public void Receive(byte[] data)
        {
            //将接收到的数据放入数据池中
            receiveCache.AddRange(data);

            ReadData();
        }

        private void ReadData()
        {
            byte[] data = NetCode.Instance.Decode(ref receiveCache);
            //说明数据保存成功
            if (data != null)
            {
                Model item = SerializeFunc.Instance.DeSerialize(data);
                Console.WriteLine(item.UserId);
                Console.WriteLine(item.UserName);
                Console.WriteLine(item.Message);

                item.Message = "Server Send To Client";
                byte[] temp = SerializeFunc.Instance.Serialize(item);
                Send(temp);
            }
        }

        public void Send(byte[] temp)
        {
            sendCache.Enqueue(temp);

            try
            {
                if (sendCache.Count == 0)
                {
                    return;
                }
                byte[] data = NetCode.Instance.Encode(sendCache.Dequeue());
                int count = data.Length / size;
                int len = size;
                for (int i = 0; i < count + 1; i++)
                {
                    if (i == count)
                    {
                        len = data.Length - i * size;
                    }
                    socket.Send(data, i * size, len, SocketFlags.None);
                }
                Console.WriteLine("发送成功!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
