using UnityEngine;

[CreateAssetMenu(fileName = "NuevoInventario", menuName = "Juego/Datos de Inventario")]
public class DatosInventario : ScriptableObject
{
    public int monedasRecolectadas = 0;

    // Opcional: Un método para reiniciar el inventario al empezar el juego
    public void Reiniciar()
    {
        monedasRecolectadas = 0;
    }
}