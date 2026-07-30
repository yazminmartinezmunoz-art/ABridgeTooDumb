using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;

/// Permite mover grupos completos.
public class AgarreGrupo : MonoBehaviour
{
    [Header("Referencias")]

    public Camera camara;

    [Header("Configuración")]

    public float distanciaRaycast = 4f;

    public float distanciaAgarre = 0.1f;

    public float alturaAgarre = 0.03f;

    public float velocidadMovimiento = 4f;

    private GrupoConstruccion grupoActual;

    private Dictionary<ObjetoConstruible, Vector3>
        offsets =
    new Dictionary<ObjetoConstruible, Vector3>();


    [Header("Distancia")]

    public float distanciaMinima = 0.1f;

    public float distanciaMaxima = 4f;

    public float velocidadScroll = 0.2f;


    [Header("UI")]

    public UIObjeto uiObjeto;

    [Header("Sistemas")]

    public SistemaSnap sistemaSnap;

    private ObjetoConstruible objetoUI;

    private LaserJugador laser;
    private bool estaAgarrandoGrupo;
    private GameObject puntoLaserGrupo;

    private SonidoLaserJugador sonidosJugador;

    private void Awake()
    {
        sistemaSnap =
            GameObject.Find("Sistema snap")
            .GetComponent<SistemaSnap>();

        uiObjeto =
            GameObject.Find("Canvas")
            .GetComponent<UIObjeto>();

        laser = GetComponentInChildren<LaserJugador>();

        puntoLaserGrupo = new GameObject("PuntoLaserGrupo");

        sonidosJugador = GetComponent<SonidoLaserJugador>();
    }

    private void Update()
    {
        // Si el grupo desapareció
        if (grupoActual == null &&
            estaAgarrandoGrupo)
        {
            if (sonidosJugador != null)
            {
                sonidosJugador.DetenerAgarre();
            }

            laser.photonView.RPC(
                "RPC_DesactivarLaser",
                RpcTarget.All);

            estaAgarrandoGrupo = false;
        }

        DetectarGrupo();

        GestionarScroll();
    }
    private void FixedUpdate()
    {
        MoverGrupo();
    }

    private void DetectarGrupo()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Vector3 centroPantalla =
                new Vector3(
                    Screen.width / 2,
                    Screen.height / 2);

            Ray ray =
                camara.ScreenPointToRay(
                    centroPantalla);

