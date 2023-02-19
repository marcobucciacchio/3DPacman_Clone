using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaveRoomMenu : MonoBehaviour
{
    private RoomsCanvases _roomCanvases;
    public void OnClick_LeaveRoom()
    {
        PhotonNetwork.LeaveRoom(true);
        _roomCanvases.CurrentRoomCanvas.Hide();
    }

    public void FirstInitialize(RoomsCanvases canvases)
    {
        _roomCanvases = canvases;
    }
}
