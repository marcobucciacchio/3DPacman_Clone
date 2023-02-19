using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiatePacMan : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject _prefab;

/*    [SerializeField]
    private Transform[] spawnpoints;
*/
    private void Awake()
    {

        Vector3 position = new Vector3(transform.position.x + 10f, transform.position.y, transform.position.z);
        MasterManager.NetworkInstantiate(_prefab, position, Quaternion.identity);
    }

/*   [PunRPC]
    void RPCStartGame(Vector3 spawnPos, Quaternion spawnRot)
    {
        PhotonNetwork.Instantiate("player", spawnPos, spawnRot);
    }*/
}
