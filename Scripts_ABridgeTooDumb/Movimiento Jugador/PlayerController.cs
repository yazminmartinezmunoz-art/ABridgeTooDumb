using UnityEngine;
using Photon.Pun;
using Unity.Cinemachine;

public class PlayerController : MonoBehaviourPunCallbacks
{
    [Header("Movimiento")]
    public float speed;
    public float rotationVelocity = 200f;

    [Header("Salto")]
    public float jumpStrength = 300f;
    public LayerMask groundLayer;
    public Vector3 groundCheckOffset;
    public float groundCheckDistance;
    public PhysicsMaterial noFrictionMaterial;
    [SerializeField] private bool isGrounded;

    [Header("Coyote Time")]
    [SerializeField] private float coyoteTime = 0.15f;
    private float coyoteTimeCounter;

    [Header("Animacion")]
    public Animator animator;
    public float tValue = 2f;
    private Vector3 smoothDirection = Vector3.zero;

    private Camera cam;
    private Rigidbody rb;
    private CapsuleCollider capsuleCollider;
    private Vector3 moveDirection = Vector3.zero;
    private Quaternion targetRotation;
    private bool hasMoveInput = false;
    private bool wantToJump = false;

    private void Start()
    {
        cam = Camera.main;
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (photonView.IsMine)
        {
            CinemachineCamera cmCamera = FindFirstObjectByType<CinemachineCamera>();
            if (cmCamera != null)
            {
                cmCamera.Follow = transform;
                cmCamera.LookAt = transform;
            }
        }
    }

    private void Update()
    {
        if (!photonView.IsMine) return;

        CheckGround();
        HandleCoyoteTime();
        HandleMovementInput();
        HandleJumpInput();
    }

    private void CheckGround()
    {
        Vector3 origin = transform.position + groundCheckOffset;
        float radius = capsuleCollider.radius * 20f;

        isGrounded = Physics.SphereCast(origin, radius, Vector3.down, out RaycastHit hit, groundCheckDistance, groundLayer, QueryTriggerInteraction.Ignore);

        capsuleCollider.material = isGrounded ? null : noFrictionMaterial;
    }

    private void HandleCoyoteTime()
    {
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
    }

    private void HandleMovementInput()
    {
        float haxis = Input.GetAxisRaw("Horizontal");
        float vaxis = Input.GetAxisRaw("Vertical");

        Vector3 direction = cam.transform.rotation * new Vector3(haxis, 0, vaxis);
        direction.y = 0;
        moveDirection = direction.normalized;

        smoothDirection = Vector3.MoveTowards(smoothDirection, new Vector3(haxis, 0, vaxis), Time.deltaTime * tValue);
        animator.SetFloat("ejeX", smoothDirection.x);
        animator.SetFloat("ejeZ", smoothDirection.z);
        animator.SetBool("Power", Input.GetMouseButton(0) || Input.GetMouseButton(1));

        if (moveDirection.magnitude > 0)
        {
            Vector3 camDirection = cam.transform.forward;
            camDirection.y = 0;

            if (camDirection != Vector3.zero)
            {
                targetRotation = Quaternion.LookRotation(camDirection);
                hasMoveInput = true;
            }
        }
        else
        {
            hasMoveInput = false;
        }
    }

    private void HandleJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && coyoteTimeCounter > 0f)
        {
            wantToJump = true;
            coyoteTimeCounter = 0f;

            animator.SetTrigger("Jump");
            photonView.RPC(nameof(PlayJumpAnimation), RpcTarget.Others);
        }
    }

    [PunRPC]
    private void PlayJumpAnimation()
    {
        animator.SetTrigger("Jump");
    }

    private void FixedUpdate()
    {
        if (!photonView.IsMine) return;

        Vector3 targetVelocity = moveDirection * speed;
        rb.linearVelocity = new Vector3(targetVelocity.x, rb.linearVelocity.y, targetVelocity.z);

        if (hasMoveInput)
        {
            Quaternion nuevaRotacion = Quaternion.RotateTowards(rb.rotation, targetRotation, rotationVelocity * Time.fixedDeltaTime);
            rb.MoveRotation(nuevaRotacion);
        }

        if (wantToJump)
        {
            rb.angularVelocity = new Vector3(rb.angularVelocity.x, 0f, rb.angularVelocity.z);
            rb.AddForce(Vector3.up * jumpStrength, ForceMode.Impulse);
            wantToJump = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (capsuleCollider == null)
        {
            capsuleCollider = GetComponent<CapsuleCollider>();
        }
            

        Vector3 origin = transform.position + groundCheckOffset;
        float radius = capsuleCollider.radius * 20f;
        Vector3 endPosition = origin + Vector3.down * groundCheckDistance;

        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(origin, radius);
        Gizmos.DrawWireSphere(endPosition, radius);
        Gizmos.DrawLine(origin, endPosition);
    }
}