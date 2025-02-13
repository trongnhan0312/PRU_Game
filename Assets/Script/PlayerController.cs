using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 15f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    private Animator animator;
    private bool isGrounded;
    private Rigidbody2D rb;


    [SerializeField] private Transform firePos;
    [SerializeField] private GameObject bulletPrefabs;
    [SerializeField] private float shotDelay=0.15f;
    private float nextShot;
    [SerializeField] private int maxAmmo = 24;
    public int currentAmmo;

    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private LayerMask enemyLayers;


    /* private GameManager gameManager;*/

    [SerializeField] private Image ammoBar; // Thanh màu xanh (UI Image)
    [SerializeField] protected float maxHp = 100f;
    protected float currentHp;
    [SerializeField] private Image hpBar;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
/*        gameManager = FindAnyObjectByType<GameManager>();*/
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentAmmo=maxAmmo;
        currentHp = maxHp;
        UpdateHpBar();
    }

    // Update is called once per frame
    void Update()
    {
/*        if (gameManager.IsGameOver() || gameManager.IsGameWin()) return;*/
        HandleMovement();
        HandleJump();
        UpdateAnimation();
        HandleShoot();
        HandleAttack();

    }

    private void HandleMovement()
    {
        float moveInput = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
        if (moveInput > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput < 0) transform.localScale = new Vector3(-1, 1, 1);
    }

    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);

    }
    private IEnumerator ShootRoutine()
    {
        animator.SetTrigger("Shoot");

        // Chờ đến khi animation bắn kết thúc
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        // Tạo đạn
        GameObject bullet = Instantiate(bulletPrefabs, firePos.position, firePos.rotation);

        // Kiểm tra hướng nhân vật
        if (transform.localScale.x < 0)
        {
            Vector3 bulletScale = bullet.transform.localScale;
            bulletScale.x *= -1; // Lật viên đạn lại
            bullet.transform.localScale = bulletScale;
        }

        currentAmmo--;
        UpdateAmmoUI();
    }

    private void HandleShoot()
    {
        bool isRunning = Mathf.Abs(rb.linearVelocity.x) > 0.1f; // Kiểm tra chạy
        bool isJumping = !isGrounded; // Kiểm tra nhảy

        // Nếu đang chạy hoặc đang nhảy thì không thể bắn
        if (isRunning || isJumping) return;

        if (Input.GetMouseButtonDown(1) && currentAmmo > 0 && Time.time > nextShot)
        {
            nextShot = Time.time + shotDelay;
            StartCoroutine(ShootRoutine());
        }
    }
    private void HandleAttack()
    {
        bool isRunning = Mathf.Abs(rb.linearVelocity.x) > 0.1f;
        bool isJumping = !isGrounded;
        if (isRunning || isJumping) return;

        if (Input.GetMouseButtonDown(0)) // Chuột trái để đánh
        {
            animator.SetTrigger("IsAttacking");
            Attack(); // Gọi trực tiếp Attack()
        }
    }



    private void Attack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange);

        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.CompareTag("Enemy")) // Kiểm tra tag Enemy
            {
                Enemy enemyScript = enemy.GetComponent<Enemy>();
                if (enemyScript != null)
                {
                    enemyScript.TakeDamage(10f); // Gây 10 sát thương
                }
            }
        }
    }

    private void UpdateAmmoUI()
    {
        if (ammoBar != null)
            ammoBar.fillAmount = (float)currentAmmo / maxAmmo;
    }

    private void UpdateAnimation()
    {
        bool isRunning = Mathf.Abs(rb.linearVelocity.x) > 0.1f;
        bool isJumping = !isGrounded;

        animator.SetBool("IsRunning", isRunning);
        animator.SetBool("IsJumping", isJumping);
    }


    public void TakeDame(float damage)
    {
        currentHp -= damage;
        currentHp = Mathf.Max(currentHp, 0);
        UpdateHpBar();

        StopAllCoroutines(); // Dừng tất cả animation trước đó để tránh bị ghi đè
        animator.SetBool("IsHurt", true); // Chạy animation bị thương

        if (currentHp <= 0)
        {
            Die();
        }
    }



    private void Die()
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
    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
    public void ResetHurtAnimation()
    {
        animator.SetBool("IsHurt", false);
    }

}