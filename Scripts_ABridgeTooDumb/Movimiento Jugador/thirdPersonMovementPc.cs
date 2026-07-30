using UnityEngine;
using Photon.Pun;
using Unity.Cinemachine;
using UnityEngine.Rendering;
using static UnityEngine.UI.Image;

public class thirdPersonMovementPc : MonoBehaviourPunCallbacks
{
    [Header("Movimiento")]
    public float speed;
    public float rotationVelocity = 200f;

    [Header("Animacion")]
    public Animator animator;
    public float tValue = 2f;
    private Vector3 smoothDirection = Vector3.zero;

    [Header("Coyote Jump")]
    [SerializeField] private float coyoteTime = 0.15f;
    [SerializeField] private float coyoteTimeCounter;

    private Camera cam;
    private Rigidbody rb;
    private Vector3 moveDirection = Vector3.zero;
    private Quaternion targetRotation;
    private bool hasMoveInput = false;



    private void Start()
    {
        cam = Camera.main;
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (photonView.IsMine)
        {
            CinemachineCamera cmCamera = FindFirstObjectByType<CinemachineCamera>();

            if (cmCamera != null)
            {
                cmCamera.Follow = this.transform;
                cmCamera.LookAt = this.transform;
            }
        }
    }
    
    void Update()
    {
        if (!photonView.IsMine) return;


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
            targetRotation = Quaternion.LookRotation(camDirection);

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

    private void OnGroundHandler()
    {
        /*
        if (characterController.isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }

        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
        */
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
    }
}