using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

/// Manager singleton persistente encargado de reproducir
/// TODOS los efectos de sonido del juego, desacoplados
/// del ciclo de vida de los objetos que los disparan.
/// Usa un pool de AudioSources para evitar Instantiate/Destroy
/// constantes
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Mixer")]
    public AudioMixerGroup grupoSFX;
    public AudioMixerGroup grupoUI;
    public AudioMixerGroup grupoMusica;

    [Header("Pool")]
    [Tooltip("Cantidad de AudioSources 3D disponibles simultáneamente.")]
    public int tamañoPool = 16;

    [Header("Música (opcional)")]
    public AudioSource fuenteMusica;

    private readonly List<AudioSource> pool = new List<AudioSource>();

    private void Awake()
    {
        // Singleton clásico: si ya existe uno, este se destruye.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        CrearPool();
    }

    private void CrearPool()
    {
        for (int i = 0; i < tamañoPool; i++)
        {
            GameObject go = new GameObject($"AudioSource_Pool_{i}");
            go.transform.SetParent(transform);

            AudioSource source = go.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.spatialBlend = 1f; // 3D por defecto
            source.outputAudioMixerGroup = grupoSFX;

            pool.Add(source);
        }
    }

    //Busca un AudioSource libre en el pool.
    //Si todos están ocupados, reutiliza el que menos falta le queda.
    private AudioSource ObtenerFuenteLibre()
    {
        foreach (AudioSource source in pool)
        {
            if (!source.isPlaying)
                return source;
        }

        // Pool lleno: robar la fuente más próxima a terminar
        // en vez de ignorar el sonido nuevo.
        AudioSource sourceUsable = pool[0];
        float menorTiempoRestante = float.MaxValue;

        foreach (AudioSource source in pool)
        {
            float restante = source.clip != null ? (source.clip.length - source.time) : 0f;
            if (restante < menorTiempoRestante)
            {
                menorTiempoRestante = restante;
                sourceUsable = source;
            }
        }

        return sourceUsable;
    }

    public void ReproducirSFX(AudioClip clip, Vector3 posicion, float volumen = 1f, float pitch = 1f)
    {
        if (clip == null) return;

        AudioSource fuente = ObtenerFuenteLibre();

        fuente.transform.position = posicion;
        fuente.clip = clip;
        fuente.volume = volumen;
        fuente.pitch = pitch;
        fuente.spatialBlend = 1f;
        fuente.outputAudioMixerGroup = grupoSFX;
        fuente.Play();
    }

    //Reproduce un sonido 2D (UI, notificaciones) sin posición en el mundo.
    public void ReproducirUI(AudioClip clip, float volumen = 1f)
    {
        if (clip == null) return;

        AudioSource fuente = ObtenerFuenteLibre();

        fuente.clip = clip;
        fuente.volume = volumen;
        fuente.pitch = 1f;
        fuente.spatialBlend = 0f; // 2D
        fuente.outputAudioMixerGroup = grupoUI;
        fuente.Play();
    }

    //Reproduce música de fondo con loop (usa una fuente dedicada, no el pool).
    public void ReproducirMusica(AudioClip clip, float volumen = 1f)
    {
        if (fuenteMusica == null || clip == null) return;

        fuenteMusica.clip = clip;
        fuenteMusica.volume = volumen;
        fuenteMusica.loop = true;
        fuenteMusica.outputAudioMixerGroup = grupoMusica;
        fuenteMusica.Play();
    }

    public void DetenerMusica()
    {
        if (fuenteMusica != null)
            fuenteMusica.Stop();
    }
}