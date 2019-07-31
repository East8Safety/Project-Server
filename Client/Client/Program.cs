using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace GameClient
{
    public class Program
    {
        public static bool canSend = false;

        public static bool isAttack = false;

        static void Main(string[] args)
        {
            Client.instance.Connect();
            Client.instance.ThreadSendStart();
            Client.instance.ThreadReceiveStart();

            

            while (true)
            {
                if(canSend == true && isAttack == false)
                {
                    C2SAttackModel data = new C2SAttackModel();
                    Message msg = new Message();

                    data.weaponId = 101;
                    data.locationX = 23;
                    data.locationZ = 54;

                    msg.clientId = Client.instance.clientId;
                    msg.messageType = (int)messageType.C2SAttack;
                    msg.msg = SerializeFunc.instance.Serialize(data);

                    lock (Client.instance.messageWaited)
                    {
                        Client.instance.messageWaited.Enqueue(msg);
                    }

                    isAttack = true;
                }

                if (canSend == true)
                {
                    C2SMoveModel move = new C2SMoveModel();
                    Message msg = new Message();

                    move.x = 1; move.z = 1;

                    msg.clientId = Client.instance.clientId;
                    msg.messageType = (int)messageType.C2SMove;
                    msg.msg = SerializeFunc.instance.Serialize(move);

                    lock (Client.instance.messageWaited)
                    {
                        Client.instance.messageWaited.Enqueue(msg);
                    }
                }
                
                Thread.Sleep(10);
            }
        }
    }
}
