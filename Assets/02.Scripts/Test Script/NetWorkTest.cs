using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetWorkTest : MonoBehaviourPunCallbacks
{
    private void Awake()
    {
        Screen.SetResolution(960,540,false);
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster() =>
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 6 }, null);

    public override void OnJoinedRoom()
    {
        // if()
        PhotonNetwork.Instantiate("Player", new Vector3(-0.5f,2,1.5f), Quaternion.identity);
        
    }
}
