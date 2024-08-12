// See https://aka.ms/new-console-template for more information

using System.Net;
using System.Net.Sockets;
using System.Text;

Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8081);
socket.Bind(ipPoint);
Console.WriteLine("SERVER OKAY");

// Receive
byte[] bytes = new byte[512];
EndPoint remoteIpPoint = new IPEndPoint(IPAddress.Any, 0);
int length = socket.ReceiveFrom(bytes, ref remoteIpPoint);
Console.WriteLine("MESSAGE RECEIVE " + Encoding.UTF8.GetString(bytes, 0, length));

// Send
socket.SendTo(Encoding.UTF8.GetBytes("WELCOME TO THE SERVER"), remoteIpPoint);

socket.Shutdown(SocketShutdown.Both);
socket.Close();

Console.ReadKey();