using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class movement : MonoBehaviour
{
    private NavMeshAgent agente;
    
    [Tooltip("Asigna aquí la capa (Layer) de tu suelo/terreno")]
    public LayerMask capaTerreno;

    void Start()
    {
        agente = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        // Detecta el clic izquierdo del mouse
        if (Input.GetMouseButtonDown(0))
        {
            // Lanza un rayo desde la cámara hasta donde está el mouse
            Ray rayo = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit impacto;

            // Si el rayo golpea algo que sea parte de la "capaTerreno"
            if (Physics.Raycast(rayo, out impacto, 100f, capaTerreno))
            {
                // Le dice al NavMeshAgent que vaya a ese punto
                agente.SetDestination(impacto.point);
            }
        }
    }
}