using UnityEngine;
using System.Collections.Generic;
using System;
using Photon.Pun;
using UnityEngine.Audio;
using System.Collections;
using System.Drawing;
using Color = UnityEngine.Color;

/// Contiene todos los datos importantes
/// del objeto construible.
public class ObjetoConstruible : MonoBehaviourPun
{
    private Rigidbody rb;
    private Collider myCollider;
    private SonidosConstruccion sonidos;

    [Header("Información")]
    public string nombreObjeto;
    
    [TextArea(3, 5)]
    public string descripcion;
    public Sprite imagenObjeto;

    [Header("Resistencia")]
    public int vidasMaximas = 3;
    public int vidasActuales;

    public ParticleSystem destroyParticles;

    [Header("Outline")]
    public Renderer myRenderer;
    public Material outlineMaterial;
    [SerializeField] private float activeWidth = 1.05f;
    public Color colorMirando = Color.cyan;
    public Color colorUnionPosible = Color.green;
    private static readonly int ThicknessID = Shader.PropertyToID("_Thickness");
    private static readonly int ColorID = Shader.PropertyToID("_ColorHDR");

    public enum TipoMaterial
    {
        Madera,
        Roca
    }

    [Header("Tipo")]
    public TipoMaterial tipoMaterial;
    public PhysicsMaterial noFrictionMaterial;

    [HideInInspector]
    public bool estaSiendoAgarrado;

    [Header("Peso Cooperativo")]

    public int jugadoresNecesarios = 1;

    [HideInInspector]
    public List<int> jugadoresAgarrando = new List<int>();

    [HideInInspector]
    public GrupoConstruccion grupoActual;

    public event Action AlCambiarVida;

    [HideInInspector]
    public PuntoUnion[] puntosUnion;

    public event Action AlCambiarJugadores;

    private void Awake()
    {
        myRenderer = GetComponent<Renderer>();
        myCollider = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
        outlineMaterial = myRenderer.materials[1];
        sonidos = GetComponent<SonidosConstruccion>();

        vidasActuales = vidasMaximas;

        puntosUnion =
            GetComponentsInChildren<PuntoUnion>();



        foreach (PuntoUnion punto in puntosUnion)
        {
            punto.dueño = this;
        }
    }

    void Start()
    {
        ToggleOutline(false);
    }
    /// Reduce vidas del objeto.
    public void RecibirDaño(int daño)
    {
        photonView.RPC("RPC_RecibirDaño", RpcTarget.All, daño);
    }

    [PunRPC]
    private void RPC_RecibirDaño(int daño)
    {
        vidasActuales -= daño;
        sonidos.Reproducir(SonidosConstruccion.TipoSonido.Impacto);
        AlCambiarVida?.Invoke();

        if (vidasActuales <= 0 && photonView.IsMine)
        {
            photonView.RPC("RPC_PedirDestruccion", RpcTarget.All);
        }        
    }

    [PunRPC]
    private void RPC_PedirDestruccion()
    {
        if (grupoActual != null)
            grupoActual.RemoverObjeto(this);

        sonidos.ReproducirLocal(SonidosConstruccion.TipoSonido.Destruccion);

        if (photonView.IsMine)
        {
            if (destroyParticles != null)
                PhotonNetwork.Instantiate(destroyParticles.name, transform.position, Quaternion.identity);

            PhotonNetwork.Destroy(gameObject);
        }
    }

    /// Registra un jugador agarrando.
    public void AgregarJugador(int idJugador)
    {

        if (!jugadoresAgarrando.Contains(idJugador))
        {
            jugadoresAgarrando.Add(idJugador);

            AlCambiarJugadores?.Invoke();
        }

        if (grupoActual != null)
        {
            grupoActual.ActualizarJugadores();
        }
    }

    /// Elimina un jugador.
    public void QuitarJugador(int idJugador)
    {
        if (jugadoresAgarrando.Contains(idJugador))
        {
            jugadoresAgarrando.Remove(idJugador);

            AlCambiarJugadores?.Invoke();
        }

        if (grupoActual != null)
        {
            grupoActual.ActualizarJugadores();
        }
    }

