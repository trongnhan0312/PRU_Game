using UnityEngine;

public class PlayerFireBall : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 25f;
    [SerializeField] private float timeDestroy = 0.5f;

    [SerializeField] private float damage = 10f;
    [SerializeField] GameObject bloodPrefab;
    private Vector2 moveDirection; // Lưu hướng bay của đạn

    void Start()
    {
        // Định hướng bay theo hướng của nhân vật khi tạo đạn
        moveDirection = transform.right * (transform.localScale.x > 0 ? 1 : -1);

        Destroy(gameObject, timeDestroy);
    }

    void Update()
    {
        MoveFireBall();
    }

    void MoveFireBall()
    {
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                GameObject blood = Instantiate(bloodPrefab, transform.position, Quaternion.identity);
                Destroy(blood, 1f);
            }

            Boss boss = collision.GetComponent<Boss>();
            if (boss != null)
            {
                boss.TakeDamage(damage);
                GameObject bloodBoss = Instantiate(bloodPrefab, transform.position, Quaternion.identity);
                Destroy(bloodBoss, 1f);
            }

            Destroy(gameObject);
        }
    }
}
