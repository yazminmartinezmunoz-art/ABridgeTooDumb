using UnityEngine;

public class AudioButtonController : MonoBehaviour
{
    public AudioSource audioSource;
    public GameObject boton;

    void Start()
    {
        boton.SetActive(false);

        audioSource.Play();

        Invoke(nameof(MostrarBoton), audioSource.clip.length);
    }

    void MostrarBoton()
    {
        boton.SetActive(true);
    }
}