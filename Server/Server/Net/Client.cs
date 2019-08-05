﻿using System;
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
        public const int size = 1024;
        //clientId
        public int clientId = 0;

        public Client()
        {
            buffer = new byte[size];
        }
    }
}
