using UnityEngine;
using Photon.Pun;
using System.Collections;

public class Meteorito : MonoBehaviourPun
{
    [Header("Explosión")]
    public float radioExplosion = 8f;
    public float fuerzaExplosion = 2000f;

    [Header("Vida")]
    public float tiempoDeVida = 10f;

    [Header("Efectos")]
    public GameObject efectoImpacto;
    public GameObject efectoExplosion;

    private bool exploto = false;

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(AutoDestruir());
        }
    }

    IEnumerator AutoDestruir()
    {
        yield return new WaitForSeconds(tiempoDeVida);

        if (gameObject != null)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint contacto = collision.contacts[0];

        photonView.RPC(
            "RPC_MostrarImpacto",
            RpcTarget.All,
            contacto.point
        );

        if (!PhotonNetwork.IsMasterClient)
            return;

        if (exploto)
            return;

        if (collision.gameObject.CompareTag("Suelo"))
        {
            exploto = true;
            Explode();
        }
    }

    [PunRPC]
    void RPC_MostrarImpacto(Vector3 posicion)
    {
        if (efectoImpacto != null)
        {
            Instantiate(
                efectoImpacto,
                posicion,
                Quaternion.identity
            );
        }
    }

    [PunRPC]
    void RPC_MostrarExplosion(Vector3 posicion)
    {
        if (efectoExplosion != null)
        {
            Instantiate(
                efectoExplosion,
                posicion,
                Quaternion.identity
            );
        }
    }

    void Explode()
    {
        photonView.RPC(
            "RPC_MostrarExplosion",
            RpcTarget.All,
            transform.position
        );

        Collider[] objetos = Physics.OverlapSphere(
            transform.position,
            radioExplosion
        );

        foreach (Collider obj in objetos)
        {
            Rigidbody rb = obj.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.AddExplosionForce(
                    fuerzaExplosion,
                    transform.position,
                    radioExplosion,
                    2f,
                    ForceMode.Impulse
                );
            }
        }

        //Debug.Log("EXPLODE EJECUTADO");

        PhotonNetwork.Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(
            transform.position,
            radioExplosion
        );
    }
}