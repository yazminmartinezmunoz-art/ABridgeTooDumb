using UnityEngine;
using UnityEngine.Audio;

public class EjemploSonido : MonoBehaviour
{
    public AudioSource AudioSource;
    public AudioClip ataque;
    public AudioClip power;
    public AudioClip termino;
    


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            AudioSource.Play(); //Reproducir el clip, renueva la reproduccion

        if (Input.GetKeyDown(KeyCode.Alpha2))
            AudioSource.PlayOneShot(AudioSource.clip); //Reproducir un clip en especifico
        //Este mo remueva la reproduccion, permite solapar el sonido

    }
    public void Ataque()
    {
        AudioSource.pitch = Random.Range(0.9f, 1.1f); //Para variar el sonido y no sea tan monotono
        AudioSource.PlayOneShot(ataque);
    }
    public void FinAccion()
    {
        AudioSource.Stop();
        AudioSource.PlayOneShot(termino);
    }
    public void Power()
    {
        AudioSource.pitch = Random.Range(0.9f, 1.1f); //Para variar el sonido y no sea tan monotono
        AudioSource.PlayOneShot(power);
    }

}
