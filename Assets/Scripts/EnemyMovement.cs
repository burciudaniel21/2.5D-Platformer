using System.Collections;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    private Rigidbody rb;
    private Animator animator;
    private Transform currentPoint;
    public float speed;
    public float delayTime = 3f;
    private bool isWaiting;
    private float waitTimer = 0f;
    private float rotationSpeed = 5;
    public float chaseRadius = 5f;
    public float attackRange = 1.3f;
    private Transform player;
    private bool isChasing = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        if (pointA != null)
        {
            currentPoint = pointA.transform;
        }
    }

    void Update()
    {
        if (!isChasing)
        {
            Patrol();
        }
        else
        {
            ChasePlayer();
        }
    }

    private void Patrol()
    {
        if (!isWaiting)
        {
            animator.SetBool("isWalking", true);
            Vector2 moveDirection = (currentPoint == pointA) ? Vector2.right : Vector2.left;
            rb.velocity = moveDirection * speed;

            Quaternion targetRotation = Quaternion.Euler(0, (currentPoint == pointA) ? 90 : -90, 0);
            rb.rotation = Quaternion.Lerp(rb.rotation, targetRotation, Time.deltaTime * rotationSpeed);

            if (Vector2.Distance(transform.position, currentPoint.position) < 0.5f)
            {
                StartCoroutine(WaitAtEndPoint());
            }
            CheckForPlayer();
        }
        else
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= delayTime)
            {
                isWaiting = false;
                waitTimer = 0f;
            }
            else
            {
                rb.velocity = Vector2.zero;
            }
        }
    }

    private void CheckForPlayer()
    {
        if (Vector3.Distance(transform.position, player.position) < chaseRadius)
        {
            isChasing = true;
        }
    }

    private void ChasePlayer()
    {
        animator.SetBool("isWalking", true);

        // Raycast downwards to detect the ground
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit))
        {
            // Check if the ground has the "Ground" tag
            if (hit.collider.CompareTag("Ground"))
            {
                // Calculate the direction to the player only on the X-axis
                Vector3 moveDirection = new Vector3(player.position.x - transform.position.x, 0f, 0f).normalized;

                // Move the enemy along the X-axis
                rb.velocity = moveDirection * (animator.GetBool("isAttacking") ? 0f : speed);

                // Calculate the rotation towards the player
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);

                // Only rotate the enemy on the Y-axis
                rb.rotation = Quaternion.Lerp(rb.rotation, Quaternion.Euler(0f, targetRotation.eulerAngles.y, 0f), Time.deltaTime * rotationSpeed);
            }
            else
            {
                // If not standing on a platform, stop movement
                rb.velocity = Vector3.zero;
            }
        }

        // Check if the player is within attack range
        if (Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            animator.SetBool("isAttacking", true);
            animator.SetBool("isWalking", false);
        }
        else
        {
            animator.SetBool("isAttacking", false);
        }

        // Check if the player has moved out of the chase radius
        if (Vector3.Distance(transform.position, player.position) > chaseRadius)
        {
            isChasing = false;
        }
    }


    IEnumerator WaitAtEndPoint()
    {
        isWaiting = true;
        animator.SetBool("isWalking", false);
        yield return new WaitForSeconds(delayTime);

        if ((currentPoint == pointA))
        {
            currentPoint = pointB;
        }
        else
        {
            currentPoint = pointA;
        }
        isWaiting = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, chaseRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
