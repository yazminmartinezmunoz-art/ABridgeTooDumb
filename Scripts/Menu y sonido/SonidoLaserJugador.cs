using UnityEngine;
using Photon.Pun;

public class SonidoLaserJugador : MonoBehaviourPun
{
    public AudioSource audioSource;

    [Header("Sonidos")]
    public AudioClip sonidoAgarrar;

    private void Awake()
    {
        audioSource.loop = true;
    }

    public void IniciarAgarre()
    {
        photonView.RPC(
            "RPC_IniciarAgarre",
            RpcTarget.All);
    }

    public void DetenerAgarre()
    {
        photonView.RPC(
            "RPC_DetenerAgarre",
            RpcTarget.All);
    }

    [PunRPC]
    void RPC_IniciarAgarre()
    {
        if (sonidoAgarrar == null)
            return;

        if (audioSource.isPlaying)
            return;

        audioSource.clip =
            sonidoAgarrar;

        audioSource.pitch =
            Random.Range(0.95f, 1.05f);

        audioSource.Play();
    }

    [PunRPC]
    void RPC_DetenerAgarre()
    {
        audioSource.Stop();
    }
}