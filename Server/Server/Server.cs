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
        public static readonly Server Instance = new Server();
        //套接字
        private Socket socket;
        //最大连接数
        private int maxClient = 10;
        //端口
        private int port = 35353;
        private Stack<Client> pools;

        private Server()
        {
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

            while (true)
            {
                socket.BeginAccept(AsyncAccept, null);
                Thread.Sleep(10);
            }
        }

        public void InitPool()
        {
            //实例化客户端的用户池
            pools = new Stack<Client>(maxClient);
            for (int i = 0; i < maxClient; i++)
            {
                Client client = new Client();
                pools.Push(client);
            }
        }

        private void AsyncAccept(IAsyncResult result)
        {
            try
            {
                //结束监听，同时获取到客户端
                Socket client = socket.EndAccept(result);
                Console.WriteLine("有客户端连接");
                //来了一个客户端
                Client userClient = pools.Pop();
                userClient.socket = client;
                //客户端连接之后，可以接受客户端消息
                BeginReceive(userClient);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        //异步监听消息
        private void BeginReceive(Client userClient)
        {
            try
            {
                //异步方法
                userClient.socket.BeginReceive(userClient.buffer, 0, userClient.buffer.Length, SocketFlags.None,
                        EndReceive, userClient);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        //监听到消息之后调用的函数
        private void EndReceive(IAsyncResult result)
        {
            try
            {
                //取出客户端
                Client userClient = result.AsyncState as Client;
                //获取消息的长度
                int len = userClient.socket.EndReceive(result);
                if (len > 0)
                {
                    byte[] data = new byte[len];
                    Buffer.BlockCopy(userClient.buffer, 0, data, 0, len);
                    //用户接受消息
                    userClient.Receive(data);
                    //尾递归，再次监听客户端消息
                    //BeginReceive(userClient);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}