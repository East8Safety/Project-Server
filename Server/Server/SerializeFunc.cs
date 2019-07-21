using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GameServer
{
    class SerializeFunc
    {
        public static readonly SerializeFunc Instance = new SerializeFunc();

        public byte[] Serialize(Model model)
        {
            try
            {
                //涉及格式转换，需要用到流，将二进制序列化到流中
                using (MemoryStream ms = new MemoryStream())
                {
                    //使用ProtoBuf工具的序列化方法
                    ProtoBuf.Serializer.Serialize<Model>(ms, model);
                    //定义二级制数组，保存序列化后的结果
                    byte[] result = new byte[ms.Length];
                    //将流的位置设为0，起始点
                    ms.Position = 0;
                    //将流中的内容读取到二进制数组中
                    ms.Read(result, 0, result.Length);
                    return result;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("序列化失败: {0}", ex.ToString()));
                return null;
            }
        }

        // 将收到的消息反序列化成对象
        // < returns>The serialize.< /returns>
        // < param name="msg">收到的消息.</param>
        public Model DeSerialize(byte[] msg)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    //将消息写入流中
                    ms.Write(msg, 0, msg.Length);
                    //将流的位置归0
                    ms.Position = 0;
                    //使用工具反序列化对象
                    Model result = ProtoBuf.Serializer.Deserialize<Model>(ms);
                    return result;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("反序列化失败: {0}", ex.ToString()));
                return null;
            }
        }
    }
}
