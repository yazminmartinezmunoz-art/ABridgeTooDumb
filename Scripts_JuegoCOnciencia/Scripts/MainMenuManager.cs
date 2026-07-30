using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
   
    [SerializeField] private string gameplaySceneName = "Gameplay_Prueba";

    public void PlayGame()
    {
        SceneManager.LoadScene(gameplaySceneName);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
       
        UnityEditor.EditorApplication.isPlaying = false;
#else
            // Cuando el juego está construido (Build)
            Application.Quit();
#endif
    }
}
