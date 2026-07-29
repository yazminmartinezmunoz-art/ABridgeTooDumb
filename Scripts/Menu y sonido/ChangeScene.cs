using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public void _ChangeScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }
}
