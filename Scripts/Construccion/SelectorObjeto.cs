using UnityEngine;

/// Detecta qué objeto está mirando el jugador.
public class SelectorObjetos : MonoBehaviour
{
    [Header("Referencias")]
    public Camera camara;
    public UIObjeto uiObjeto;

    [Header("Configuración")]
    public float distanciaMaxima = 8f;
    private ObjetoConstruible objetoMirado;

    private void Awake()
    {
        uiObjeto = GameObject.Find("Canvas").GetComponent<UIObjeto>();
    }

    private void Update()
    {
        DetectarObjeto();
        MostrarInformacion();
    }

    private void DetectarObjeto()
    {

        Vector3 centroPantalla =
            new Vector3(
                Screen.width / 2,
                Screen.height / 2);

        Ray ray =
            camara.ScreenPointToRay(
                centroPantalla);

        Debug.DrawRay(
            ray.origin,
            ray.direction * distanciaMaxima,
            Color.red);


        if (
            Physics.Raycast(
                ray,
                out RaycastHit hit,
                distanciaMaxima))
        {
            objetoMirado =
                hit.collider.GetComponent<ObjetoConstruible>();
        }
        else
        {
            objetoMirado = null;
        }
        

    }

    private void MostrarInformacion()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (objetoMirado != null)
            {
                uiObjeto.Mostrar(objetoMirado);
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            uiObjeto.Ocultar();
        }
    }
}