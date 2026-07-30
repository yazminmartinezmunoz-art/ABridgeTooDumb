using UnityEngine;
using Photon.Pun;

public class JumpController : MonoBehaviourPunCallbacks
{
    private Rigidbody rb;
    private Animator animator;
    public bool isGrounded = false;
    public float jumpStrenght = 300f;
    public LayerMask groundLayer;
    private CapsuleCollider capsuleCollider;
    private bool wantToJump;
    public float distance;

    public Vector3 offset;

    public PhysicsMaterial noFrictionMaterial;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider>();
    }
    void Update()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        if (photonView.IsMine)
        {
            
            bool wasGrounded = isGrounded;
            Vector3 origin = transform.position + offset;// + capsuleCollider.center;

            isGrounded = Physics.SphereCast(origin, capsuleCollider.radius * 20, Vector3.down, out RaycastHit hit, distance, groundLayer, QueryTriggerInteraction.Ignore);

            if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            {
                wantToJump = true;
                capsuleCollider.material = noFrictionMaterial;
                animator.SetTrigger("Jump"); // local
                photonView.RPC("PlayJumpAnimation", RpcTarget.Others);
            }

            if (!isGrounded)
            {
                capsuleCollider.material = noFrictionMaterial;
            }

            else
            {
                capsuleCollider.material = null;
            }
        }     
    }
    [PunRPC]
    void PlayJumpAnimation()
    {
        animator.SetTrigger("Jump");
    }
    private void FixedUpdate()
    {
        if (wantToJump)
        {
            rb.angularVelocity = new Vector3(rb.angularVelocity.x, 0, rb.angularVelocity.z);
            rb.AddForce(Vector3.up * jumpStrenght, ForceMode.Impulse);

            wantToJump = false;
        }
    }
    private void OnDrawGizmosSelected()
    {
        if (capsuleCollider == null)
            capsuleCollider = GetComponent<CapsuleCollider>();

        Vector3 origin = transform.position + offset;// + capsuleCollider.center;
        float radius = capsuleCollider.radius * 20;
        //float distance = (capsuleCollider.height / 2f);// - capsuleCollider.radius + 0.02f;

        Vector3 direction = Vector3.down;
        Vector3 endPosition = origin + direction * distance;

        // Color según estado
        Gizmos.color = isGrounded ? Color.green : Color.red;

        // Esfera inicial
        Gizmos.DrawWireSphere(origin, radius);

        // Esfera final
        Gizmos.DrawWireSphere(endPosition, radius);

        // Línea de trayectoria
        Gizmos.DrawLine(origin, endPosition);
    }
}
