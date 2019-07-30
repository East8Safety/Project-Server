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
        //clientId递增
        private static int clientGuid = 0;
        public List<byte> receiveCache;
        //等待发送的消息
        public Queue<Message> messageWaited;
        //用户
        public Dictionary<int, Client> clientPools;
        //clientId与charId绑定
        public Dictionary<int, int> clientId2CharId;
        

        private Server()
        {
            messageWaited = new Queue<Message>();
            receiveCache = new List<byte>();
            clientPools = new Dictionary<int, Client>(maxClient);
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

            socket.BeginAccept(AsyncAccept, null);

            while (true)
            {
                Thread.Sleep(10);
            }
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

                ReceiveData.instance.BeginReceive(client);

                EventManager.instance.AddEvent(()=> {
                    CharacterManager.instance.CreateCharacter(client.clientId);
                });

                EventManager.instance.AddEvent(() => {
                    int charId;
                    clientId2CharId.TryGetValue(client.clientId, out charId);
                    GameProcess.instance.SendCharId(client.clientId, charId);
                });

                EventManager.instance.AddEvent(() => {
                    int charId;
                    clientId2CharId.TryGetValue(client.clientId, out charId);
                    GameProcess.instance.JoinNewPlayer(charId);
                });

                socket.BeginAccept(AsyncAccept, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}