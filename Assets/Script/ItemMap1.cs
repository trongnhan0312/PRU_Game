using UnityEngine;

public class ItemMap1 : MonoBehaviour
{
    public delegate void ItemCollectedHandler();
    public event ItemCollectedHandler onCollected;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("📦 Vật phẩm đã được nhặt!");

            NextLevelTrigger trigger = FindObjectOfType<NextLevelTrigger>();
            if (trigger != null)
            {
                trigger.CollectItem(); // Báo hiệu rằng item đã được nhặt
            }

            // Gọi sự kiện nếu có
            onCollected?.Invoke();
            Destroy(gameObject); // Xóa vật phẩm khỏi game
        }
    }
}
