// See https://aka.ms/new-console-template for more information

using System.Net;
using System.Net.Sockets;
using System.Text;
using UdpServer;

namespace MyApp // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        public static ServerSocket serverSocket;
        static void Main(string[] args)
        {
            ServerSocket socket = new ServerSocket();
            socket.Start("127.0.0.1", 8081);
            Console.WriteLine("UDP START");


            while (true)
            {
                string input = Console.ReadLine();
                if(input.Substring(0,2) == "B:")
                {
                    // test boardcast
                    PlayerMsg msg = new PlayerMsg();
                    msg.playerData = new PlayerData();
                    msg.playerID = 111;
                    msg.playerData.name = "TEST";
                    msg.playerData.lev = 12;
                    msg.playerData.atk = 88;
                    serverSocket.Broardcast(msg);
                }
            }
        }
    }
}