using Photon.Pun;
using ExitGames.Client.Photon;
using UnityEngine;

public class ReadyManager : MonoBehaviour
{
    public void CambiarReady()
    {
        bool estadoActual = false;

        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Ready"))
        {
            estadoActual =
                (bool)PhotonNetwork.LocalPlayer.CustomProperties["Ready"];
        }

        Hashtable propiedades = new Hashtable
        {
            { "Ready", !estadoActual }
        };

        PhotonNetwork.LocalPlayer.SetCustomProperties(propiedades);
    }
}