            if (Physics.Raycast(
                ray,
                out RaycastHit hit,
                distanciaRaycast))
            {
                ObjetoConstruible objeto =
                    hit.collider.GetComponent<ObjetoConstruible>();

                if (objeto == null)
                    return;

                grupoActual =
                    objeto.grupoActual;

                objetoUI = objeto;

                uiObjeto.Mostrar(objetoUI);

                // Primero comprobamos que exista.
                if (grupoActual == null)
                    return;

                // Si el grupo está conectado a una Base no puede levantarse.
                if (grupoActual.EstaAnclado())
                {
                    Debug.Log(
                        "Este grupo está anclado a una Base");

                    grupoActual = null;

                    return;
                }

                foreach (ObjetoConstruible pieza
                         in grupoActual.objetos)
                {
                    // Solo agregar si el jugador
                    // aún no estaba registrado
                    if (!pieza.jugadoresAgarrando.Contains(
                        PhotonNetwork.LocalPlayer.ActorNumber))
                    {
                        pieza.photonView.RPC(
                            "RPC_AgregarJugadorAgarrando",
                            RpcTarget.All,
                            PhotonNetwork.LocalPlayer
                                .ActorNumber);
                    }
                }

                offsets.Clear();

                // Solo descongelamos si existe fuerza suficiente
                if (grupoActual.TieneFuerzaSuficiente())
                {
                    foreach (ObjetoConstruible pieza
                             in grupoActual.objetos)
                    {
                        PhotonView pv =
                            pieza.GetComponent<PhotonView>();

                        if (pv != null)
                        {
                            pv.RPC(
                                "RPC_DescongelarObjeto",
                                RpcTarget.All);
                        }
                    }
                }

                Vector3 centroGrupo = CalcularCentro();

                // ACTIVAR LÁSER
                puntoLaserGrupo.transform.position =
                    CalcularCentro();

                PhotonView puntoPV =
                    puntoLaserGrupo
                    .GetComponent<PhotonView>();

                laser.photonView.RPC(
                    "RPC_ActivarLaser",
                    RpcTarget.All,
                    grupoActual.objetos[0]
                    .photonView.ViewID);

                estaAgarrandoGrupo = true;

                //SONIDO AGARRE
                if (sonidosJugador != null)
                {
                    sonidosJugador.IniciarAgarre();
                }

                foreach (ObjetoConstruible pieza
                         in grupoActual.objetos)
                {
                    offsets.Add(
                        pieza,
                        pieza.transform.position -
                        centroGrupo);
                }
            }
        }

        if (Input.GetMouseButtonUp(1))
        {
            if (grupoActual == null)
                return;

            // Guardamos una copia del grupo actual
            List<ObjetoConstruible> copia =
                new List<ObjetoConstruible>(
                    grupoActual.objetos);

            bool snapRealizado =
                IntentarSnapGrupo();

            // tu código de congelar/descongelar...

            foreach (ObjetoConstruible pieza
                     in copia)
            {
                if (pieza == null)
                    continue;

                if (pieza.jugadoresAgarrando.Contains(
                    PhotonNetwork.LocalPlayer.ActorNumber))
                {
                    pieza.photonView.RPC(
                        "RPC_QuitarJugadorAgarrando",
                        RpcTarget.All,
                        PhotonNetwork.LocalPlayer
                            .ActorNumber);
                }
            }

            uiObjeto.Ocultar();

            objetoUI = null;

            if (sonidosJugador != null)
            {
                sonidosJugador.DetenerAgarre();
            }

            grupoActual = null;

            // APAGAR LÁSER
            laser.photonView.RPC(
                "RPC_DesactivarLaser",
                RpcTarget.All);

            estaAgarrandoGrupo = false;
        }
    }

    private void MoverGrupo()
    {
        if (grupoActual == null)
            return;

        if (!grupoActual
            .TieneFuerzaSuficiente())
        {
            return;
        }
        if (puntoLaserGrupo != null)
        {
            puntoLaserGrupo.transform.position =
                CalcularCentro();
        }

        Vector3 objetivo =
            camara.transform.position +
            camara.transform.forward *
            distanciaAgarre;

        objetivo +=
            Vector3.up *
            alturaAgarre;

        foreach (ObjetoConstruible objeto
            in grupoActual.objetos)
        {
            Rigidbody rb =
                objeto.GetComponent<Rigidbody>();

            if (rb == null)
                continue;

            Vector3 objetivoPieza =
                objetivo +
                offsets[objeto];

            Vector3 direccion =
                objetivoPieza -
                rb.position;

            rb.linearVelocity =
                direccion *
                velocidadMovimiento;
        }
    }

    private Vector3 CalcularCentro()
    {
        Vector3 centro = Vector3.zero;

        foreach (ObjetoConstruible pieza
                 in grupoActual.objetos)
        {
            centro += pieza.transform.position;
        }

        return centro /
               grupoActual.objetos.Count;
    }
    /// Revisa todos los objetos del grupo para buscar posibles conexiones.
    private bool IntentarSnapGrupo()
    {
        if (grupoActual == null)
        {
            Debug.Log("grupoActual es NULL");
            return false;
        }

        Debug.Log("SistemaSnap = " + sistemaSnap);

        List<ObjetoConstruible> copia =
            new List<ObjetoConstruible>(
                grupoActual.objetos);

        foreach (ObjetoConstruible objeto
                 in copia)
        {
            Debug.Log("Objeto = " + objeto);

            if (objeto == null)
            {
                Debug.Log("Objeto nulo encontrado");
                continue;
            }

            Debug.Log(
                "Intentando snap con: " +
                objeto.name);

            bool snapRealizado =
                sistemaSnap.IntentarSnap(
                    objeto);

            if (snapRealizado)
            {
                return true;
            }
        }

        return false;
    }

    public bool EstaAgarrandoGrupo()
    {
        return grupoActual != null;
    }



    /// Permite acercar o alejar el grupo usando la rueda.
    private void GestionarScroll()
    {
        if (grupoActual == null)
            return;

        float scroll =
            Input.GetAxis("Mouse ScrollWheel");

        distanciaAgarre +=
            scroll * velocidadScroll;

        distanciaAgarre =
            Mathf.Clamp(
                distanciaAgarre,
                distanciaMinima,
                distanciaMaxima);
    }

}