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
        //用于存放接收数据
        public byte[] buffer;
        //每次接受和发送数据的大小
        public const int size = 1024;
        //接收缓存
        public List<byte> receiveCache;
        //等待发送的消息
        public Queue<Message> messageWaited;
        //已接收的消息
        public Queue<Message> messageReceived;

        //线程
        public Thread clientReceiveThread;
        public Thread clientSendThread;

        public Client()
        {
            buffer = new byte[size];
            receiveCache = new List<byte>();
            messageWaited = new Queue<Message>();
            messageReceived = new Queue<Message>();
        }

        public void Connect()
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ip = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 35353);
            socket.Connect(ip);
            if (socket.Connected)
            {
                this.socket = socket;
            }
        }

        public void Receive(byte[] data)
        {
            Message msg;
            //将接收到的数据放入数据池中
            try
            {
                receiveCache.AddRange(data);
                msg = NetCode.Instance.Decode(ref receiveCache);
                if (msg == null)
                {
                    return;
                }
                lock (messageReceived)
                {
                   messageReceived.Enqueue(msg);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void ThreadReceiveStart()
        {
            clientReceiveThread = new Thread(() =>
            {
                ReceiveData.instance.ClientReceiveStart(this);
            });
            clientReceiveThread.Start();
        }

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