using System.Collections;
using System.Collections.Generic;
using System.Net;
using System;
using System.Net.Sockets;
using System.Threading;

namespace GameServer
{
    public class Server
    {
        public static readonly Server instance = new Server();
        //套接字
        private Socket socket;
        //最大连接数
        private int maxClient = 10;
        //端口
        private int port = 35353;
        private static int clientGuid = 0;
        //等待发送的消息
        public Queue<Message> messageWaited;
        //已接收的消息
        public Queue<Message> messageReceived;
        //用户
        public Dictionary<int, Client> clientPools;
        //clientId与charId绑定
        public Dictionary<int, int> clientId2CharId;
        //线程
        public Thread serverThread;
        public Thread serverSendThread;
        public Thread serverReceiveThread;

        private Server()
        {
            messageWaited = new Queue<Message>();
            messageReceived = new Queue<Message>();
            clientId2CharId = new Dictionary<int, int>();
            //初始化socket
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(new IPEndPoint(IPAddress.Any, port));
        }

        //开启服务器
        public void Start()
        {
            //监听
            socket.Listen(maxClient);
            Console.WriteLine("Server Start OK!");

            //初始化用户池
            InitPool();

            socket.BeginAccept(AsyncAccept, null);

            while (true)
            {
                Thread.Sleep(10);
            }
        }

        public void InitPool()
        {
            //实例化客户端的用户池
            clientPools = new Dictionary<int, Client>(maxClient);
            //for (int i = 0; i < maxClient; i++)
            //{
            //    Client client = new Client();
            //    client.clientId = i;
            //    clientPools.Add(client.clientId, client);
            //}
        }

        private void AsyncAccept(IAsyncResult result)
        {
            try
            {
                //结束监听，同时获取到客户端
                Socket userSocket = socket.EndAccept(result);
                Console.WriteLine("Client Connected");
                //来了一个客户端
                Client client = new Client();
                client.clientId = clientGuid;
                clientGuid++;
                client.socket = userSocket;
                clientPools.Add(client.clientId, client);

                EventManager.instance.AddEvent(()=> {
                    GameProcess.instance.CreateCharacter(client.clientId);
                });

                EventManager.instance.AddEvent(() => {
                    int charId;
                    Server.instance.clientId2CharId.TryGetValue(client.clientId, out charId);
                    GameProcess.instance.SendCharId(client.clientId, charId);
                });

                EventManager.instance.AddEvent(() => {
                    GameProcess.instance.UpdateCharacters();
                });

                socket.BeginAccept(AsyncAccept, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        //开启监听线程
        public void ThreadStart()
        {
            serverThread = new Thread(() =>
            {
                Start();
            });
            serverThread.Name = "serverThread";
            serverThread.Start();
        }

        //开启发送线程
        public void ThreadSendStart()
        {
            serverSendThread = new Thread(() =>
            {
                SendData.instance.ServerSendStart();
            });
            serverSendThread.Name = "serverSendThread";
            serverSendThread.Start();
        }

        //开启接收线程
        public void ThreadReceiveStart()
        {
            serverReceiveThread = new Thread(() =>
            {
                ReceiveData.instance.ServerReceiveStart();
            });
            serverReceiveThread.Name = "serverReceiveThread";
            serverReceiveThread.Start();
        }
    }
}