    /// ¿Tiene suficiente fuerza?
    public bool TieneFuerzaSuficiente()
    {
        if (grupoActual != null)
        {
            return grupoActual.TieneFuerzaSuficiente();
        }

        return jugadoresAgarrando.Count >=
               ObtenerJugadoresNecesarios();
    }

    //la etquiquera [PunRPC] permite que el metotodo de abajo se ejecute en todas las computadoras conectadas,
    //aqui se usa para que todos vean el objeto congelado al hacer snap.
    [PunRPC]
    public void RPC_CongelarObjeto()
    {
        rb.isKinematic = true;
        rb.useGravity = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        myCollider.material = null;

        Debug.Log("RPC: Objeto congelado");
    }

    [PunRPC]
    public void RPC_DescongelarObjeto()
    {
        rb.isKinematic = false;
        rb.useGravity = true;
        myCollider.material = null;

        Debug.Log("RPC: Objeto descongelado");
    }

    [PunRPC]
    public void RPC_ObjetoAgarrado()
    {
        rb.isKinematic = false;
        rb.useGravity = false;
        myCollider.material = noFrictionMaterial;

        Debug.Log("RPC: Objeto agarrado");
    }

    [PunRPC]
    public void RPC_AgregarJugadorAgarrando(
    int actorNumber)
    {
        if (!jugadoresAgarrando
            .Contains(actorNumber))
        {
            jugadoresAgarrando
                .Add(actorNumber);

            AlCambiarJugadores?.Invoke();

            if (grupoActual != null)
            {
                grupoActual
                    .ActualizarJugadores();
            }
        }
        Debug.Log(
            "RPC_AgregarJugadorAgarrando " +
            actorNumber);
    }
    [PunRPC]
    public void RPC_QuitarJugadorAgarrando(
    int actorNumber)
    {
        if (jugadoresAgarrando
            .Contains(actorNumber))
        {
            jugadoresAgarrando
                .Remove(actorNumber);

            AlCambiarJugadores?.Invoke();

            if (grupoActual != null)
            {
                grupoActual
                    .ActualizarJugadores();
            }
        }
        Debug.Log(
            "RPC_QuitarJugadorAgarrando " +
            actorNumber);
    }

    [PunRPC]
    public void RPC_CrearGrupo(int viewID)
    {
        PhotonView otroPV = PhotonView.Find(viewID);

        if (otroPV == null)
            return;

        ObjetoConstruible otro = otroPV.GetComponent<ObjetoConstruible>();

        if (otro == null)
            return;

        if (this.grupoActual != null)
        {
            this.grupoActual.AgregarObjeto(otro);
            return;
        }
        if (otro.grupoActual != null)
        {
            otro.grupoActual.AgregarObjeto(this);
            return;
        }

        GameObject nuevoGrupo =new GameObject("GrupoConstruccion");
        GrupoConstruccion grupo = nuevoGrupo.AddComponent<GrupoConstruccion>();
        grupo.AgregarObjeto(this);
        grupo.AgregarObjeto(otro);
    }

    public int ObtenerJugadoresNecesarios()
    {
        int maximo = 1;

        if (Photon.Pun.PhotonNetwork.InRoom)
        {
            maximo =
                Photon.Pun.PhotonNetwork
                .CurrentRoom
                .PlayerCount;
        }

        int resultado = Mathf.Clamp(jugadoresNecesarios, 1, maximo);

        return resultado;
    }

    
    public void ToggleOutline(bool state)
    {
        outlineMaterial.SetFloat(ThicknessID, state ? activeWidth : 0f);

        if (state)
        {
            outlineMaterial.SetColor(ColorID, colorMirando);
        }
            
    }

    public void ActualizarColorOutline(bool unionPosible)
    {
        outlineMaterial.SetColor(ColorID, unionPosible ? colorUnionPosible : colorMirando);
    }

}