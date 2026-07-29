using UnityEngine;
using UnityEngine.UI;

public class ControlCursor : MonoBehaviour
{
    // Método para ocultar y desbloquear el cursor (Desactivar)
    public void DesactivarMouse()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Método para mostrar y desbloquear el cursor (Activar)
    public void ActivarMouse()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}