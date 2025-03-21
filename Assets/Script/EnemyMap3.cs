
using System.Collections;
using UnityEngine;

public class EnemyMap3 : Enemy
{
    public Animator animator;
    public float attackCooldown = 1f; // Thời gian giữa các lần gây sát thương
    private float lastAttackTime = 0f;
    [SerializeField] private GameObject manaObject;
    [SerializeField] private GameObject effectObject;
    [SerializeField] private GameObject nextText; // Tham chiếu đến "-> Next"
    [SerializeField] private GameObject nextCheckpoint; // Checkpoint để qua màn
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip attackClip; // Âm thanh tấn công
    [SerializeField] private AudioClip deathClip;  // Âm thanh khi chết

    public bool IsKilledBoss { get; private set; } = false; // Dùng property

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDame(enterDamage);
                animator.SetBool("IsAttacking", true);
                animator.SetBool("IsMoving", false);
                player.ResetHurtAnimation();

                // ✅ Phát âm thanh khi Enemy tấn công lần đầu
                if (audioSource != null && attackClip != null)
                {
                    audioSource.PlayOneShot(attackClip);
                }
            }
        }
    }


    public float damageInterval = 0.3f; // Thời gian giữa các lần gây sát thương
    private bool isDamaging = false; // Kiểm soát tránh gây sát thương liên tục mỗi frame

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isDamaging)
        {
            StartCoroutine(DealDamageOverTime(collision.gameObject.GetComponent<PlayerController>()));
        }
    }

    private IEnumerator DealDamageOverTime(PlayerController player)
    {
        isDamaging = true;
        while (player != null && isDamaging)
        {
            player.TakeDame(stayDamage);
            Debug.Log("🔥 Gây sát thương liên tục cho Player!");

            // ✅ Phát âm thanh mỗi lần gây sát thương theo thời gian
            if (audioSource != null && attackClip != null)
            {
                audioSource.PlayOneShot(attackClip);
            }

            yield return new WaitForSeconds(damageInterval);
        }
        isDamaging = false;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isDamaging = false;
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                StopAllCoroutines(); // Dừng tất cả coroutine đang chạy
                player.ResetHurtAnimation(); // Gọi hàm để tắt IsHurt khi rời khỏi quái
            }

            animator.SetBool("IsAttacking", false);
            animator.SetBool("IsMoving", true);
        }
    }

    protected override void Die()
    {
        Vector3 effectPosition = transform.position;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, Mathf.Infinity, LayerMask.GetMask("Ground"));
        if (hit.collider != null)
        {
            effectPosition.y = hit.point.y + 0.8f;
        }

        if (effectObject != null)
        {
            GameObject effectDie = Instantiate(effectObject, effectPosition, Quaternion.identity);
            Destroy(effectDie, 0.5f);
        }

        if (manaObject != null)
        {
            Vector3 spawnPosition = transform.position;
            if (hit.collider != null)
            {
                spawnPosition.y = hit.point.y + 0.05f;
            }
            GameObject mana = Instantiate(manaObject, spawnPosition, Quaternion.identity);
        }

        // ✅ Phát âm thanh khi Enemy chết
        if (audioSource != null && deathClip != null)
        {
            audioSource.PlayOneShot(deathClip);
        }

        base.Die();

        if (isBoss)
        {
            IsKilledBoss = true;
        }

        if (isBoss && IsKilledBoss)
        {
            if (nextText != null)
                nextText.SetActive(true);

            if (nextCheckpoint != null)
                nextCheckpoint.SetActive(true);
        }
    }



}
