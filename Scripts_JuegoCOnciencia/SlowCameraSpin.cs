using UnityEngine;

public class SlowCameraSpin : MonoBehaviour
{
    [Tooltip("Degrees per second")]
    public float spinSpeed = 8f;          

    [Tooltip("Axis to spin around (usually Y)")]
    public Vector3 spinAxis = Vector3.up; // (0,1,0)

    void Update()
    {
        transform.Rotate(spinAxis, spinSpeed * Time.deltaTime, Space.Self);
    }
}