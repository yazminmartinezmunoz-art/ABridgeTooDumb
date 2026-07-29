using UnityEngine;

/// Punto donde puede conectarse
/// otro objeto construible.
public class PuntoUnion : MonoBehaviour
{
    [HideInInspector]
    public ObjetoConstruible dueño;
}