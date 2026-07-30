using UnityEngine;
using UnityEngine.AI;
using System.Collections; // Necesario para usar Corrutinas

public class GeneradorDeRopa : MonoBehaviour
{
    public GameObject prefabRopa;

    [Tooltip("Tiempo en segundos entre cada aparición de ropa")]
    public float tiempoEntreApariciones = 5f;

    [Tooltip("El radio máximo donde buscará un lugar para poner la ropa")]
    public float radioDeGeneracion = 20f;

    [Tooltip("Límite máximo de prendas en el mapa para evitar saturar la memoria")]
    public int limiteRopaEnMapa = 30;

    void Start()
    {
        // Iniciamos la rutina cíclica en lugar de generar todo de golpe
        StartCoroutine(RutinaGeneracion());
    }

    IEnumerator RutinaGeneracion()
    {
        // Bucle infinito que se ejecuta mientras el objeto exista en la escena
        while (true)
        {
            // Busca cuántas prendas hay actualmente (el prefab debe tener el Tag "Ropa")
            GameObject[] ropaActual = GameObject.FindGameObjectsWithTag("Ropa");

            // Solo genera más si no hemos superado el límite
            if (ropaActual.Length < limiteRopaEnMapa)
            {
                GenerarUnaPrenda();
            }

            // Pausa el script durante 5 segundos antes de repetir el ciclo
            yield return new WaitForSeconds(tiempoEntreApariciones);
        }
    }

    void GenerarUnaPrenda()
    {
        // 1. Crea un punto al azar en una esfera alrededor del generador
        Vector3 puntoAleatorio = transform.position + Random.insideUnitSphere * radioDeGeneracion;
        NavMeshHit hit;

        // 2. Busca el punto válido más cercano en el NavMesh dentro de un radio de 3 metros
        if (NavMesh.SamplePosition(puntoAleatorio, out hit, 3f, NavMesh.AllAreas))
        {
            // 3. Instancia la ropa en ese punto válido del suelo
            Instantiate(prefabRopa, hit.position, Quaternion.identity);
        }
    }
}