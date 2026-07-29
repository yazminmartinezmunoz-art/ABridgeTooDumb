using UnityEngine;

public class Respawner : MonoBehaviour
{
    public Transform respawnPoint;
    public Transform objectRespawnPosition;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Transform playerT = other.transform;

            Debug.Log(playerT.name + "ha reaparecido");
            playerT.position = respawnPoint.position;
        }

        if (other.gameObject.CompareTag("Object"))
        {
            Transform objectT = other.transform;

            Debug.Log(objectT.name + "ha reaparecido");
            objectT.position = respawnPoint.position;
        }
    }
}
