using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float patrolSpeed = 2f; // Tốc độ di chuyển tuần tra
    [SerializeField] private float chaseSpeed = 2f; // Tốc độ khi đuổi theo Player
    [SerializeField] private float patrolDistance = 2f; // Khoảng cách di chuyển qua lại
    [SerializeField] private float detectionRange = 2f; // Phạm vi phát hiện Player

    [SerializeField] protected float maxHp = 50f; // Máu tối đa
    protected float currentHp; // Máu hiện tại
    [SerializeField] protected Image HpBar;

    private Vector2 initialPosition;
    private Vector2 patrolTarget;
    private PlayerController player;
    private bool chasingPlayer = false;
    private bool movingRight = true;

    [SerializeField] protected float enterDamage = 10f; // Sát thương khi Player va chạm
    [SerializeField] protected float stayDamage = 1f; // Sát thương khi tấn công

    private Animator animator;
    private void Start()
    {
        player = FindObjectOfType<PlayerController>();
        initialPosition = transform.position;
        patrolTarget = initialPosition + Vector2.right * patrolDistance;
        currentHp = maxHp;
        animator = GetComponent<Animator>(); // Tham chiếu Animator
        UpdateHpBar();
    }

    private void Update()
    {
        bool isMoving = false; // Mặc định là không di chuyển

        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

            if (distanceToPlayer < detectionRange)
            {
                chasingPlayer = true;
            }
            else
            {
                chasingPlayer = false;
            }
        }

        if (chasingPlayer)
        {
            moveToPlayer();
            isMoving = true; // Enemy đang di chuyển
        }
        else
        {
            Patrol();
            isMoving = true; // Nếu Patrol, vẫn coi là di chuyển
        }

        animator.SetBool("isMoving", isMoving); // Cập nhật Animator
    }


    private void moveToPlayer()
    {
        Vector2 targetPosition = new Vector2(player.transform.position.x, transform.position.y);
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, chaseSpeed * Time.deltaTime);

        FlipEnemy(player.transform.position.x);
    }

    private void Patrol()
    {
        transform.position = Vector2.MoveTowards(transform.position, patrolTarget, patrolSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, patrolTarget) < 0.1f)
        {
            movingRight = !movingRight;
            patrolTarget = initialPosition + (movingRight ? Vector2.right : Vector2.left) * patrolDistance;
            FlipEnemy(patrolTarget.x);
        }
    }

    private void FlipEnemy(float targetX)
    {
        Vector3 newScale = transform.localScale;
        newScale.x = (targetX < transform.position.x) ? -Mathf.Abs(newScale.x) : Mathf.Abs(newScale.x);
        transform.localScale = newScale;
    }

    public virtual void TakeDamage(float damage)
    {
        currentHp -= damage;
        currentHp= Mathf.Max(currentHp, 0);
        UpdateHpBar();
        if (currentHp <= 0) 
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }

    protected virtual void UpdateHpBar()
    {
        if (HpBar != null)
        {
            HpBar.fillAmount = currentHp / maxHp;
        }
    }
}
