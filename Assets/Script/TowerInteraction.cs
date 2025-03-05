using UnityEngine;
using UnityEngine.InputSystem;

public class TowerInteraction : MonoBehaviour
{
    public GameObject newTowerPrefab; // Prefab của tower mới
    private bool isPlayerInRange = false; // Kiểm tra xem player có ở trong phạm vi không
    private GameObject currentTower; // Tham chiếu đến tower hiện tại
    public NPCDialog npcPrefab;
    void Start()
    {
        currentTower = this.gameObject; // Gán tower ban đầu
    }

    void Update()
    {
        // Kiểm tra khi player nhấn E và đã vào phạm vi
        if (isPlayerInRange && Keyboard.current.eKey.wasPressedThisFrame)
        {
            Debug.Log("Phím E đã được nhấn, thay đổi tower!"); // Thêm log để kiểm tra
            ChangeTowerAsset(); // Thực hiện thay đổi tower
        }
    }

    // Phát hiện va chạm với player
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerInRange = true; // Player vào phạm vi
            Debug.Log("Player đã vào phạm vi tower!"); // Log khi player vào phạm vi
        }
    }

    // Phát hiện khi player rời khỏi phạm vi va chạm
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerInRange = false; // Player ra khỏi phạm vi
            Debug.Log("Player đã ra khỏi phạm vi tower!"); // Log khi player ra khỏi phạm vi
        }
    }

    // Hàm thay đổi asset của tower
    private void ChangeTowerAsset()
    {
        if (newTowerPrefab != null && currentTower != null)
        {
            Debug.Log("Đang thay đổi tower..."); // Thêm log để kiểm tra
            // Tắt tower hiện tại
            currentTower.SetActive(false);
            newTowerPrefab.SetActive(true);
            // Tạo tower mới tại vị trí hiện tại của tower cũ
            //currentTower = Instantiate(newTowerPrefab, transform.position, transform.rotation);
            //currentTower.SetActive(true); // Kích hoạt tower mới
            if (npcPrefab != null)
            {
                Debug.Log("Hiển thị NPC END"); // Thêm log để kiểm tra

                npcPrefab.gameObject.SetActive(true); // Kích hoạt NPC để bắt đầu hội thoại
            }
        }
    }
}
