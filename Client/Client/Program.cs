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

            //AllCharLocation allCharLocation = new AllCharLocation();
            //allCharLocation.allCharLocation = new Dictionary<int, Location>();
            //Location l = new Location();
            //l.x = 1; l.z = 2; l.locationX = 3; l.locationZ = 4;
            //allCharLocation.allCharLocation.Add(1, l);

            //byte[] b = SerializeFunc.instance.Serialize(allCharLocation);
            //AllCharLocation allCharLocation2 = SerializeFunc.instance.DeSerialize<AllCharLocation>(b);
            //int a = 0;

            while (true)
            {
                if(canSend == true)
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
                            Console.WriteLine(string.Format("Char:{0} location: {1} {2}", item.Key, location.locationX, location.locationZ));
                        }
                        break;
                    case (int)messageType.S2CSendCharId:
                        CharId charId = SerializeFunc.instance.DeSerialize<CharId>(msg2.msg);
                        canSend = true;
                        Console.WriteLine(string.Format("This client's charId: {0}", charId.charId));
                        break;
                    case (int)messageType.S2CUpdateChar:
                        AllCharIds allCharIds = SerializeFunc.instance.DeSerialize<AllCharIds>(msg2.msg);
                        Console.WriteLine(string.Format("Client count: {0}", allCharIds.charIds.Count));
                        break;
                    default:
                        break;
                }

                Thread.Sleep(10);
            }
            
        }
    }
}
