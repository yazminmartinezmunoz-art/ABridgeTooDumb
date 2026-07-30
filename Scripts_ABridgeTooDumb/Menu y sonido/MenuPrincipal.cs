using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuPrincipal : MonoBehaviour
{
    public TMP_InputField nombreInput;
    public Button botonContinuar;

    private void Start()
    {
        if (botonContinuar == null)
            botonContinuar = GameObject.Find("NombreDelBoton").GetComponent<Button>();

        botonContinuar.interactable = PhotonNetwork.InLobby;
    }

    private void Update()
    {
        if (botonContinuar != null)
            botonContinuar.interactable = PhotonNetwork.InLobby;

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            botonContinuar.onClick.Invoke();
        }
    }

    public void Continuar()
    {
        Debug.Log("Botón Continuar presionado");

        PhotonNetwork.NickName = nombreInput.text;

        Debug.Log("Nombre: " + PhotonNetwork.NickName);

        SceneManager.LoadScene("Lobby");
    }

    public void SalirJuego()
    {
        Debug.Log("Saliendo del juego...");

        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }


}