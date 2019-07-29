using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GameServer
{
    public class SerializeFunc
    {
        public static readonly SerializeFunc instance = new SerializeFunc();

        //序列化
        public byte[] Serialize<T>(T model)
        {
            //涉及格式转换，需要用到流，将二进制序列化到流中
            using (MemoryStream ms = new MemoryStream())
            {
                //使用ProtoBuf工具的序列化方法
                ProtoBuf.Serializer.Serialize<T>(ms, model);
                //定义二级制数组，保存序列化后的结果
                byte[] result = new byte[ms.Length];
                //将流的位置设为0，起始点
                ms.Position = 0;
                //将流中的内容读取到二进制数组中
                ms.Read(result, 0, result.Length);
                return result;
            }
        }

        //反序列化
        public T DeSerialize<T>(byte[] msg)
        {
            using (MemoryStream ms = new MemoryStream(msg, false))
            {
                return ProtoBuf.Serializer.Deserialize<T>(ms);
            }
        }
    }
}
