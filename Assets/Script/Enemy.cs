using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float patrolSpeed = 2f, chaseSpeed = 3.5f, patrolDistance = 3f, detectionRange = 5f;
    private Vector3 startPos;
    private int direction = 1;
    private Transform player;
    private bool isChasing = false, isAttacking = false;
    private Animator animator;

    private void Start()
    {
        startPos = transform.position;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        animator = GetComponent<Animator>();

        if (animator == null) Debug.LogError("⚠️ Animator chưa được gắn vào Enemy!");
    }

    private void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        float distanceFromStart = Mathf.Abs(player.position.x - startPos.x);

        if (isAttacking) return; // Nếu đang tấn công thì không di chuyển

        if (distanceToPlayer <= detectionRange && distanceFromStart <= patrolDistance)
        {
            if (!isChasing)
            {
                Debug.Log("🚀 Enemy bắt đầu đuổi theo Player!");
                animator.SetBool("IsMoving", true);
            }

            isChasing = true;
            ChasePlayer();
        }
        else
        {
            if (isChasing)
            {
                Debug.Log("❌ Enemy ngừng đuổi theo Player!");
                isChasing = false;
                animator.SetBool("IsMoving", false);
            }
            Patrol();
        }
    }

    private void Patrol()
    {
        transform.position += Vector3.right * patrolSpeed * direction * Time.deltaTime;
        animator.SetBool("IsMoving", true);
        if (Mathf.Abs(transform.position.x - startPos.x) >= patrolDistance) Flip();
    }

    private void ChasePlayer()
    {
        Vector3 target = new Vector3(player.position.x, transform.position.y, transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, target, chaseSpeed * Time.deltaTime);
        if ((player.position.x > transform.position.x && direction == -1) ||
            (player.position.x < transform.position.x && direction == 1)) Flip();
    }

    private void Flip()
    {
        direction *= -1;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("🔥 Enemy bắt đầu tấn công Player!");
            isAttacking = true;
            animator.SetBool("IsAttacking", true);
            animator.SetBool("IsMoving", false);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("⚡ Enemy ngừng tấn công!");
            isAttacking = false;
            animator.SetBool("IsAttacking", false);
            animator.SetBool("IsMoving", true);
        }
    }
}
