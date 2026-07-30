using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class NPCMovimiento : MonoBehaviour
{
    private NavMeshAgent agente;

    [Tooltip("Distancia máxima a la que buscará un nuevo punto para caminar")]
    public float radioPaseo = 10f;

    [Tooltip("Tiempo que se queda quieto antes de buscar otro destino")]
    public float tiempoEspera = 3f;

    private bool estaHablando = false;

    void Start()
    {
        agente = GetComponent<NavMeshAgent>();
        StartCoroutine(RutinaPaseo());
    }

    IEnumerator RutinaPaseo()
    {
        while (true)
        {
            // Si no está hablando y ya llegó a su destino (o está muy cerca)
            if (!estaHablando && !agente.pathPending && agente.remainingDistance < 0.5f)
            {
                // Espera un momento quieto
                yield return new WaitForSeconds(tiempoEspera);

                // Busca un nuevo punto aleatorio
                Vector3 puntoAleatorio = transform.position + Random.insideUnitSphere * radioPaseo;
                NavMeshHit hit;

                if (NavMesh.SamplePosition(puntoAleatorio, out hit, radioPaseo, NavMesh.AllAreas))
                {
                    agente.SetDestination(hit.position);
                }
            }

            yield return null; // Espera al siguiente frame
        }
    }

    // Puedes llamar a este método desde tu script NPCInteractuable para que se detenga al hablarle
    public void DetenerParaHablar()
    {
        estaHablando = true;
        agente.isStopped = true; // Frena al agente inmediatamente
    }

    // Llama a este método cuando se cierre el panel de diálogo
    public void ReanudarPaseo()
    {
        estaHablando = false;
        agente.isStopped = false;
    }
}