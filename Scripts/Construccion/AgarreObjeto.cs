using Photon.Pun;
using System;
using UnityEngine;

/// Sistema de agarre individual.
/// El objeto sigue una posición calculada
/// desde la cámara y no un punto fijo.
public class AgarreObjeto : MonoBehaviourPun
{
    [Header("Referencias")]
    public Camera camara;
    public UIObjeto uiObjeto;

    [Header("Detección")]
    public float distanciaRaycast = 4f;
    public float tiempoMantener = 0.2f;
    public LayerMask layer;

    [Header("Agarre")]
    public float distanciaAgarre = 0.4f;
    public float alturaAgarre = 0.03f;
    public float velocidadSeguimiento = 4f;
    public float distanciaMinima = 0.2f;
    public float distanciaMaxima = 8f;
    public float velocidadScroll = 0.2f;
    private ObjetoConstruible objetoMirado;
    private ObjetoConstruible objetoAgarrado;
    private Rigidbody rbAgarrado;
    private float tiempoClick;
    private bool estaAgarrando;
    private AgarreGrupo agarreGrupo;
    private LaserJugador laser;

    //public SistemaUnion sistemaUnion;

    [Header("Snap - Feedback visual")]
    public float intervaloChequeoSnap = 0.1f;
    private float temporizadorSnap;
    private bool ultimoEstadoUnionPosible;

    private GrupoConstruccion grupoAgarrado;
    private GrupoConstruccion grupoActual;
    private bool agarrandoGrupo;
    private float tiempoSinObjeto = 0f;

    [Header("UI")]
    public float tiempoOcultarUI = 0.3f;
    public SistemaSnap sistemaSnap;
    private SonidoLaserJugador sonidosJugador;

    private int idJugadorLocal
    {
        get
        {
            return PhotonNetwork.LocalPlayer.ActorNumber;
        }
    }

    private void Awake()
    {
        sistemaSnap = SistemaSnap.Instance;
        uiObjeto = GameObject.Find("Canvas").GetComponent<UIObjeto>();
        agarreGrupo = GetComponent<AgarreGrupo>();
        laser = GetComponentInChildren<LaserJugador>();
        sonidosJugador = GetComponent<SonidoLaserJugador>();
    }

    private void Update()
    {
        if (!photonView.IsMine) return;

        if (objetoAgarrado == null && estaAgarrando)
        {
            estaAgarrando = false;
            rbAgarrado = null;

            // Detener sonido de agarre
            if (sonidosJugador != null)
            {
                sonidosJugador.DetenerAgarre();
            }

            if (laser != null)  
            laser.photonView.RPC("RPC_DesactivarLaser", RpcTarget.All);
        }

        DetectarObjeto();
        GestionarClickIzquierdo();
        GestionarDistancia();
        ActualizarFeedbackSnap();
    }

    private void FixedUpdate()
    {
        MoverObjeto();
    }

    private void ActualizarFeedbackSnap()
    {
        if (objetoAgarrado == null)
            return;

        temporizadorSnap += Time.deltaTime;

        if (temporizadorSnap < intervaloChequeoSnap)
            return;

        temporizadorSnap = 0f;

        bool unionPosible = sistemaSnap.HayPuntoCercano(objetoAgarrado);

        // Solo actualizar el material si cambió, para no
        // llamar SetColor todos los frames sin necesidad.
        if (unionPosible != ultimoEstadoUnionPosible)
        {
            ultimoEstadoUnionPosible = unionPosible;
            objetoAgarrado.ActualizarColorOutline(unionPosible);
        }
    }

    /// Detecta el objeto bajo la mira.
    private void DetectarObjeto()
    {
        Vector3 centroPantalla = new Vector3(Screen.width / 2, Screen.height / 2);

        Ray ray = camara.ScreenPointToRay(centroPantalla);

        if (Physics.Raycast(ray, out RaycastHit hit, distanciaRaycast, layer, QueryTriggerInteraction.Ignore))
        {
            ObjetoConstruible nuevoObjeto = hit.collider.GetComponent<ObjetoConstruible>();

            // Si estamos apuntando un objeto distinto
            if (nuevoObjeto != objetoMirado)
            {
                if (objetoMirado != null && objetoMirado != objetoAgarrado)
                {              
                    objetoMirado.ToggleOutline(false);
                }

                objetoMirado = nuevoObjeto;
                tiempoSinObjeto = 0f;

                if (objetoMirado != null)
                {
                    if (objetoMirado != objetoAgarrado)
                    {
                        objetoMirado.ToggleOutline(true);
                    }

                    uiObjeto.Mostrar(objetoMirado);
                }
            }
        }
        else
        {
            
            if (objetoMirado != null)
            {
                if (objetoMirado != objetoAgarrado)
                {
                    objetoMirado.ToggleOutline(false);
                }

                tiempoSinObjeto += Time.deltaTime;

                if (tiempoSinObjeto >= tiempoOcultarUI)
                {
                    if (estaAgarrando || (agarreGrupo != null && agarreGrupo.EstaAgarrandoGrupo()))
                    {
                        tiempoSinObjeto = 0f;
                        return;
                    }

                    objetoMirado = null;
                    //uiObjeto.Ocultar();
                }
            }
        }
    }

