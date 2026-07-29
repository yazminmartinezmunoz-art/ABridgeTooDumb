using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;
using Photon.Realtime;

/// Comprueba si todos los jugadores
/// han llegado a la meta.
public class EndTrigger : MonoBehaviourPunCallbacks
{
    // Guarda los IDs de los jugadores
    // que están dentro de la meta.
    private HashSet<int> jugadoresEnMeta =
        new HashSet<int>();

    //-------------------------------------------------

    private void OnTriggerEnter(
        Collider other)
    {
        // Solo el MasterClient controla
        // la condición de victoria.
        if (!PhotonNetwork.IsMasterClient)
            return;

        // Nos aseguramos de que sea un jugador.
        if (!other.CompareTag("Player"))
            return;

        PhotonView pv =
            other.GetComponent<PhotonView>();

        if (pv == null)
            return;

        // Agrega el jugador.
        jugadoresEnMeta.Add(
            pv.Owner.ActorNumber);

        Debug.Log(
            "Jugadores en meta: "
            + jugadoresEnMeta.Count
            + "/"
            + PhotonNetwork
                .CurrentRoom
                .PlayerCount);

        // ¿Están todos?
        if (jugadoresEnMeta.Count >=
            PhotonNetwork
                .CurrentRoom
                .PlayerCount)
        {
            photonView.RPC(
                "RPC_EndGame",
                RpcTarget.All);
        }
    }

    //-------------------------------------------------

    private void OnTriggerExit(
        Collider other)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        if (!other.CompareTag("Player"))
            return;

        PhotonView pv =
            other.GetComponent<PhotonView>();

        if (pv == null)
            return;

        // Elimina el jugador.
        jugadoresEnMeta.Remove(
            pv.Owner.ActorNumber);

        Debug.Log(
            "Jugadores en meta: "
            + jugadoresEnMeta.Count
            + "/"
            + PhotonNetwork
                .CurrentRoom
                .PlayerCount);
    }

    //-------------------------------------------------

    // Si alguien abandona la sala
    // y estaba dentro de la meta,
    // también lo eliminamos.
    public override void
        OnPlayerLeftRoom(
        Photon.Realtime.Player otherPlayer)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        jugadoresEnMeta.Remove(
            otherPlayer.ActorNumber);

        Debug.Log(
            "Jugador salió de la sala.");

        Debug.Log(
            "Jugadores en meta: "
            + jugadoresEnMeta.Count
            + "/"
            + PhotonNetwork
                .CurrentRoom
                .PlayerCount);

        // Por si al salir alguien
        // ahora todos los jugadores
        // restantes ya están en la meta.
        if (jugadoresEnMeta.Count >=
            PhotonNetwork
                .CurrentRoom
                .PlayerCount)
        {
            photonView.RPC(
                "RPC_EndGame",
                RpcTarget.All);
        }
    }

    //-------------------------------------------------

    [PunRPC]
    private void RPC_EndGame()
    {
        Debug.Log(
            "¡Juego finalizado! Han llegado a la isla del tesoro.");

        GameManager.instance
            .Victory();
    }
}