using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class ButtonsSystem : MonoBehaviourPun
{
    public void LoadLobby()
    {
        photonView.RPC("_LoadLobby", RpcTarget.All);
    }

    //[PunRPC]

    public void _LoadLobby()
    {
        //PhotonNetwork.LoadLevel("Lobby");
        SceneManager.LoadScene("Lobby");
    }
}
