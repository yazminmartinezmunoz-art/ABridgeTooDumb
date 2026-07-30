using UnityEngine;

/// Aplica daño al colisionar con fuerza.
[RequireComponent(typeof(ObjetoConstruible))]
public class DañoPorImpacto : MonoBehaviour
{
   
    private ObjetoConstruible objeto;

    [Header("Daño")]

    public float fuerzaMinimaDaño = 15f;

    private void Awake()
    {
        objeto = GetComponent<ObjetoConstruible>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        float fuerzaImpacto = collision.relativeVelocity.magnitude;

        if (fuerzaImpacto >= fuerzaMinimaDaño)
        {
            objeto.RecibirDaño(1);
        }
    }
}