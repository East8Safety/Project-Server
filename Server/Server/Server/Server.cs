﻿using System.Collections;
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
        //接收消息缓存
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
            ConsoleLog.instance.Info(string.Format("服务器启动"));

            socket.BeginAccept(AsyncAccept, null);

            while (true)
            {
                Thread.Sleep(2);
            }
        }

        private void AsyncAccept(IAsyncResult result)
        {
            try
            {
                //结束监听，同时获取到客户端
                Socket userSocket = socket.EndAccept(result);
                
                //来了一个客户端
                Client client = new Client();
                client.clientId = clientGuid;
                clientGuid++;
                client.socket = userSocket;
                lock (clientPools)
                {
                    clientPools.Add(client.clientId, client);
                }

                ConsoleLog.instance.Info(string.Format("客户端连接 clientId: {0}", client.clientId));

                //开启消息接收
                ReceiveData.instance.BeginReceive(client);

                //连接之后任务
                GameProcess.instance.AfterConnect(client.clientId);

                //接着监听
                socket.BeginAccept(AsyncAccept, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        //服务器初始化
        public void Init()
        {
            GameMapManager.instance.CreateMap(10, 10);
        }

        public int GetCharId(int clientId)
        {
            int charId;
            clientId2CharId.TryGetValue(clientId, out charId);
            return charId;
        }
    }
}