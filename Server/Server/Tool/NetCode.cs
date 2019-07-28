using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GameServer
{
    public class NetCode
    {
        public static readonly NetCode Instance = new NetCode();

        //编码，加入包头
        public byte[] Encode(int messageType, byte[] data)
        {
            byte[] result = new byte[data.Length + 8];
            //使用流将编码写二进制
            MemoryStream ms = new MemoryStream();
            BinaryWriter br = new BinaryWriter(ms);
            br.Write(messageType);
            br.Write(data.Length);
            br.Write(data);
            //将流中的内容复制到数组中
            System.Buffer.BlockCopy(ms.ToArray(), 0, result, 0, (int)ms.Length);
            br.Close();
            ms.Close();
            return result;
        }

        //解码
        public Message Decode(ref List<byte> cache)
        {
            Message msg = new Message();

            //首先要获取长度，整形4个字节，如果字节数不足4个字节
            if (cache.Count < 8)
            {
                return null;
            }
            //读取数据
            MemoryStream ms = new MemoryStream(cache.ToArray());
            BinaryReader br = new BinaryReader(ms);
            int messageType = br.ReadInt32();
            int len = br.ReadInt32();
            //根据长度，判断内容是否传递完毕
            if (len > ms.Length - ms.Position)
            {
                return null;
            }
            //获取数据
            byte[] result = br.ReadBytes(len);
            //清空消息池
            cache.Clear();
            //讲剩余没处理的消息存入消息池
            cache.AddRange(br.ReadBytes((int)ms.Length - (int)ms.Position));
            br.Close();
            ms.Close();

            msg.msg = result;
            msg.messageType = messageType;
            return msg;
        }
    }
}
