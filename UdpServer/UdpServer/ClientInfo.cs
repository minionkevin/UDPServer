using MyApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace UdpServer
{
    internal class ClientInfo
    {
        public IPEndPoint clientIpandPort;
        public string clientID;
        public long lastTime = -1;

        public ClientInfo(string ip, int port)
        {
            clientID = string.Concat(ip, port);
            clientIpandPort = new IPEndPoint(IPAddress.Parse(ip), port);
        }

        public void HandleReceiveMsg(byte[] bytes)
        {
            byte[] cacheBytes = new byte[512];
            bytes.CopyTo(cacheBytes, 0);
            lastTime = DateTime.Now.Ticks / TimeSpan.TicksPerSecond;
            ThreadPool.QueueUserWorkItem(ReceiveMsg, cacheBytes);
        }

        private void ReceiveMsg(object obj)
        {
            try 
            {
                byte[] bytes = obj as byte[];

                int index = 0;
                int msgID = BitConverter.ToInt32(bytes, index);
                index += 4;
                int msgLength = BitConverter.ToInt32(bytes, index);
                index += 4;


                switch (msgID)
                {
                    case 1001:
                        PlayerMsg playerMsg = new PlayerMsg();
                        playerMsg.Reading(bytes, index);
                        Console.WriteLine(playerMsg.playerID);
                        Console.WriteLine(playerMsg.playerData.name);
                        Console.WriteLine(playerMsg.playerData.atk);
                        Console.WriteLine(playerMsg.playerData.lev);
                        break;
                    case 1003:
                        QuitMsg quieMsg = new QuitMsg();
                        Console.WriteLine("CLIENT QUIT");
                        Program.serverSocket.RemoveClient(this.clientID);
                        break;
                }
            }
            catch(Exception e) 
            {
                Console.WriteLine("HANDLE RECEIVE MESSAGE FAILED");
                Program.serverSocket.RemoveClient(this.clientID);
            }
        }
    }
}
