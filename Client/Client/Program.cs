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
            Client.instance.Connect("127.0.0.1", 35353);
            Client.instance.ThreadSendStart();
            Client.instance.ThreadReceiveStart();

            //if (canSend == true && isAttack == false)
            {
                C2SAttack data = new C2SAttack();
                Message msg = new Message();

                data.weaponId = 101;
                data.locationX = 4;
                data.locationZ = 5;

                msg.clientId = Client.instance.clientId;
                msg.messageType = (int)messageType.C2SAttack;
                msg.msg = SerializeFunc.instance.Serialize(data);

                lock (Client.instance.messageWaited)
                {
                    Client.instance.messageWaited.Enqueue(msg);
                }
            }

            //while (true)
            //{
            //    if(canSend == true && isAttack == false)
            //    {
            //        C2SAttack data = new C2SAttack();
            //        Message msg = new Message();

            //        data.weaponId = 101;
            //        data.locationX = 4;
            //        data.locationZ = 5;

            //        msg.clientId = Client.instance.clientId;
            //        msg.messageType = (int)messageType.C2SAttack;
            //        msg.msg = SerializeFunc.instance.Serialize(data);

            //        lock (Client.instance.messageWaited)
            //        {
            //            Client.instance.messageWaited.Enqueue(msg);
            //        }

            //        C2SChooseChar c2SChooseChar = new C2SChooseChar();
            //        Message msg2 = new Message();

            //        c2SChooseChar.charType = 1;

            //        msg2.clientId = Client.instance.clientId;
            //        msg2.messageType = (int)messageType.C2SChooseChar;
            //        msg2.msg = SerializeFunc.instance.Serialize(c2SChooseChar);

            //        lock (Client.instance.messageWaited)
            //        {
            //            Client.instance.messageWaited.Enqueue(msg2);
            //        }

            //        isAttack = true;
            //    }

            //    if (canSend == true)
            //    {
            //        C2SMove move = new C2SMove();
            //        Message msg = new Message();

            //        move.x = 1; move.z = 1;

            //        msg.clientId = Client.instance.clientId;
            //        msg.messageType = (int)messageType.C2SMove;
            //        msg.msg = SerializeFunc.instance.Serialize(move);

            //        lock (Client.instance.messageWaited)
            //        {
            //            Client.instance.messageWaited.Enqueue(msg);
            //        }
            //    }

            //    Thread.Sleep(10);
            //}
        }
    }
}
