using System.Collections;
using UnityEngine;

public class BossMap3Controller : Enemy
{
    public Animator animator;
    public float attackCooldown = 1f; // Thời gian giữa các lần gây sát thương
    private float lastAttackTime = 0f;
    [SerializeField] private GameObject manaObject;
    [SerializeField] private GameObject effectObject;
    [SerializeField] private GameObject nextText; // Tham chiếu đến "-> Next"
    [SerializeField] private GameObject nextCheckpoint; // Checkpoint để qua màn
    public bool IsKilledBoss { get; private set; } = false; // Dùng property

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDame(enterDamage);
                animator.SetBool("IsMoving", false);

                animator.Play("BossMap3Attack1"); // Chạy animation trực tiếp
                animator.SetBool("IsAttacking", true);

                Debug.Log("🔴 Đã kích hoạt Attack Animation");
            }
        }
    }



    public float damageInterval = 0.3f; // Thời gian giữa các lần gây sát thương
    private bool isDamaging = false; // Kiểm soát tránh gây sát thương liên tục mỗi frame

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!animator.GetBool("IsAttacking"))
            {
                animator.SetBool("IsAttacking", true);
                animator.SetBool("IsMoving", false);
                Debug.Log("🔴 Boss bắt đầu tấn công Player");
            }

            if (!isDamaging)
            {
                StartCoroutine(DealDamageOverTime(collision.gameObject.GetComponent<PlayerController>()));
            }
        }
    }


    private IEnumerator DealDamageOverTime(PlayerController player)
    {
        isDamaging = true;
        while (player != null && isDamaging) // Chỉ tiếp tục nếu player còn trong vùng quái
        {
            player.TakeDame(stayDamage);

            Debug.Log("🔥 Gây sát thương liên tục cho Player!");
            yield return new WaitForSeconds(damageInterval);
        }
        isDamaging = false;
    }



    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (animator.GetBool("IsAttacking"))
            {
                animator.SetBool("IsAttacking", false);
                animator.SetBool("IsMoving", true);
                Debug.Log("⚠️ Boss ngừng tấn công, chuyển về trạng thái di chuyển");
            }

            StopAllCoroutines(); // Dừng gây sát thương khi Player rời đi
            isDamaging = false;
        }
    }


    protected override void Die()
    {
        Vector3 effectPosition = transform.position;

        // Raycast kiểm tra vị trí mặt đất để đặt hiệu ứng chết
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, Mathf.Infinity, LayerMask.GetMask("Ground"));
        if (hit.collider != null)
        {
            effectPosition.y = hit.point.y + 0.8f;
        }

        // ✅ Tạo hiệu ứng chết (Luôn có)
        if (effectObject != null)
        {
            GameObject effectDie = Instantiate(effectObject, effectPosition, Quaternion.identity);
            Destroy(effectDie, 0.5f);
        }

        // ✅ Nếu có manaObject thì spawn
        if (manaObject != null)
        {
            Vector3 spawnPosition = transform.position;

            // Kiểm tra vị trí để spawn mana trên mặt đất
            if (hit.collider != null)
            {
                spawnPosition.y = hit.point.y + 0.05f;
            }

            GameObject mana = Instantiate(manaObject, spawnPosition, Quaternion.identity);
            Destroy(mana, 10f);
        }

        base.Die();
        if (isBoss)
        {
            IsKilledBoss = true;
        }
        if (isBoss && IsKilledBoss) // Nếu là boss thì hiển thị "Next" và mở checkpoint
        {
            if (nextText != null)
                nextText.SetActive(true); // Hiện chữ "-> Next"

            if (nextCheckpoint != null)
                nextCheckpoint.SetActive(true); // Bật trigger qua màn
        }
    }


}
