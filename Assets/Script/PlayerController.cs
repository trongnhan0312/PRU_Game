    using System.Collections;
    using Unity.Mathematics;
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;
public class PlayerController : MonoBehaviour
{

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 15f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    private Animator animator;
    private bool isGrounded;
    [SerializeField] private float fallLimit = -10f;
    private Rigidbody2D rb;


    [SerializeField] private Transform firePos;
    [SerializeField] private GameObject bulletPrefabs;
    [SerializeField] private float shotDelay = 0.15f;
    private float nextShot;
    [SerializeField] private int maxMana = 24;


    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private TextMeshProUGUI ammoManaText;
    [SerializeField] private TextMeshProUGUI ammoHPText;

    [SerializeField] GameObject blood;
    private SpriteRenderer spriteRenderer;
    /* private GameManager gameManager;*/

    [SerializeField] private Image manaBar; // Thanh màu xanh (UI Image)
    [SerializeField] protected float maxHp = 100f;
    protected float currentHp;
    public float currentMana;
    [SerializeField] private Image hpBar;

    [SerializeField] private GameManager gameManager;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        /*        gameManager = FindAnyObjectByType<GameManager>();*/
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentMana = maxMana;
        currentHp = maxHp;
        UpdateHpBar();
        UpdateAmmoManaText();
        UpdateAmmoHPText();
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
        if (transform.position.y < fallLimit)
        {
            Die();
        }
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            gameManager.PauseMenu();
        }

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

        currentMana--;
        UpdateAmmoUI();
        UpdateAmmoManaText();
    }

    private void HandleShoot()
    {
        bool isRunning = Mathf.Abs(rb.linearVelocity.x) > 0.1f; // Kiểm tra chạy
        bool isJumping = !isGrounded; // Kiểm tra nhảy

        // Nếu đang chạy hoặc đang nhảy thì không thể bắn
        if (isRunning || isJumping) return;

        if (Input.GetMouseButtonDown(1) && currentMana > 0 && Time.time > nextShot)
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
            animator.SetBool("IsHurt", false);
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
                Boss boss = enemy.GetComponent<Boss>();

                if (enemyScript != null)
                {
                    enemyScript.TakeDamage(10f); // Gây 10 sát thương
                    GameObject bloodEffect = Instantiate(blood, enemy.transform.position, Quaternion.identity);
                    Destroy(bloodEffect, 1f);
                }

                if (boss != null)
                {
                    boss.TakeDamage(10f); // Gây 10 sát thương
                    GameObject bloodEffectBoss = Instantiate(blood, boss.transform.position, Quaternion.identity);
                    Destroy(bloodEffectBoss, 1f);
                }
            }
        }
    }


    private void UpdateAmmoUI()
    {
        if (manaBar != null)
            manaBar.fillAmount = (float)currentMana / maxMana;
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
        UpdateAmmoHPText();
        StopAllCoroutines(); // Dừng tất cả animation trước đó để tránh bị ghi đè
        animator.SetBool("IsHurt", true); // Chạy animation bị thương
        StartCoroutine(HurtEffect());

        if (currentHp <= 0)
        {
            Die();
        }
    }



    private void Die()
    {
        gameManager.GameOverMenu();
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

    // Hiệu ứng nhấp nháy đỏ
    private IEnumerator HurtEffect()
    {

        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(0.1f);

    }
    // ✅ Hàm cập nhật thanh mana
    private void UpdateManaBar()
    {
        if (manaBar != null)
        {
            manaBar.fillAmount = currentMana / maxMana;
        }
    }

    // ✅ Hàm tăng mana
    public void IncreaseMana(float amount)
    {
        currentMana += amount;
        currentMana = Mathf.Min(currentMana, maxMana); // Không vượt quá giới hạn
        UpdateManaBar(); // Cập nhật UI
        UpdateAmmoManaText();
    }
    public void IncreaseHP(float amount)
    {
        currentHp += amount;
        currentHp = Mathf.Min(currentHp, maxHp); // Không vượt quá giới hạn
        UpdateHpBar(); // Cập nhật UI
        UpdateAmmoHPText();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Mana"))
        {
            Destroy(collision.gameObject);
            IncreaseMana(2);
            UpdateAmmoUI();// Cộng 2 Mana
        }
        if (collision.CompareTag("HP"))
        {
            Destroy(collision.gameObject);
            IncreaseHP(20);
            UpdateHpBar(); // Cộng 20 HP
        }
        if (collision.CompareTag("KC"))
        {
            Destroy(collision.gameObject);
        }
    }
    private void UpdateAmmoManaText()
    {
        if (ammoManaText != null)
        {
            if (currentMana > 0)
            {
                ammoManaText.text = currentMana.ToString();
            }
            else
            {
                ammoManaText.text = "Empty";
            }
        }

    }
    private void UpdateAmmoHPText()
    {
        if (ammoHPText != null)
        {
            if (currentHp > 0)
            {
                ammoHPText.text = currentHp.ToString();
            }
            else
            {
                ammoHPText.text = "0";
            }
        }

    }
}