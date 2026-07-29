using UnityEngine;
using Photon.Pun;

/// Busca puntos cercanos y realiza
/// el snap automático.
public class SistemaSnap : MonoBehaviourPun
{
    public static SistemaSnap Instance;

    [Header("Configuración")]
    public float distanciaSnap = 0.15f;

    private readonly Collider[] bufferOverlap = new Collider[16];

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    //Consulta si existe un punto de unión cercano válido,
    //SIN ejecutar el snap. Pensado para feedback visual
    //continuo mientras se sostiene un objeto.
    public bool HayPuntoCercano(ObjetoConstruible objeto)
    {
        return BuscarMejorPunto(objeto, out _, out _);
    }

    //Busca el mejor punto de unión y, si existe, ejecuta el snap.
    public bool IntentarSnap(ObjetoConstruible objeto)
    {
        if (!BuscarMejorPunto(objeto, out PuntoUnion mejorLocal, out PuntoUnion mejorExterno))
            return false;

        RealizarSnap(objeto, mejorLocal, mejorExterno);
        return true;
    }

    //Lógica de búsqueda compartida entre IntentarSnap y HayPuntoCercano.
    private bool BuscarMejorPunto(ObjetoConstruible objeto, out PuntoUnion mejorPuntoLocal, out PuntoUnion mejorPuntoExterno)
    {
        mejorPuntoLocal = null;
        mejorPuntoExterno = null;

        if (objeto == null)
            return false;

        float mejorDistancia = distanciaSnap;

        foreach (PuntoUnion miPunto in objeto.puntosUnion)
        {
            int cantidad = Physics.OverlapSphereNonAlloc(miPunto.transform.position, distanciaSnap, bufferOverlap);

            for (int i = 0; i < cantidad; i++)
            {
                Collider col = bufferOverlap[i];

                PuntoUnion otroPunto = col.GetComponent<PuntoUnion>();

                if (otroPunto == null)
                    continue;

                if (otroPunto.dueño == objeto)
                    continue;

                // Ignorar objetos que ya pertenecen al mismo grupo.
                if (objeto.grupoActual != null && otroPunto.dueño.grupoActual != null && objeto.grupoActual == otroPunto.dueño.grupoActual)
                {
                    continue;
                }

                float distancia = Vector3.Distance(miPunto.transform.position, otroPunto.transform.position);

                if (distancia < mejorDistancia)
                {
                    mejorDistancia = distancia;
                    mejorPuntoLocal = miPunto;
                    mejorPuntoExterno = otroPunto;
                }
            }
        }

        return mejorPuntoLocal != null;
    }

    private void RealizarSnap(ObjetoConstruible objetoA, PuntoUnion puntoA, PuntoUnion puntoB)
    {
        Vector3 offset = puntoB.transform.position - puntoA.transform.position;

        objetoA.transform.position += offset;

        GrupoConstruccion grupoA = objetoA.grupoActual;
        GrupoConstruccion grupoB = puntoB.dueño.grupoActual;

        bool conectadoABase = puntoB.CompareTag("Base") || puntoB.dueño.CompareTag("Base");

        if (!conectadoABase)
        {
            if (grupoA == null && grupoB == null)
            {
                CrearGrupo(objetoA, puntoB.dueño);
            }
            else if (grupoA != null && grupoB == null)
            {
                grupoA.AgregarObjeto(puntoB.dueño);
            }
            else if (grupoA == null && grupoB != null)
            {
                grupoB.AgregarObjeto(objetoA);
            }
            else if (grupoA != grupoB)
            {
                grupoA.FusionarGrupo(grupoB);
            }
        }

        // Sonido de unión: no está dentro de una RPC ya
        // sincronizada, así que necesita el envío en red completo.
        SonidosConstruccion audioObjeto = objetoA.GetComponent<SonidosConstruccion>();

        if (audioObjeto != null)
        {
            audioObjeto.Reproducir(SonidosConstruccion.TipoSonido.Unir);
        }

        // Al soltarse, el outline vuelve a su color normal
        // (deja de sugerir "unión posible").
        objetoA.ActualizarColorOutline(false);

        PhotonView pv = objetoA.GetComponent<PhotonView>();

        if (pv != null)
        {
            pv.RPC("RPC_CongelarObjeto", RpcTarget.All);
        }

        if (objetoA.grupoActual != null)
        {
            foreach (ObjetoConstruible pieza in objetoA.grupoActual.objetos)
            {
                PhotonView pvPieza = pieza.GetComponent<PhotonView>();

                if (pvPieza != null)
                {
                    pvPieza.RPC("RPC_CongelarObjeto", RpcTarget.All);
                }
            }
        }
    }

