using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;

namespace GameClient
{
    public class Client
    {
        public static readonly Client instance = new Client();

        //连接客户端的Socket
        public Socket socket;
        //服务器分配的clientId
        public int clientId;
        //用于存放接收数据
        public byte[] buffer;
        //每次接受和发送数据的大小
        public const int size = 1024;
        //接收缓存
        public List<byte> receiveCache;
        //等待发送的消息
        public Queue<Message> messageWaited;

        //线程
        public Thread clientReceiveThread;
        public Thread clientSendThread;

        public Client()
        {
            buffer = new byte[size];
            receiveCache = new List<byte>();
            messageWaited = new Queue<Message>();
        }

        //连接
        public void Connect()
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ip = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 35353);
            socket.Connect(ip);
            if (socket.Connected)
            {
                this.socket = socket;
                ReceiveData.instance.BeginReceive(this);
            }
        }

        //接收数据
        public void Receive(byte[] data)
        {
            //将接收到的数据放入数据池中
            try
            {
                lock (receiveCache)
                {
                    receiveCache.AddRange(data);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //开启处理接收的消息线程
        public void ThreadReceiveStart()
        {
            clientReceiveThread = new Thread(() =>
            {
                ReceiveData.instance.ClientReceiveStart();
            });
            clientReceiveThread.Start();
        }

        //开启发送线程
        public void ThreadSendStart()
        {
            clientSendThread = new Thread(() =>
            {
                SendData.instance.ClientSendStart();
            });
            clientSendThread.Start();
        }
    }
}