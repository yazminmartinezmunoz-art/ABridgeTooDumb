using UnityEngine;

public class Efecto : MonoBehaviour
{
    [Header("Unity Setup")]
    public float time;

    public bool shakeCamara;
    [Range(0f, 1f)]
    public float duration;
    [Range(0f, 1f)]
    public float magnitude;

    public void Start()
    {
        if (shakeCamara)
            StartCoroutine(Object.FindFirstObjectByType<CamaraShake>().Shake(duration, magnitude));


        Destroy(gameObject, time);
    }

}
