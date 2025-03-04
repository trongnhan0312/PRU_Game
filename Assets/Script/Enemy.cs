using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float patrolSpeed = 2f, chaseSpeed = 3.5f, patrolDistance = 3f, detectionRange = 5f;
    private Vector3 startPos;
    private int direction = 1;
    private Transform player;
    private bool isChasing = false, isAttacking = false;
    private Animator animator;

    [SerializeField] protected float maxHp = 50f;
    protected float currentHp;
    [SerializeField] private Image hpBar;

    [SerializeField] protected float enterDamage = 10f;
    [SerializeField] protected float stayDamage = 1f;

    private bool isHurt = false;
    [SerializeField] public bool isBoss = false; // Đánh dấu boss

    private void Start()
    {
        startPos = transform.position;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        animator = GetComponent<Animator>();
        currentHp = maxHp;
        UpdateHpBar();
        if (animator == null) Debug.LogError("⚠️ Animator chưa được gắn vào Enemy!");
    }

    private void Update()
    {
        if (player == null || isHurt) return; // Nếu bị tấn công, không di chuyển

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
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * direction, transform.localScale.y, transform.localScale.z);
    }




    // Coroutine để xử lý animation đánh + gây sát thương định kỳ
    private IEnumerator AttackRoutine(PlayerController player)
    {
        while (isAttacking && player != null)
        {
            player.TakeDame(enterDamage); // Gây damage khi bắt đầu chạm
            yield return new WaitForSeconds(1f); // Thời gian delay cho mỗi lần đánh
            player.TakeDame(stayDamage); // Gây damage theo thời gian
        }
    }


    public void TakeDamage(float damage)
    {
        currentHp -= damage;
        currentHp = Mathf.Max(currentHp, 0);
        UpdateHpBar();

        if (!isHurt) // Chỉ dừng di chuyển khi lần đầu bị tấn công
        {
            isHurt = true;
            Debug.Log("🔥 Quái bị tấn công!");
            StartCoroutine(HurtRecovery());
        }

        if (currentHp <= 0)
        {
            Die();
        }
    }

    // Coroutine để hiển thị animation bị thương và tạm dừng hành động
    private IEnumerator HurtRecovery()
    {
        yield return new WaitForSeconds(0.5f); // Thời gian chờ khi bị tấn công
        isHurt = false; // Quái có thể di chuyển lại
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }

    private void UpdateHpBar()
    {
        if (hpBar != null)
        {
            hpBar.fillAmount = currentHp / maxHp;
        }
    }
}