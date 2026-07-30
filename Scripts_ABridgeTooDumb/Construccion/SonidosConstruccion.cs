using UnityEngine;
using Photon.Pun;

/// Determina QUÉ clip corresponde según el material y el tipo
/// de evento, y sincroniza el disparo entre clientes.
/// La reproducción real ocurre en AudioManager, desacoplada
/// del ciclo de vida de este objeto.
public class SonidosConstruccion : MonoBehaviourPun
{
    public enum TipoSonido
    {
        Impacto,
        Destruccion,
        Agarrar,
        Unir,
        Separar
    }

    [Header("Impacto")]
    public AudioClip impactoMadera;
    public AudioClip impactoRoca;

    [Header("Destrucción")]
    public AudioClip destruccionMadera;
    public AudioClip destruccionRoca;

    [Header("Otros")]
    public AudioClip sonidoAgarrar;
    public AudioClip sonidoUnir;
    public AudioClip sonidoSeparar;

    [Header("Variación de pitch")]
    public float pitchMin = 0.95f;
    public float pitchMax = 1.05f;

    private ObjetoConstruible objeto;

    private void Awake()
    {
        objeto = GetComponent<ObjetoConstruible>();
    }

    /// Punto de entrada público. Sincroniza el sonido
    /// entre todos los clientes.
    ///
    /// Nota: si este método se llama desde adentro de una RPC se van a generar viajes de red innecesarios!!!
    /// Solo llamar si se llama desde un método no RCP y se quiera usar para toda la red
    /// En caso de querer reproducir un sonido que ya viene desde un Método RCP, utilizar ReproducirLocal() más abajo

    public void Reproducir(TipoSonido tipo)
    {
        photonView.RPC("RPC_Reproducir", RpcTarget.All, (int)tipo);
    }

    [PunRPC]
    private void RPC_Reproducir(int tipoInt)
    {
        ReproducirLocal((TipoSonido)tipoInt);
    }

    /// Reproduce el sonido solo en este cliente, sin generar
    /// tráfico de red. Usar cuando el llamador ya está dentro
    /// de un contexto sincronizado (otra RPC).
    public void ReproducirLocal(TipoSonido tipo)
    {
        AudioClip clip = ObtenerClip(tipo);

        if (clip == null || AudioManager.Instance == null)
            return;

        float pitch = Random.Range(pitchMin, pitchMax);

        AudioManager.Instance.ReproducirSFX(clip, transform.position, 1f, pitch);
    }

    private AudioClip ObtenerClip(TipoSonido tipo)
    {
        switch (tipo)
        {
            case TipoSonido.Impacto:
                return objeto.tipoMaterial == ObjetoConstruible.TipoMaterial.Madera
                    ? impactoMadera
                    : impactoRoca;

            case TipoSonido.Destruccion:
                return objeto.tipoMaterial == ObjetoConstruible.TipoMaterial.Madera
                    ? destruccionMadera
                    : destruccionRoca;

            case TipoSonido.Agarrar:
                return sonidoAgarrar;

            case TipoSonido.Unir:
                return sonidoUnir;

            case TipoSonido.Separar:
                return sonidoSeparar;
        }

        return null;
    }
}