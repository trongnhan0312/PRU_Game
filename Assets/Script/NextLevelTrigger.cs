using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevelTrigger : MonoBehaviour
{

    private bool isTriggered = false; // Tránh load nhiều lần
    private UIManager UIManager;
    public GameObject CircleSpace;
    private bool isItemCollected = false;

    void Start()
    {
        UIManager = FindObjectOfType<UIManager>(); // Tìm GameManager trong Scene

    }

    void Update()
    {
        // 🔥 Nếu boss chưa chết hoặc chưa nhặt item → Chặn player đi qua
        GetComponent<Collider2D>().isTrigger = CanProceedToNextLevel();
    }

    private bool CanProceedToNextLevel()
    {
        // 🔹 Kiểm tra tất cả Boss đã chết
        EnemyMap1[] bosses = FindObjectsOfType<EnemyMap1>();
        foreach (EnemyMap1 boss in bosses)
        {
            if (boss.isBoss && !boss.IsKilledBoss)
            {
                Debug.Log("🚫 Boss chưa chết, không thể qua màn!");
                CircleSpace.SetActive(false);
                return false;
            }
        }

        // 🔹 Kiểm tra xem người chơi đã nhặt vật phẩm chưa
        if (!isItemCollected)
        {
            Debug.Log("📦 Vật phẩm chưa được nhặt, không thể qua màn!");
            CircleSpace.SetActive(false);
            return false;
        }

        Debug.Log("✅ Boss đã chết & vật phẩm đã được nhặt, mở cổng qua màn!");
        CircleSpace.SetActive(true);
        return true;
    }

    // ✅ Gọi khi nhặt vật phẩm
    public void CollectItem()
    {
        isItemCollected = true;
        Debug.Log("🎉 Người chơi đã nhặt vật phẩm!");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isTriggered && CanProceedToNextLevel())
        {
            isTriggered = true; // Đánh dấu đã chạm checkpoint

            // 🔹 Chặn Player di chuyển
            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.bodyType = RigidbodyType2D.Static;
            }

            CircleSpace.SetActive(false);

            // 🔹 Load scene sau 0.2s để tránh lag
            Invoke("LoadNextScene", 0.2f);
        }
    }

    private void LoadNextScene()
    {

        if (UIManager != null)
        {
            UIManager.UnlockMap2();
            UIManager.Map(); // Gọi GameManager để mở khóa Map 2

       
        }
    }
}
