using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PlayerController : MonoBehaviour
{

    // 🎯 [1] Cấu hình di chuyển
    [Header("🚀 Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 15f;
    [SerializeField] private float fallLimit = -10f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;

    private bool isGrounded;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    // 🔥 [2] Cấu hình bắn đạn
    [Header("🔫 Shooting Settings")]
    [SerializeField] private Transform firePos;
    [SerializeField] private GameObject bulletPrefabs;
    [SerializeField] private float shotDelay = 0.15f;
    private float nextShot;
    [SerializeField] private int maxMana = 24;

    // ⚔️ [3] Cấu hình tấn công
    [Header("⚔️ Attack Settings")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private float attackDamage = 10f;

    // 🩸 [4] Hiệu ứng & VFX
    [Header("🩸 Effects & UI")]
    [SerializeField] private GameObject blood;
    [SerializeField] private Image manaBar;
    [SerializeField] private Image hpBar;
    [SerializeField] private TextMeshProUGUI ammoManaText;
    [SerializeField] private TextMeshProUGUI ammoHPText;

    // 💖 [5] Chỉ số nhân vật
    [Header("💖 Player Stats")]
    [SerializeField] protected float maxHp = 100f;
    protected float currentHp;
    public float currentMana;

    // 🎮 [6] Hệ thống quản lý
    [Header("🎮 Game Management")]
    [SerializeField] private UIManager UIManager;
    public bool isInDialog = false; // Theo dõi trạng thái hội thoại

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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UIManager.PauseMenu();
        }

    }


    [SerializeField] private AudioSource footstepAudio; // Âm thanh bước chân
    [SerializeField] private AudioClip footstepClip; // File âm thanh
    [SerializeField] private float footstepInterval = 0.3f; // Thời gian giữa mỗi bước chân

    private float footstepTimer = 0f; // Đếm thời gian giữa các bước chân

    private void HandleMovement()
    {
        if (isInDialog) return;

        float moveInput = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        // Đảo hướng nhân vật
        if (moveInput > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput < 0) transform.localScale = new Vector3(-1, 1, 1);

        // Phát âm thanh bước chân khi nhân vật di chuyển
        if (Mathf.Abs(moveInput) > 0.1f)
        {
            footstepTimer += Time.deltaTime;
            if (footstepTimer >= footstepInterval)
            {
                footstepAudio.PlayOneShot(footstepClip);
                footstepTimer = 0f;
            }
        }
        else
        {
            footstepTimer = 0f; // Reset khi dừng
        }
    }

    [SerializeField] private AudioSource jumpAudio; // Âm thanh khi nhảy
    [SerializeField] private AudioClip jumpClip; // File âm thanh

    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpAudio.PlayOneShot(jumpClip); // Phát âm thanh khi nhảy
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


    [SerializeField] private AudioSource attackAudio; // Âm thanh chém
    [SerializeField] private AudioClip attackClip; // File âm thanh chém

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

            attackAudio.PlayOneShot(attackClip); // Phát âm thanh khi tấn công
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
                    enemyScript.TakeDamage(attackDamage); // Gây 10 sát thương
                    GameObject bloodEffect = Instantiate(blood, enemy.transform.position, quaternion.identity);
                    Destroy(bloodEffect, 1f);
                }

                Boss boss = enemy.GetComponent<Boss>();
                if (boss != null)
                {
                    boss.TakeDamage(attackDamage); // Gây 10 sát thương
                    GameObject bloodEffect = Instantiate(blood, boss.transform.position, quaternion.identity);
                    Destroy(bloodEffect, 1f);
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


    [SerializeField] private AudioSource hurtAudio; // Âm thanh khi bị thương
    [SerializeField] private AudioClip hurtClip; // File âm thanh la hét
    [SerializeField] private AudioSource deathAudio; // Âm thanh khi chết
    [SerializeField] private AudioClip deathClip; // File âm thanh chết

    private bool isDead = false; // Kiểm tra trạng thái nhân vật

    public void TakeDame(float damage)
    {
        if (isDead) return; // Không nhận sát thương nếu đã chết

        currentHp -= damage;
        currentHp = Mathf.Max(currentHp, 0);
        UpdateHpBar();
        UpdateAmmoHPText();
        StopAllCoroutines(); // Dừng tất cả animation trước đó để tránh bị ghi đè
        animator.SetBool("IsHurt", true); // Chạy animation bị thương
        StartCoroutine(HurtEffect());

        hurtAudio.PlayOneShot(hurtClip); // Phát âm thanh bị thương

        if (currentHp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return; // Đảm bảo chỉ chết một lần
        isDead = true;

        animator.SetTrigger("Die"); // Kích hoạt animation chết
        deathAudio.PlayOneShot(deathClip); // Phát âm thanh chết

        // Hiển thị Game Over ngay lập tức
        UIManager.GameOverMenu();
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

    [SerializeField] private AudioSource claimAudio; // Âm thanh khi nhặt vật phẩm
    [SerializeField] private AudioClip claimClip; // File âm thanh nhặt vật phẩm
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Mana"))
        {
            claimAudio.PlayOneShot(claimClip);
            Destroy(collision.gameObject);
            IncreaseMana(5);
            UpdateAmmoUI();// Cộng 5 Mana
        }
        if (collision.CompareTag("HP"))
        {
            claimAudio.PlayOneShot(claimClip);
            Destroy(collision.gameObject);
            IncreaseHP(10);
            UpdateHpBar(); // Cộng 10 HP
        }
        if (collision.CompareTag("KC"))
        {
            Debug.Log("📦 Nhặt vật phẩm!");
            claimAudio.PlayOneShot(claimClip);
            Destroy(collision.gameObject);
        }
        if (collision.CompareTag("Trap"))
        {
            Debug.Log("🔥 Va chạm với Trap!");
            Die();
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