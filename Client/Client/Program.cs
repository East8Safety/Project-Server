using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace GameClient
{
    class Program
    {
        public static bool canSend = false;

        static void Main(string[] args)
        {
            Client.instance.Connect();
            Client.instance.ThreadSendStart();
            Client.instance.ThreadReceiveStart();
            int a = 0;
            while (true)
            {
                if(canSend == true)
                {
                    if (a < 100)
                    {
                        Move move = new Move();
                        move.x = 1; move.z = 1;
                        Message msg = new Message();
                        msg.messageType = (int)messageType.C2SMove;
                        msg.msg = SerializeFunc.instance.Serialize(move);

                        lock (Client.instance.messageWaited)
                        {
                            Client.instance.messageWaited.Enqueue(msg);
                        }
                        a++;
                    }
                }
                

                Message msg2;
                lock (Client.instance.messageReceived)
                {
                    if (!Client.instance.messageReceived.TryDequeue(out msg2))
                    {
                        continue;
                    }
                }

                switch (msg2.messageType)
                {
                    case (int)messageType.S2CMove:
                        AllCharLocation allCharLocation = SerializeFunc.instance.DeSerialize<AllCharLocation>(msg2.msg);
                        foreach (var item in allCharLocation.allCharLocation)
                        {
                            var location = item.Value;
                            Console.WriteLine(string.Format("Char:{0} location: {1} {2} {3}:{4}", item.Key, location.locationX, location.locationZ,
                                DateTime.Now.ToString(), DateTime.Now.Millisecond.ToString()));
                        }
                        break;
                    case (int)messageType.S2CSendCharId:
                        CharId charId = SerializeFunc.instance.DeSerialize<CharId>(msg2.msg);
                        canSend = true;
                        Console.WriteLine(string.Format("This client's charId: {0}", charId.charId));
                        break;
                    case (int)messageType.S2CJoinNewPlayer:
                        NewCharId newCharId = SerializeFunc.instance.DeSerialize<NewCharId>(msg2.msg);
                        Console.WriteLine(string.Format("New Char: {0}", newCharId.charId));
                        break;
                    default:
                        break;
                }
                Thread.Sleep(20);
            }
        }
    }
}
