using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerListingMenu : MonoBehaviourPunCallbacks
{
    [SerializeField] public GameObject menu;
    [SerializeField] public GameObject twoPlayerGame;


    [SerializeField]
    private Transform _content;
    [SerializeField]
    private PlayerListing _playerListing;

    private List<PlayerListing> _listings = new List<PlayerListing>();
    private RoomsCanvases _roomsCanvases;
    private bool _ready = false;

    public Transform[] spawnPoints;

    public override void OnEnable()
    {
       
        base.OnEnable();
 
        GetCurrentRoomPlayers();
    }

    public override void OnDisable()
    {
        base.OnDisable();
        for(int i = 0; i < _listings.Count; i++)
        {
            Destroy(_listings[i].gameObject);
        }
        _listings.Clear();
    }

    private void GetCurrentRoomPlayers()
    {
/*        if (!PhotonNetwork.IsConnected)
            return;
        if (PhotonNetwork.CurrentRoom == null || PhotonNetwork.CurrentRoom.Players == null)
            return;
*/
        foreach (KeyValuePair<int, Player> playerInfo in PhotonNetwork.CurrentRoom.Players)
        {
            AddPlayerListing(playerInfo.Value);
        }


    }

    public void FirstInitialize(RoomsCanvases canvases)
    {
        _roomsCanvases = canvases;
    }


    private void AddPlayerListing(Player player)
    {
        int index = _listings.FindIndex(x => x.Player == player);
        if(index != -1)
        {
            _listings[index].SetPlayerInfo(player);
        }
        else
        {
            PlayerListing listing = Instantiate(_playerListing, _content);
            if (listing != null)
            {
                listing.SetPlayerInfo(player);
                _listings.Add(listing);
            }
        }
        
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        _roomsCanvases.CurrentRoomCanvas.LeaveRoomMenu.OnClick_LeaveRoom();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        AddPlayerListing(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        int index = _listings.FindIndex(x => x.Player == otherPlayer);
        if (index != -1)
        {
            Destroy(_listings[index].gameObject);
            _listings.RemoveAt(index);
        }
    }

    public void OnClick_StartGame()
    {
        Player[] players = PhotonNetwork.PlayerList;
        Debug.Log(players[0]);
 
        for(int i = 0; i < players.Length; i++)
        {
            photonView.RPC("RPCStartGame", players[i], spawnPoints[i].position, Quaternion.identity);
        }
        if (PhotonNetwork.IsMasterClient)
        { 
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
           // PhotonNetwork.LoadLevel(1);


        }

    }
    
    [PunRPC]
    void RPCStartGame(Vector3 spawnPos,Quaternion spawnRot)
    {
        menu.gameObject.SetActive(false);
        twoPlayerGame.SetActive(true);
        PhotonNetwork.Instantiate("player", spawnPos, spawnRot);

    }

    public void OnClick_ReadyUp()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
           // SetReadyUp(!_ready);
            base.photonView.RPC("RPC_ChangeReadyState",RpcTarget.MasterClient,PhotonNetwork.LocalPlayer, _ready);
        }
    }

    [PunRPC]
    private void RPC_ChangeReadyState(Player player, bool ready)
    {
        int index = _listings.FindIndex(x => x.Player == player);
        if (index != -1)
        {
            _listings[index].Ready = ready;
        }
    }
}
