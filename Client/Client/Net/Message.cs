using System;
using System.Collections.Generic;
using System.Text;

namespace GameClient
{
    public class Message
    {
        //消息类型
        public int messageType { get; set; }
        //数据
        public byte[] msg { get; set; }
    }
}
