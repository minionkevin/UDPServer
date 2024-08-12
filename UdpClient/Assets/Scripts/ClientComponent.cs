using UnityEngine;

public class ClientComponent : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        // IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8080);
        // socket.Bind(ipPoint);
        //
        // IPEndPoint remoteIpPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8081);
        // socket.SendTo(Encoding.UTF8.GetBytes("CLIENT IS HERE"),remoteIpPoint);
        //
        // byte[] bytes = new byte[512];
        // EndPoint remoteIpPoint2 = new IPEndPoint(IPAddress.Any, 0);
        // int length = socket.ReceiveFrom(bytes, ref remoteIpPoint2);
        //
        // print("MESSAGE RECEIVE " + Encoding.UTF8.GetString(bytes,0,length));
        //
        // socket.Shutdown(SocketShutdown.Both);
        // socket.Close();

        if (UDPNetMgr.Instance == null)
        {
            GameObject obj = new GameObject("UDPMgr");
            obj.AddComponent<UDPNetMgr>();
        }
        UDPNetMgr.Instance.HandleStartClient("127.0.0.1",8080);
    }
}
