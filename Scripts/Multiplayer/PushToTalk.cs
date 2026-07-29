/*using UnityEngine;
using Photon.Pun;
using System.Collections;

public class PushToTalk : MonoBehaviour
{
    [Header("Configuración de Teclas")]
    [SerializeField] private KeyCode talkKey = KeyCode.V;

    private Recorder voiceRecorder;

    void Start()
    {
        // Busca el componente Recorder en el mismo GameObject o en la escena
        voiceRecorder = FindObjectOfType<Recorder>();

        if (voiceRecorder != null)
        {
            // Apaga el micrófono al iniciar el juego
            voiceRecorder.TransmitEnabled = false;
        }
        else
        {
            Debug.LogError("No se encontró el componente Recorder en la escena.");
        }
    }

    void Update()
    {
        if (voiceRecorder == null) return;

        // Si presionas la tecla, empieza a transmitir
        if (Input.GetKeyDown(talkKey))
        {
            voiceRecorder.TransmitEnabled = true;
            Debug.Log("Micrófono ABIERTO (Transmitiendo...)");
        }

        // Si sueltas la tecla, deja de transmitir
        if (Input.GetKeyUp(talkKey))
        {
            voiceRecorder.TransmitEnabled = false;
            Debug.Log("Micrófono CERRADO");
        }
    }
}
*/