using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public Camera camara1;
    public Camera camara2;

    void Start()
    {
        camara1.enabled = true;
        camara2.enabled = false;
    }

    public void CambiarCamara()
    {
        camara1.enabled = false;
        camara2.enabled = true;
    }
}