    /// Maneja click corto y mantenido.
    private void GestionarClickIzquierdo()
    {
        if (Input.GetMouseButtonDown(0))
        {
            tiempoClick = 0f;
            estaAgarrando = false;
            distanciaAgarre = 0.65f;
        }

        if (Input.GetMouseButton(0))
        {
            tiempoClick += Time.deltaTime;

            if (!estaAgarrando && tiempoClick >= tiempoMantener)
            {
                ComenzarAgarre();
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (estaAgarrando)
            {
                SoltarObjeto();
            }
        }
    }

    //Inicia el agarre.
    private void ComenzarAgarre()
    {
        if (!photonView.IsMine) return;

        if (objetoMirado == null)
            return;

        // Si pertenece a un grupo,
        // el agarre debe hacerse con click derecho
        if (objetoMirado.grupoActual != null)
        {
            GrupoConstruccion grupo = objetoMirado.grupoActual;
            grupo.RemoverObjeto(objetoMirado);
        }

        Rigidbody rb = objetoMirado.GetComponent<Rigidbody>();

        if (rb == null) return;

        PhotonView targetPV = rb.gameObject.GetComponent<PhotonView>();

        if (!targetPV.IsMine)
        {
            targetPV.TransferOwnership(PhotonNetwork.LocalPlayer);
        }

        rb.WakeUp();

        objetoMirado.photonView.RPC("RPC_AgregarJugadorAgarrando", RpcTarget.All, idJugadorLocal);

        objetoAgarrado = objetoMirado;
        rbAgarrado = rb;
        objetoAgarrado.estaSiendoAgarrado = true;
        estaAgarrando = true;

        //SONIDO
        if (sonidosJugador != null)
        {
            sonidosJugador.IniciarAgarre();
        }

        targetPV.RPC("RPC_ObjetoAgarrado", RpcTarget.All);

        // ACTIVAR LÁSER

        if (laser != null)
        {
            // Forzamos el RPC a través del PhotonView del LÁSER del jugador local,
            // garantizando que todos los clientes sepan que TU láser ahora apunta a ese objeto.
            //laser.photonView.RPC("RPC_ActivarLaser", RpcTarget.All, targetPV.ViewID);
            laser.ActivarLaser(objetoAgarrado.transform);
        }
    }



    //Permite acercar o alejar el objeto.
    //Similar a REPO.
    private void GestionarDistancia()
    {
        if (objetoAgarrado == null)
            return;

        float scroll = Input.mouseScrollDelta.y;

        distanciaAgarre += scroll * velocidadScroll;

        distanciaAgarre = Mathf.Clamp(distanciaAgarre, distanciaMinima, distanciaMaxima);
    }

    //Mueve el objeto hacia una posición
    //calculada desde la cámara.
    private void MoverObjeto()
    {
        if (!photonView.IsMine)
            return;

        if (objetoAgarrado == null)
            return;

        if (rbAgarrado == null)
            return;

        if (!objetoAgarrado.TieneFuerzaSuficiente())
        {
            rbAgarrado.linearVelocity = Vector3.zero;
            return;
        }

        // Posición objetivo frente a la cámara
        Vector3 posicionObjetivo = camara.transform.position + camara.transform.forward * distanciaAgarre;

        // Levantar un poco el objeto
        posicionObjetivo += Vector3.up * alturaAgarre;
        Vector3 direccion = posicionObjetivo - rbAgarrado.position;
        rbAgarrado.linearVelocity = direccion * velocidadSeguimiento;

    }


    //Suelta el objeto.
    private void SoltarObjeto()
    {
        // Si el objeto ya fue destruido
        if (objetoAgarrado == null)
        {
            rbAgarrado = null;
            estaAgarrando = false;

            // Desactivar laser temporalmente
            laser.photonView.RPC("RPC_DesactivarLaser", RpcTarget.All);

            return;
        }

        objetoAgarrado.estaSiendoAgarrado = false;
        objetoAgarrado.photonView.RPC("RPC_QuitarJugadorAgarrando", RpcTarget.All, idJugadorLocal);

        if (objetoAgarrado == null)
            return;

        objetoAgarrado.photonView.RPC("RPC_DescongelarObjeto", RpcTarget.All);
        sistemaSnap.IntentarSnap(objetoAgarrado);

        objetoAgarrado.ToggleOutline(false);

        if (sonidosJugador != null)
        {
            sonidosJugador.DetenerAgarre();
        }

        ultimoEstadoUnionPosible = false;
        objetoAgarrado = null;
        rbAgarrado = null;
        estaAgarrando = false;

        objetoMirado = null;
        tiempoSinObjeto = 0f;

        // Desactivar laser temporalmente
        laser.photonView.RPC("RPC_DesactivarLaser", RpcTarget.All);
    }
}