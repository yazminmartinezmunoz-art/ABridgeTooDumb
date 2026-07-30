using UnityEngine;

public class ZonaDeposito : MonoBehaviour
{
    [Tooltip("Arrastra aquí el archivo Scriptable Object 'InventarioJugador'")]
    public DatosInventario inventario;

    [Tooltip("Arrastra aquí el Prefab del bolso que quieres que aparezca")]
    public GameObject prefabBolso;

    [Tooltip("El punto exacto donde aparecerá el bolso")]
    public Transform puntoDeAparicion;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Revisa si el jugador tiene monedas para depositar
            if (inventario.monedasRecolectadas > 10)
            {
                Debug.Log("Depositando 10 poleras.");
                
                // Instancia (crea) el bolso en la posición indicada
                Instantiate(prefabBolso, puntoDeAparicion.position, puntoDeAparicion.rotation);

                // Vacía el inventario
                inventario.monedasRecolectadas = (inventario.monedasRecolectadas - 10);
            }
            else
            {
                Debug.Log("No tienes monedas para depositar.");
            }
        }
    }
}