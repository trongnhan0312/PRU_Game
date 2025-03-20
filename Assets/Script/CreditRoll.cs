using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; // Nếu dùng TextMeshPro, nếu không thì sử dụng UnityEngine.UI cho Text UI

public class CreditRoll : MonoBehaviour
{
    public float scrollSpeed = 100f; // Tốc độ di chuyển của chữ
    public float startPositionY = -16f; // Vị trí bắt đầu của text (dưới màn hình)
    public float endPositionY = 1000f; // Vị trí kết thúc của text (trên màn hình)

    private TextMeshProUGUI creditText; // Tham chiếu tới TextMeshPro
    private RectTransform rectTransform; // Để thay đổi vị trí của text
    private bool isCreditComplete = false;
    public GameObject PanelCredit;
    private UIManager uiManager; // Tham chiếu đến UIManager

    void Start()
    {
        creditText = GetComponent<TextMeshProUGUI>(); // Lấy TextMeshProUGUI từ đối tượng
        rectTransform = creditText.GetComponent<RectTransform>(); // Lấy RectTransform để thay đổi vị trí
        // Đặt text bắt đầu từ dưới màn hình
        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, startPositionY);

        uiManager = FindObjectOfType<UIManager>();
    }

    void Update()
    {
        if (!isCreditComplete)
        {
            rectTransform.anchoredPosition += new Vector2(0f, scrollSpeed * Time.deltaTime);

            if (rectTransform.anchoredPosition.y > endPositionY)
            {
                rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, startPositionY);
                isCreditComplete = true; // Đánh dấu credit đã hoàn thành
                EndCreditAndChangeScene();
            }
        }
    }

    public void EndCreditAndChangeScene()
    {
        if (uiManager != null)
        {
            PanelCredit.SetActive(false); // Tắt credit panel
            uiManager.MainMenuFromMap3(); // Gọi phương thức từ instance của UIManager
        }
        else
        {
            Debug.LogError("UIManager not found!");
        }
    }
}