    // El método de descongelar objeto está en el script AgarreObjeto
    private void CrearGrupo(ObjetoConstruible objetoA, ObjetoConstruible objetoB)
    {
        objetoA.photonView.RPC("RPC_CrearGrupo", RpcTarget.All, objetoB.photonView.ViewID);
    }

    /*
    public bool IntentarSnap(ObjetoConstruible objeto)
    {
        if (objeto == null)
            return false;

        PuntoUnion mejorPuntoLocal = null;
        PuntoUnion mejorPuntoExterno = null;
        float mejorDistancia = distanciaSnap;

        foreach (PuntoUnion miPunto in objeto.puntosUnion)
        {
            Collider[] cercanos = Physics.OverlapSphere(miPunto.transform.position, distanciaSnap);

            foreach (Collider col in cercanos)
            {
                PuntoUnion otroPunto = col.GetComponent<PuntoUnion>();

                if (otroPunto == null)
                    continue;

                if (otroPunto.dueño == objeto)
                    continue;

                // Ignorar objetos que ya pertenecen
                // al mismo grupo.
                if (objeto.grupoActual != null && otroPunto.dueño.grupoActual != null && objeto.grupoActual == otroPunto.dueño.grupoActual)
                {
                    continue;
                }

                float distancia = Vector3.Distance(miPunto.transform.position, otroPunto.transform.position);

                if (distancia < mejorDistancia)
                {
                    mejorDistancia = distancia;
                    mejorPuntoLocal = miPunto;
                    mejorPuntoExterno = otroPunto;
                }
            }
        }

        if (mejorPuntoLocal == null)
            return false;

        RealizarSnap(objeto, mejorPuntoLocal, mejorPuntoExterno);

        return true;
    }
    private void RealizarSnap(ObjetoConstruible objetoA, PuntoUnion puntoA, PuntoUnion puntoB)
    {
        Vector3 offset = puntoB.transform.position - puntoA.transform.position;

        objetoA.transform.position += offset;

        GrupoConstruccion grupoA = objetoA.grupoActual;

        GrupoConstruccion grupoB = puntoB.dueño.grupoActual;

        // Si el objeto se conecta a una Base
        bool conectadoABase = puntoB.CompareTag("Base") || puntoB.dueño.CompareTag("Base");

        if (!conectadoABase)
        {
            // Lógica normal de grupos

            if (grupoA == null &&
                grupoB == null)
            {
                CrearGrupo(objetoA, puntoB.dueño);
            }
            else if (grupoA != null && grupoB == null)
            {
                grupoA.AgregarObjeto(puntoB.dueño);
            }
            else if (grupoA == null && grupoB != null)
            {
                grupoB.AgregarObjeto(objetoA);
            }
            else if (grupoA != grupoB)
            {
                grupoA.FusionarGrupo(grupoB);
            }
        }

        //SONIDO UNION
        SonidosConstruccion audioObjeto = objetoA.GetComponent<SonidosConstruccion>();

        if (audioObjeto != null)
        {
            audioObjeto.Reproducir(SonidosConstruccion.TipoSonido.Unir);
        }

        // Congelar SIEMPRE
        PhotonView pv = objetoA.GetComponent<PhotonView>();

        if (pv != null)
        {
            pv.RPC("RPC_CongelarObjeto", RpcTarget.All);
        }

        // Si pertenece a grupo,
        // congelar también las piezas
        if (objetoA.grupoActual != null)
        {
            foreach (ObjetoConstruible pieza in objetoA.grupoActual.objetos)
            {
                PhotonView pvPieza = pieza.GetComponent<PhotonView>();

                if (pvPieza != null)
                {
                    pvPieza.RPC("RPC_CongelarObjeto", RpcTarget.All);
                }
            }
        }
    }

    //El metodo de descongelar objeto esta en el script AgarreObjeto
    private void CrearGrupo(ObjetoConstruible objetoA, ObjetoConstruible objetoB)
    {
        objetoA.photonView.RPC("RPC_CrearGrupo", RpcTarget.All, objetoB.photonView.ViewID);
    }
    */
}