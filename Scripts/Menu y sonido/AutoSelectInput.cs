using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class AutoSelectInput : MonoBehaviour
{
    private TMP_InputField inputField;

    void Start()
    {
        inputField = GetComponent<TMP_InputField>();

        if (inputField != null)
        {
            inputField.Select();
            inputField.ActivateInputField();
        }
    }
}
