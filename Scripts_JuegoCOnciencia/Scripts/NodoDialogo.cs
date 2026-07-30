using UnityEngine;

// Esto agrupa el texto del jugador y a qué diálogo nos lleva
[System.Serializable]
public struct OpcionRespuesta
{
    public string textoRespuesta;
    public NodoDialogo siguienteNodo; // Si lo dejas vacío, el diálogo se cierra
}

[CreateAssetMenu(fileName = "NuevoDialogo", menuName = "Juego/Nodo de Diálogo")]
public class NodoDialogo : ScriptableObject
{
    [TextArea(3, 10)] // Hace que la caja de texto en el editor sea más grande
    public string textoDelNPC;

    public OpcionRespuesta[] respuestasJugador;
}