using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using ExitGames.Client.Photon;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [Header("UI")]
    public TMP_InputField codigoSalaInput;
    public TMP_Text listaJugadores;
    public TMP_Text nombreSalaTexto;
    public GameObject roomUI;
    public GameObject lobbyUI;
    [Header("Advertencia")]
    public GameObject panelAdvertencia;
    public GameObject advertenciaCampoVacio;
    public GameObject advertenciaSalaNoValida;
    private bool iniciarPartidaSolo = false;
    private void Start()
    {
        ActualizarLista();

        if (PhotonNetwork.InRoom)
        {
            roomUI.SetActive(true);
            lobbyUI.SetActive(false);
        }
    }

    public void ReloadScene()
    {
        if (!PhotonNetwork.InLobby)
        {
            Debug.Log("Aún no estás dentro del Lobby de Photon");
            return;
        }

        PhotonNetwork.LoadLevel("Lobby");
    }

    #region SALAS

    public void CrearSala()
    {
        if (!PhotonNetwork.InLobby)
        {
            Debug.Log("Aún no estás dentro del Lobby de Photon");
            return;
        }

        string codigo = codigoSalaInput.text.Trim();

        if (string.IsNullOrWhiteSpace(codigo)) return;

        RoomOptions opciones = new RoomOptions
        {
            MaxPlayers = 4
        };

        PhotonNetwork.CreateRoom(codigo, opciones);
    }

    public void UnirseSala()
    {
        if (string.IsNullOrWhiteSpace(codigoSalaInput.text))
        {
            Debug.Log("Campo vacio");
            advertenciaCampoVacio.SetActive(true);
            return;
        }

        if (!PhotonNetwork.InLobby)
        {
            Debug.Log("Aún no estás dentro del Lobby de Photon");
            return;
        }


        PhotonNetwork.JoinRoom(codigoSalaInput.text.Trim());
    }

    public void SalirSala()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void VolverMenu()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            SceneManager.LoadScene("Menu");
        }
    }

    #endregion

    #region READY

    bool TodosListos()
    {
        foreach (Player jugador in PhotonNetwork.PlayerList)
        {
            if (!jugador.CustomProperties.ContainsKey("Ready"))
                return false;

            if (!(bool)jugador.CustomProperties["Ready"])
                return false;
        }

        return true;
    }

    #endregion

    #region INICIAR PARTIDA

    public void IniciarPartida()
    {
        Debug.Log("Botón Iniciar presionado");

        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.Log("No soy el Host");
            return;
        }

        // Si está solo, mostrar advertencia
        if (PhotonNetwork.CurrentRoom.PlayerCount < 2)
        {
            iniciarPartidaSolo = true;
            panelAdvertencia.SetActive(true);
            return;
        }

        if (!TodosListos())
        {
            Debug.Log("No todos están listos");
            return;
        }

        Debug.Log("Cargando escena Juego");

        PhotonNetwork.LoadLevel("Juego");
    }

    public void CerrarAdvertencia()
    {
        panelAdvertencia.SetActive(false);

        if (iniciarPartidaSolo)
        {
            iniciarPartidaSolo = false;

            Debug.Log("Iniciando partida en solitario");

            PhotonNetwork.LoadLevel("Juego");
        }
    }

    #endregion

    #region CALLBACKS PHOTON

    public override void OnCreatedRoom()
    {
        Debug.Log("Sala creada correctamente");
        roomUI.SetActive(true);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Error al crear sala: " + message);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        if (returnCode == ErrorCode.GameDoesNotExist)
        {
            Debug.Log("Error al unirse: La sala introducida no existe.");
        }

        else
        {
            Debug.Log("No se pudo entrar a la sala: " + message);
        }

        if (!PhotonNetwork.InRoom)
        {
            advertenciaSalaNoValida.SetActive(true);
        }

        //En caso de que falle lolvemoa al lobby de photon porque al intentar unirse Photon nos saca automaticamente pero no nos
        //devuelve al lobby. (lobby no es lo mismo que sala)
        PhotonNetwork.JoinLobby();
        

    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Entraste a la sala");
        roomUI.SetActive(true);
        lobbyUI.SetActive(false);

        ActualizarLista();
    }

    public override void OnLeftRoom()
    {
        listaJugadores.text = "";

        if (nombreSalaTexto != null)
            nombreSalaTexto.text = "";

        SceneManager.LoadScene("Menu");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(newPlayer.NickName + " entró a la sala");

        ActualizarLista();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log(otherPlayer.NickName + " salió de la sala");

        ActualizarLista();
    }

    public override void OnPlayerPropertiesUpdate(
        Player targetPlayer,
        Hashtable changedProps)
    {
        ActualizarLista();
    }

    #endregion

    #region UI

    void ActualizarLista()
    {
        if (!PhotonNetwork.InRoom)
            return;

        if (nombreSalaTexto != null)
        {
            nombreSalaTexto.text =
                "Sala: " + PhotonNetwork.CurrentRoom.Name +
                "\nJugadores: " +
                PhotonNetwork.CurrentRoom.PlayerCount +
                "/" +
                PhotonNetwork.CurrentRoom.MaxPlayers;
        }

        listaJugadores.text = "";

        foreach (Player jugador in PhotonNetwork.PlayerList)
        {
            bool listo = false;

            if (jugador.CustomProperties.ContainsKey("Ready"))
            {
                listo = (bool)jugador.CustomProperties["Ready"];
            }

            string estado = listo
                ? "<color=green>Listo</color>"
                : "<color=red>No listo</color>";

            // Sprite 0 del TMP Sprite Asset
            string icono = "<sprite=0> ";

            listaJugadores.text +=
                icono +
                jugador.NickName +
                " - " +
                estado +
                "\n";
        }
    }

    #endregion
}