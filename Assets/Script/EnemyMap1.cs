using System.Collections;
using UnityEngine;

public class EnemyMap1 : Enemy
{
    public Animator animator;
    public float attackCooldown = 1f; // Thời gian giữa các lần gây sát thương
    private float lastAttackTime = 0f;
    [SerializeField] private GameObject manaObject;
    [SerializeField] private GameObject effectObject;
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
        if (manaObject != null)
        {
            Vector3 spawnPosition = transform.position;

            // Raycast kiểm tra vị trí mặt đất
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, Mathf.Infinity, LayerMask.GetMask("Ground"));
            if (hit.collider != null)
            {
                spawnPosition.y = hit.point.y + 0.05f; // Đặt item hơi cao một chút để tránh bị lọt vào nền
            }
            Vector3 effect = transform.position;

            // Raycast kiểm tra vị trí mặt đất
            RaycastHit2D hits = Physics2D.Raycast(transform.position, Vector2.down, Mathf.Infinity, LayerMask.GetMask("Ground"));
            if (hit.collider != null)
            {
                effect.y = hit.point.y + 0.8f; // Đặt effect hơi cao một chút để tránh bị lọt vào nền
            }
            GameObject effectDie = Instantiate(effectObject, effect, Quaternion.identity);
            Destroy(effectDie, 0.5f);
            GameObject mana = Instantiate(manaObject, spawnPosition, Quaternion.identity);
            Destroy(mana, 10f);
        }
        base.Die();
    }


}
