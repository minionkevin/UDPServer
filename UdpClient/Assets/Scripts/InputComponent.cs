using UnityEngine;

public class InputComponent : MonoBehaviour
{
    public void SendMsg()
    {
        PlayerMsg msg = new PlayerMsg();
        msg.playerData = new PlayerData();
        msg.playerID = 12;
        msg.playerData.name = "MESSAGE FROM CLIENT";
        msg.playerData.atk = 88;
        msg.playerData.lev = 66;
        UDPNetMgr.Instance.SendMsg(msg);
    }
}
