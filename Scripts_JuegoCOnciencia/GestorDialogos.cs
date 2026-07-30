using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GestorDialogos : MonoBehaviour
{
    public static GestorDialogos instancia;

    public GameObject panelDialogo;
    public TextMeshProUGUI textoNPCUI;
    public Transform contenedorBotones;
    public GameObject prefabBotonRespuesta;

    // NUEVO: Variable para recordar con qué NPC estamos hablando
    private NPCMovimiento npcActual;

    void Awake()
    {
        instancia = this;
        panelDialogo.SetActive(false);
    }

    // ACTUALIZADO: Ahora pide el nodo y el script de movimiento del NPC
    public void IniciarDialogo(NodoDialogo nodoInicial, NPCMovimiento npc)
    {
        npcActual = npc; // Guardamos el NPC en la memoria
        panelDialogo.SetActive(true);
        MostrarNodo(nodoInicial);
    }

    void MostrarNodo(NodoDialogo nodo)
    {
        // Si el nodo está vacío, cerramos el diálogo
        if (nodo == null)
        {
            panelDialogo.SetActive(false);

            // NUEVO: Le decimos al NPC que vuelva a caminar y limpiamos la variable
            if (npcActual != null)
            {
                npcActual.ReanudarPaseo();
                npcActual = null;
            }
            return;
        }

        textoNPCUI.text = nodo.textoDelNPC;

        foreach (Transform hijo in contenedorBotones)
        {
            Destroy(hijo.gameObject);
        }

        foreach (OpcionRespuesta opcion in nodo.respuestasJugador)
        {
            GameObject nuevoBoton = Instantiate(prefabBotonRespuesta, contenedorBotones);
            nuevoBoton.GetComponentInChildren<TextMeshProUGUI>().text = opcion.textoRespuesta;

            nuevoBoton.GetComponent<Button>().onClick.AddListener(() => MostrarNodo(opcion.siguienteNodo));
        }
    }
}