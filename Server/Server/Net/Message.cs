using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer
{
    public class Message
    {
        //clientId
        public int clientId { get; set; }
        //消息类型
        public int messageType { get; set; }
        //数据
        public byte[] msg { get; set; }
    }
}
