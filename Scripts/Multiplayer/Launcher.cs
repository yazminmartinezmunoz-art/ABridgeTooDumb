using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Launcher : MonoBehaviourPunCallbacks
{
    public string playerPrefabName;
    public Transform spawnPoint;

    private void Start()
    {
        PhotonNetwork.Instantiate(playerPrefabName, spawnPoint.position, spawnPoint.rotation);
    }


    /*
    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 10;
        PhotonNetwork.JoinOrCreateRoom("test_room", roomOptions, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.Instantiate(playerPrefabName, spawnPoint.position, spawnPoint.rotation);
    }
    */
}