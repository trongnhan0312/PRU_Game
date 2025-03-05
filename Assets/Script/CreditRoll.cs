using UnityEngine;
using TMPro; // Nếu dùng TextMeshPro, nếu không thì sử dụng UnityEngine.UI cho Text UI

public class CreditRoll : MonoBehaviour
{
    public float scrollSpeed = 30f; // Tốc độ di chuyển của chữ
    public float startPositionY = -500f; // Vị trí bắt đầu của text (dưới màn hình)
    public float endPositionY = 1000f; // Vị trí kết thúc của text (trên màn hình)

    private TextMeshProUGUI creditText; // Tham chiếu tới TextMeshPro
    private RectTransform rectTransform; // Để thay đổi vị trí của text
    private bool isCreditComplete = false;

    void Start()
    {
        creditText = GetComponent<TextMeshProUGUI>(); // Lấy TextMeshProUGUI từ đối tượng
        rectTransform = creditText.GetComponent<RectTransform>(); // Lấy RectTransform để thay đổi vị trí
        // Đặt text bắt đầu từ dưới màn hình
        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, startPositionY);
    }

    void Update()
    {
        // Di chuyển Text lên trên
        rectTransform.anchoredPosition += new Vector2(0f, scrollSpeed * Time.deltaTime);

        // Khi text vượt qua vị trí kết thúc, reset về vị trí bắt đầu
        if (rectTransform.anchoredPosition.y > endPositionY)
        {
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, startPositionY);
            isCreditComplete = true; // Đánh dấu credit đã hoàn thành
            EndCreditAndChangeScene();
        }
    }
    private void EndCreditAndChangeScene()
    {
        // Chuyển sang scene mới (Map 1)
        UnityEngine.SceneManagement.SceneManager.LoadScene("Map1"); // Đảm bảo tên scene là "Map1" hoặc thay thế theo tên scene của bạn.
    }
}
