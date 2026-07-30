using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PasarDia : MonoBehaviour
{
    [Header("Configuración del Día")]
    [Tooltip("Nombre de la escena que se va a recargar")]
    public string sceneName = "GameScene";

    [Tooltip("Tiempo de espera antes de recargar la escena (efecto de fade/sueño)")]
    public float delayBeforeReload = 1.5f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(AdvanceDayAndReload());
        }
    }

    private IEnumerator AdvanceDayAndReload()
    {
        // Guardar datos del día actual antes de avanzar
        SaveCurrentDayData();

        // Avanzar al siguiente día
        int currentDay = GetCurrentDay();
        int nextDay = currentDay + 1;
        SetCurrentDay(nextDay);

        Debug.Log($"Día {currentDay} completado. Durmiendo... Pasando al Día {nextDay}");

        // Pequeño delay para dar sensación de transición
        yield return new WaitForSeconds(delayBeforeReload);

        // Recargar la escena
        SceneManager.LoadScene(sceneName);
    }

    // ====================== SISTEMA DE GUARDADO ======================

    private const string DAY_KEY = "CurrentDay";

    private int GetCurrentDay()
    {
        return PlayerPrefs.GetInt(DAY_KEY, 1); // Día 1 por defecto
    }

    private void SetCurrentDay(int day)
    {
        PlayerPrefs.SetInt(DAY_KEY, day);
        PlayerPrefs.Save();
    }

    private void SaveCurrentDayData()
    {
        // === AQUÍ GUARDAS TODO LO QUE QUIERAS QUE PERSISTA ===

       

        PlayerPrefs.Save();
    }

    // Método público para cargar datos (llámalo desde tu GameManager o Player al iniciar la escena)
    public static void LoadDayData()
    {
        // Aquí cargas los datos guardados cuando la escena inicia
       
    }
}
