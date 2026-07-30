using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviourPun
{
    public static GameManager instance;

    public GameObject panelDerrota;
    public GameObject panelVictoria;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        panelDerrota.SetActive(false);
        panelVictoria.SetActive(false);
    }

    public void Victory()
    {
        photonView.RPC("RPC_Victory", RpcTarget.All);
    }

    public void Lose()
    {
        photonView.RPC("RPC_Lose", RpcTarget.All);
    }

    [PunRPC]
    public void RPC_Victory()
    {
        PhotonNetwork.LoadLevel("Win");

        /*
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        panelVictoria.SetActive(true);
        Debug.Log("Ganaste");
        */
    }

    [PunRPC]
    public void RPC_Lose()
    {
        PhotonNetwork.LoadLevel("GameOver");

        /*
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        panelDerrota.SetActive(true);
        Debug.Log("Perdiste");
        */
    }
}
