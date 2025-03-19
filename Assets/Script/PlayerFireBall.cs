using UnityEngine;

public class PlayerFireBall : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 25f;
    [SerializeField] private float timeDestroy = 0.5f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private GameObject bloodPrefab;
    [SerializeField] private AudioClip fireSound; // Thêm âm thanh viên đạn bay
    private Vector2 moveDirection;
    private AudioSource audioSource;

    void Start()
    {
        moveDirection = transform.right * (transform.localScale.x > 0 ? 1 : -1);
        audioSource = GetComponent<AudioSource>();

        // Chạy âm thanh khi viên đạn bay
        if (fireSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(fireSound);
        }

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

            // Ẩn viên đạn thay vì xóa ngay
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<Collider2D>().enabled = false;
            moveSpeed = 0;

            // Xóa đạn sau khi âm thanh phát xong
            Destroy(gameObject, 1f);
        }
    }

}