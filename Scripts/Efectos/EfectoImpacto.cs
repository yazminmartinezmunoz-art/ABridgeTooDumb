using UnityEngine;
using Photon.Pun;

public class EfectoImpacto : MonoBehaviour
{
    public ParticleSystem impactParticle;
    public float minimumForce;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        int maxPoints = Mathf.Min(collision.contacts.Length, 2);

        if (collision.impulse.magnitude > minimumForce)
        {
            for (int i = 0; i < maxPoints; i++)
            {
                PhotonNetwork.Instantiate(impactParticle.name, collision.contacts[i].point, Quaternion.identity);
            }
            
        }
    }
}
