using System.Collections;
using UnityEngine;

public class BossMap3Controller : Enemy
{
    public Animator animator;
    public float attackCooldown = 1f;
    private float lastAttackTime = 0f;

    [SerializeField] private GameObject manaObject;
    [SerializeField] private GameObject effectObject;
    [SerializeField] private GameObject nextText;
    [SerializeField] private GameObject nextCheckpoint;

    public bool IsKilledBoss { get; private set; } = false;

    [Header("Sát thương kỹ năng")]
    public float attack1Damage = 20f; // Sát thương của chiêu 1
    public float attack2Damage = 35f; // Sát thương của chiêu 2

    private string currentAttack = ""; // Biến lưu chiêu đang dùng

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                animator.SetBool("IsMoving", false);
                animator.SetBool("IsAttacking", true);

                // Ngẫu nhiên chọn giữa Attack1 và Attack2
                if (Random.value > 0.5f)
                {
                   
                    currentAttack = "BossMap3Attack1";
                    player.TakeDame(attack1Damage);
                }
                else
                {
                    currentAttack = "BossMap3Attack2";
                    player.TakeDame(attack2Damage);
                }

                animator.Play(currentAttack);
                Debug.Log($"🔴 Boss dùng {currentAttack}, gây {GetAttackDamage()} sát thương.");
            }
        }
    }

    public float damageInterval = 0.3f;
    private bool isDamaging = false;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!animator.GetBool("IsAttacking"))
            {
                animator.SetBool("IsAttacking", true);
                animator.SetBool("IsMoving", false);
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
        while (player != null && isDamaging)
        {
            player.TakeDame(GetAttackDamage());
            Debug.Log($"🔥 Boss tiếp tục gây {GetAttackDamage()} sát thương với {currentAttack}!");

            yield return new WaitForSeconds(damageInterval);
        }
        isDamaging = false;
    }

    private float GetAttackDamage()
    {
        return currentAttack == "BossMap3Attack1" ? attack1Damage : attack2Damage;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            animator.SetBool("IsAttacking", false);
            animator.SetBool("IsMoving", true);

            StopAllCoroutines();
            isDamaging = false;
        }
    }
}
