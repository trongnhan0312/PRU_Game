using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevelTriggerMap2 : MonoBehaviour
{
    private bool isTriggered = false; // Để tránh load nhiều lần

    void Update()
    {
        // 🔥 Nếu boss chưa chết -> Chặn player đi qua
        GetComponent<Collider2D>().isTrigger = IsBossDefeated();
    }

    private bool IsBossDefeated()
    {
        EnemyMap2[] bosses = FindObjectsByType<EnemyMap2>(FindObjectsSortMode.None); // Tìm tất cả EnemyMap2 trong scene

        foreach (EnemyMap2 boss in bosses)
        {
            if (boss.isBoss && !boss.IsKilledBoss) // Nếu có boss chưa chết thì không thể qua màn
            {
                Debug.Log("🚫 Boss chưa chết, không thể qua màn!");
                return false;
            }
        }
        Debug.Log("✅ Tất cả boss đã bị tiêu diệt, mở cổng qua màn!");
        return true; // Nếu tất cả boss đã chết, cho phép qua màn
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isTriggered && IsBossDefeated())
        {
            isTriggered = true; // Đánh dấu đã chạm checkpoint

            // Chặn Player di chuyển
            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero; // Dừng mọi di chuyển
                rb.bodyType = RigidbodyType2D.Static; // Đặt Player thành Static để không đi xuyên
            }

            // Load scene sau 0.2s để tránh lag
            Invoke("LoadNextScene", 0.2f);
        }
    }


    private void LoadNextScene()
    {
        SceneManager.LoadScene("Map3"); // Đổi "Map2" thành tên scene tiếp theo
    }
}
