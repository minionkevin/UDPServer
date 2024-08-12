using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class UDPNetMgr : MonoBehaviour
{
    private static UDPNetMgr instance;
    public static UDPNetMgr Instance => instance;

    private EndPoint serverIpPoint;
    private Socket clientSocket;
    private bool isClose = true;
    

    private Queue<BaseMsg> sendQueue = new Queue<BaseMsg>();
    private Queue<BaseMsg> receiveQueue = new Queue<BaseMsg>();

    private byte[] cacheBytes = new byte[512];

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public void HandleStartClient(string ip, int port)
    {
        if (!isClose) return;
        serverIpPoint = new IPEndPoint(IPAddress.Parse(ip), port);
        IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8081);
        
        try
        {
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            clientSocket.Bind(clientEndPoint);
            print("CLIENT START");
            isClose = false;
            
            ThreadPool.QueueUserWorkItem(HandleReceive);
            ThreadPool.QueueUserWorkItem(HandleSendTo);
        }
        catch (Exception e)
        {
            print("CLIENT START FAILED" + e.Message) ;
        }
    }

    private void Update()
    {
        if (receiveQueue.Count > 0)
        {
            BaseMsg baseMsg = receiveQueue.Dequeue();
            switch (baseMsg)
            {
                case PlayerMsg msg:
                    print(msg.playerID);
                    print(msg.playerData.name);
                    print(msg.playerData.atk);
                    print(msg.playerData.lev);
                    break;
            }
        }
    }

    private void HandleReceive(object obj)
    {
        EndPoint tempIpPoint = new IPEndPoint(IPAddress.Any, 0);
        int currIndex;
        int msgID;
        int msgLength;
        
        while (!isClose)
        {
            if (clientSocket!=null && clientSocket.Available > 0)
            {
                try
                {
                    clientSocket.ReceiveFrom(cacheBytes, ref tempIpPoint);
                    // Check server or garbage
                    if (!serverIpPoint.Equals(tempIpPoint)) continue;
                    currIndex = 0;
                    msgID = BitConverter.ToInt32(cacheBytes, currIndex);
                    currIndex += 4;
                    msgLength = BitConverter.ToInt32(cacheBytes, currIndex);
                    currIndex += 4;
                    BaseMsg msg = null;
                    switch (msgID)
                    {
                        case 1001:
                            msg = new PlayerMsg();
                            msg.Reading(cacheBytes, currIndex);
                            break;
                    }
                    if(msg!=null) receiveQueue.Enqueue(msg);
                }
                catch (SocketException e)
                {
                    print("RECEIVE FAILED " + e.SocketErrorCode);
                }
                catch (Exception e)
                {
                    print("RECEIVE FAILED");
                }
            }
        }
        
    }
    
    private void HandleSendTo(object obj)
    {
        while (!isClose)
        {
            if (clientSocket!=null && sendQueue.Count > 0)
            {
                try
                {
                    print("MSG SEND");
                    clientSocket.SendTo(sendQueue.Dequeue().Writing(), serverIpPoint);
                }
                catch (Exception e)
                {
                    print("SEND FAILED");
                }
            }
        }
    }

    public void SendMsg(BaseMsg msg)
    {
        sendQueue.Enqueue(msg);
    }

    public void Close()
    {
        if (clientSocket != null) return;
        QuitMsg msg = new QuitMsg();
        clientSocket.SendTo(msg.Writing(),serverIpPoint);
        isClose = true;
        clientSocket.Shutdown(SocketShutdown.Both);
        clientSocket.Close();
        clientSocket = null;
    }

    private void OnDestroy()
    {
        Close();
    }
}
