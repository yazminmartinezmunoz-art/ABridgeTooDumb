using UnityEngine;

public class poleras : MonoBehaviour
{
   [Tooltip("Arrastra aquí el archivo Scriptable Object 'InventarioJugador'")]
    public DatosInventario inventario;
    
    public int valorMoneda = 1;

    void OnTriggerEnter(Collider other)
    {
        // Verifica si quien tocó la moneda tiene la etiqueta "Player"
        if (other.CompareTag("Player"))
        {
            // Añade la moneda al Scriptable Object
            inventario.monedasRecolectadas += valorMoneda;
            Debug.Log("poleras actuales: " + inventario.monedasRecolectadas);
            
            // Destruye la moneda de la escena
            Destroy(gameObject);
        }
    }
}
