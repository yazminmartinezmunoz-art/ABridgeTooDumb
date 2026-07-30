using Photon.Pun;
using UnityEngine;

/// Controla el láser visual
public class LaserJugador : MonoBehaviourPun
{
    [Header("Referencias")]
    public LineRenderer line;
    public Transform puntoInicio;

    [Header("Colores")]
    public Color[] colores;
    private Transform objetivoActual;
    private bool laserActivo;
    public Material materialInstancia;
    private PhotonView pv;

    private void Awake()
    {
        pv = photonView;
        line.enabled = false;
        line.positionCount = 2;
        line.startWidth = 0.05f;
        line.endWidth = 0.05f;

        // Buscar automáticamente el punto
        // dentro DEL MISMO PERSONAJE
        if (puntoInicio == null)
        {
            Transform punto = transform.root.Find("PuntoLaser");

            if (punto != null)
            {
                puntoInicio = punto;
            }
        }

        // Color por jugador
        if (colores.Length > 0)
        {
            int indice = (pv.Owner.ActorNumber - 1) % colores.Length;
            Color color = colores[indice];
            line.startColor = color;
            line.endColor = color;
        }
    }

    private void Start()
    {
        ConfigurarColor();
    }

    private void Update()
    {
        if (pv == null) return;

        if (!laserActivo) return;

        if (objetivoActual == null)
        {
            if (pv.IsMine)
            {
                DesactivarLaser();
            }

            else
            {
                line.enabled = false;
            }

            return;
        }

        if (puntoInicio == null) return;

        line.SetPosition(0, puntoInicio.position);
        line.SetPosition(1, objetivoActual.position);
    }

    private void ConfigurarColor()
    {
        if (colores.Length == 0)
            return;

        if (pv == null)
            return;

        int indice = (pv.Owner.ActorNumber - 1) % colores.Length;
        Color color = colores[indice];
        line.startColor = color;
        line.endColor = color;

        // MUY IMPORTANTE:
        // URP usa el material
        materialInstancia.color = color;
    }

    public void ActivarLaser(Transform objetivo)
    {
        if (pv == null || !pv.IsMine || objetivo == null) return;

        PhotonView objetivoPV = objetivo.GetComponent<PhotonView>();

        if (objetivoPV != null)
        {
            pv.RPC("RPC_ActivarLaser", RpcTarget.All, objetivoPV.ViewID);
        }
    }

    public void DesactivarLaser()
    {
        if (pv == null || !pv.IsMine) return;

        pv.RPC("RPC_DesactivarLaser", RpcTarget.All);
    }

    [PunRPC]
    public void RPC_ActivarLaser(int viewIDObjetivo)
    {
        PhotonView objetivoPV = PhotonView.Find(viewIDObjetivo);

        if (objetivoPV != null)
        {
            objetivoActual = objetivoPV.transform;
            laserActivo = true;
            line.enabled = true;
        }
    }

    [PunRPC]
    public void RPC_DesactivarLaser()
    {
        objetivoActual = null;
        laserActivo = false;
        line.enabled = false;
    }
}