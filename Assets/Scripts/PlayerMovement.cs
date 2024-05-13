using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private float moveSpeed = 5f;
    private float jumpForce = 10f;
    public GameObject playerBody;

    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference jumpAction;

    private Rigidbody rb;
    private Vector2 moveInput;
    private bool isGrounded = true;
    private float rotationSpeed = 5f;
    Animator animator;
    private bool isPlayerIdle;
    private int animationToPlay;

    private void Start()
    {
        playerBody.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
        animator = GetComponent<Animator>();
    }
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        moveAction.action.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        moveAction.action.canceled += ctx => moveInput = Vector2.zero;

        jumpAction.action.performed += ctx => Jump();
    }

    private void OnEnable()
    {
        moveAction.action.Enable();
        jumpAction.action.Enable();
    }

    private void OnDisable()
    {
        moveAction.action.Disable();
        jumpAction.action.Disable();
    }

    private void FixedUpdate()
    {
        Move();
        HandleIdleAnimation(isPlayerIdle);
    }

    Vector3 lastMovementDirection;

    // Define a timer variable to track the duration of inactivity
    float idleTimer = 0f;

    public void Move()
    {
        Vector3 moveDirection = new Vector3(moveInput.x, 0f, 0f); // Only move left/right
        Vector3 movement = transform.TransformDirection(moveDirection) * moveSpeed * Time.fixedDeltaTime;

        // Check if the player is moving
        bool isMoving = movement.magnitude > 0.01f;

        // Set the animation parameter based on whether the player is moving or not
        animator.SetBool("IsWalking", isMoving);

        if (isMoving)
        {
            isPlayerIdle = false;
            rb.MovePosition(rb.position + movement);

            // Update last movement direction
            lastMovementDirection = movement.normalized;

            // Calculate the angle between the last movement direction and the forward direction of the player
            float angle = Vector3.SignedAngle(Vector3.forward, lastMovementDirection, Vector3.up);

            // Convert the angle to rotation around the Y-axis
            Quaternion targetRotation = Quaternion.Euler(0f, angle - 180f, 0f);

            Quaternion oppositeRotation = Quaternion.Inverse(targetRotation);
            playerBody.transform.rotation = Quaternion.Lerp(playerBody.transform.rotation, oppositeRotation, Time.fixedDeltaTime * rotationSpeed);

        }
        else
        {
            isPlayerIdle = true;
        }
    }
    public void IdleAnimationFinished()
    {
        // Reset all idle animations
        animator.SetBool("IsIdle", false);
        animator.SetBool("Idle2", false);
        animator.SetBool("Idle3", false);
        animator.SetBool("Idle4", false);
    }

    void ChoseRandomAnimation()
    {
        animationToPlay = Random.Range(0, 4);
    }

    private void HandleIdleAnimation(bool isIdle)
    {
        // If player was moving but is not moving now, start counting idle time
        if (isIdle)
        {
            idleTimer += Time.fixedDeltaTime;
        }
        else
        {
            idleTimer = 0f; // Reset idle timer if the player is moving
            IdleAnimationFinished();
        }

        // Set the "IsIdle" parameter if idle time exceeds 5 seconds
        if (idleTimer >= 5f)
        {

            switch (animationToPlay)
            {
                case 0:
                    animator.SetBool("IsIdle", true);
                    break;
                case 1:
                    animator.SetBool("Idle2", true);
                    break;
                case 2:
                    animator.SetBool("Idle3", true);
                    break;
                case 3:
                    animator.SetBool("Idle4", true);
                    break;
            }
        }
        else
        {
            IdleAnimationFinished();
        }
    }

    public void Jump()
    {
        if (isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}
