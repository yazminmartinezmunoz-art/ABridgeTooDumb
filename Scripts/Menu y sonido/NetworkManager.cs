using Photon.Pun;
using UnityEngine;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager Instance;

    private void Awake()
    {
        Debug.Log("NetworkManager Awake: " + gameObject.scene.name);

        if (Instance != null && Instance != this)
        {
            Debug.Log("Destruyendo NetworkManager duplicado");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            Debug.Log("Conectando a Photon...");
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Conectado al servidor Photon");

        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Entraste al Lobby");
    }

    public override void OnDisconnected(Photon.Realtime.DisconnectCause cause)
    {
        Debug.LogWarning("Desconectado de Photon: " + cause);
    }
}