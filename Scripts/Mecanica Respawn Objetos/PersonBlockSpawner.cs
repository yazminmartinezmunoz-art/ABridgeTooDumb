using System.Diagnostics.Contracts;
using Photon.Pun;
using UnityEngine;
using static SonidosConstruccion;
using UnityEngine.Audio;

public class PersonBlockSpawner : MonoBehaviourPun
{
    [SerializeField] private int playersInTrigger;
    [SerializeField] private int neededPlayers;

    [Header("Spawn")]
    public GameObject[] blockToSpawn;
    public Transform blockSpawnPoint;

    [Header("Efectos")]
    public GameObject efectoBlockToSpawn;
    public AudioSource audioSource;
    public AudioClip sonidoSpawn;

    [Header("Animación botón")]
    public Animator animator;
    public string triggerAnimacion = "Activar";

    private bool yaSpawneo = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.Log("No es master client");
            return;
        }

        if (!other.CompareTag("Player"))
            return;

        playersInTrigger++;

        if (playersInTrigger >= neededPlayers && !yaSpawneo)
        {
            yaSpawneo = true;

            int i = Random.Range(0, blockToSpawn.Length);

            //Spawn del bloque para todos
            PhotonNetwork.Instantiate(
                blockToSpawn[i].name,
                blockSpawnPoint.position,
                Quaternion.identity
            );

            //Ejecuta efectos y animación para todos
            photonView.RPC("SpawnEffectsRPC", RpcTarget.All);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        if (other.CompareTag("Player"))
        {
            playersInTrigger--;

            //Evita valores negativos
            playersInTrigger = Mathf.Max(0, playersInTrigger);

            //Reinicia el botón cuando ya no queda nadie dentro
            if (playersInTrigger == 0)
            {
                yaSpawneo = false;
            }
        }
    }

    [PunRPC]
    void SpawnEffectsRPC()
    {
        //Partículas
        if (efectoBlockToSpawn != null)
        {
            GameObject efecto = Instantiate(
                efectoBlockToSpawn,
                blockSpawnPoint.position,
                Quaternion.identity
            );

            //Destroy(efecto, 5f);
        }

        //Sonido
        if (audioSource != null && sonidoSpawn != null)
        {
            audioSource.PlayOneShot(sonidoSpawn);
        }

        //Animación
        if (animator != null)
        {
            animator.SetTrigger(triggerAnimacion);
        }
    }
}
