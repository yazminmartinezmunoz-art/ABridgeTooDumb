using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;

public class PlayerReady : MonoBehaviour
{
    public void CambiarEstado()
    {
        bool listo = EstaListo();

        Hashtable propiedades = new Hashtable();
        propiedades["Ready"] = !listo;

        PhotonNetwork.LocalPlayer.SetCustomProperties(propiedades);
    }

    public bool EstaListo()
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Ready"))
        {
            return (bool)PhotonNetwork.LocalPlayer.CustomProperties["Ready"];
        }

        return false;
    }
}