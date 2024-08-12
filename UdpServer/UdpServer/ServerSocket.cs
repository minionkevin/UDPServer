using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace UdpServer
{
    internal class ServerSocket
    {
        private Socket socket;
        private bool isClose;
        private Dictionary<string, ClientInfo> clientDic = new Dictionary<string, ClientInfo>();
        public void Start(string ip, int port)
        {
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            try { 
                socket.Bind(ipPoint);
                isClose = false;
                ThreadPool.QueueUserWorkItem(ReceiveMsg);
                ThreadPool.QueueUserWorkItem(CheckTimeOut);
            }
            catch(Exception e)
            {
                Console.WriteLine("OPEN UDP FAILED");
            }
        }

        private void ReceiveMsg(object obj)
        {
            byte[] bytes = new byte[512];
            EndPoint ipPoint = new IPEndPoint(IPAddress.Any, 0);
            string clientID = "";
            string ip = "";
            int port;


            while(!isClose)
            {
                if(socket.Available > 0)
                {
                    lock (socket) socket.ReceiveFrom(bytes, ref ipPoint);
                    IPEndPoint ipEndPoint = ipPoint as IPEndPoint;

                    ip = ipEndPoint.Address.ToString();
                    port = ipEndPoint.Port;
                    clientID = String.Concat(ip, port);

                    if (clientDic.ContainsKey(clientID))
                        clientDic[clientID].HandleReceiveMsg(bytes);
                    else
                    {
                        clientDic.Add(clientID, new ClientInfo(ip, port));
                        clientDic[clientID].HandleReceiveMsg(bytes);
                    }
                }
            }
        }

        public void SendToMsg(BaseMsg msg, IPEndPoint ipPoint)
        {
            try
            {
                lock (socket) socket.SendTo(msg.Writing(), ipPoint);
            }
            catch(SocketException e) { 
                Console.WriteLine("SEND SOCKET FAILED");
            }
            catch(Exception e)
            {
                Console.WriteLine("SEND FAILED " + e.Message);
            }
        }

        private void CheckTimeOut(Object obj)
        {
            long currTime = 0;
            List<string> delList = new List<string>();

            while(true)
            {
                Thread.Sleep(30000);
                currTime = DateTime.Now.Ticks / TimeSpan.TicksPerSecond;
                foreach(ClientInfo client in clientDic.Values)
                {
                    if(currTime - client.lastTime >= 10)
                    {
                        delList.Add(client.clientID);
                    }
                }

                foreach(string s in delList)
                {
                    clientDic.Remove(s);
                }
                delList.Clear();
            }
        }


        public void Broardcast(BaseMsg msg)
        {
            foreach(ClientInfo client in clientDic.Values)
            {
                SendToMsg(msg, client.clientIpandPort);
            }
        }

        public void RemoveClient(string clientID)
        {
            if(clientDic.ContainsKey(clientID))
            {
                Console.WriteLine("A CLIENT HAS BEEN REMOVED");
                clientDic.Remove(clientID);
            }
        }

        public void Close()
        {
            if(socket!=null)
            {
                isClose = true;
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
                socket = null;
            }
        }
    }
}
