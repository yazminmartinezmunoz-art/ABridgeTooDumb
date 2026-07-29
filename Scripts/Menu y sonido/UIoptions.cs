using UnityEngine;

public class UIoptions : MonoBehaviour
{
    public GameObject optionsPanel;
    bool paused;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!paused)
            {
                optionsPanel.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                paused = true;
            }

            else if (paused)
            {
                optionsPanel.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                paused = false;
            }
        }
    }
}
