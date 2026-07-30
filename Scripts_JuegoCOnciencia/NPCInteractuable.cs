using UnityEngine;

public class NPCInteractuable : MonoBehaviour
{
    public NodoDialogo dialogoInicial;
    private bool jugadorCerca = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) jugadorCerca = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) jugadorCerca = false;
    }

    // ACTUALIZADO: Pasamos la referencia del movimiento al Gestor
    void OnMouseDown()
    {
        if (jugadorCerca)
        {
            // Buscamos el script de movimiento pegado a este mismo NPC
            NPCMovimiento miMovimiento = GetComponent<NPCMovimiento>();

            if (miMovimiento != null)
            {
                miMovimiento.DetenerParaHablar(); // Lo frenamos
            }

            // Le enviamos el diálogo inicial Y el script de movimiento al Gestor
            GestorDialogos.instancia.IniciarDialogo(dialogoInicial, miMovimiento);
        }
        else
        {
            Debug.Log("Estás muy lejos para hablar con él.");
        }
    }
}