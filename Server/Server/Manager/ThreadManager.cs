using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace GameServer
{
    public class ThreadManager
    {
        public static readonly ThreadManager instance = new ThreadManager();

        public Thread listeningThread;
        public Thread serverSendThread;
        public Thread serverReceiveThread;
        public Thread eventManagerThread;
        public Thread serverUpdateThread;

        public void AllThreadStart()
        {
            ListeningThreadStart();
            ServerSendThreadStart();
            ServerReceiveThread();
            EventManagerThreadStart();
            ServerUpdateThreadStart();
        }

        //开启监听线程
        public void ListeningThreadStart()
        {
            listeningThread = new Thread(() =>
            {
                Server.instance.Start();
            });
            listeningThread.Name = "listeningThread";
            listeningThread.Start();
        }

        //开启发送线程
        public void ServerSendThreadStart()
        {
            serverSendThread = new Thread(() =>
            {
                SendData.instance.ServerSendStart();
            });
            serverSendThread.Name = "serverSendThread";
            serverSendThread.Start();
        }

        //开启主逻辑线程
        public void EventManagerThreadStart()
        {

            eventManagerThread = new Thread(() =>
            {
                EventManager.instance.Start();
            });
            eventManagerThread.Name = "eventManagerThread";
            eventManagerThread.Start();
        }

        //开启服务器update线程
        public void ServerUpdateThreadStart()
        {
            serverUpdateThread = new Thread(() => {
                ServerUpdate.instance.Update();
            });
            serverUpdateThread.Name = "serverUpdateThread";
            serverUpdateThread.Start();
        }

        //开启接收线程
        public void ServerReceiveThread()
        {
            serverReceiveThread = new Thread(() =>
            {
                ReceiveData.instance.Start();
            });
            serverReceiveThread.Name = "serverReceiveThread";
            serverReceiveThread.Start();
        }
    }
}
