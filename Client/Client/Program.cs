using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace GameClient
{
    public class Program
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
                Move move = new Move();
                Message msg = new Message();
                if (canSend == true)
                {
                    if (a < 100)
                    {
                        
                        move.x = 1; move.z = 1;

                        msg.clientId = Client.instance.clientId;
                        msg.messageType = (int)messageType.C2SMove;
                        msg.msg = SerializeFunc.instance.Serialize(move);

                        lock (Client.instance.messageWaited)
                        {
                            Client.instance.messageWaited.Enqueue(msg);
                        }
                        a++;
                    }
                }
                
                Thread.Sleep(10);
            }
        }
    }
}
