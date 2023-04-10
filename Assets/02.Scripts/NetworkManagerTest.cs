using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;

public class NetworkManagerTest : MonoBehaviourPunCallbacks
{
    private void Awake()
    {
        Screen.SetResolution(960,540,false);
        PhotonNetwork.ConnectUsingSettings();
    }

    public void OnConnectedToMaster() =>
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 6 }, null);

    public override void OnJoinedRoom()
    {
        // PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity);
    }

}
