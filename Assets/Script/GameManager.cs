using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Rendering.LookDev;

public class GameManager : MonoBehaviour
{
    private int score = 0;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject gameOverMenu;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject MapSelection;
    public GameObject InfoButton;
    public GameObject guidePanel;

    [SerializeField] private GameObject buttonMap2; // Kéo button vào Inspector
    [SerializeField] private GameObject buttonMap3;

    void Start()
    {
        buttonMap2.SetActive(false); // Ẩn button khi bắt đầu game
        buttonMap3.SetActive(false);
        UpdateScore();
        MainMenu();
    }

    void Update() { }

    public void ToggleGuidePanel()
    {
        bool isActive = !guidePanel.activeSelf;
        guidePanel.SetActive(isActive);
    }

    public void AddScore(int point)
    {
        score += point;
        UpdateScore();
    }

    public void UpdateScore()
    {
        scoreText.text = score.ToString();
    }

    public void MainMenu()
    {
        mainMenu.SetActive(true);
        gameOverMenu.SetActive(false);
        pauseMenu.SetActive(false);
        Time.timeScale = 0;
    }

    public void mapSelection()
    {
        MapSelection.SetActive(true);
        mainMenu.SetActive(false);
        gameOverMenu.SetActive(false);
        pauseMenu.SetActive(false);
        Time.timeScale = 0f;
    }

    public void GameOverMenu()
    {
        gameOverMenu.SetActive(true);
        pauseMenu.SetActive(false);
        mainMenu.SetActive(false);
        Time.timeScale = 0f;
    }

    public void PauseMenu()
    {
        pauseMenu.SetActive(true);
        gameOverMenu.SetActive(false);
        mainMenu.SetActive(false);
        Time.timeScale = 0f;
    }

    public void StartGame()
    {
        mainMenu.SetActive(false);
        gameOverMenu.SetActive(false);
        pauseMenu.SetActive(false);
        MapSelection.SetActive(false);
        Time.timeScale = 1f;
    }

    public void Map()
    {
        mainMenu.SetActive(false);
        gameOverMenu.SetActive(false);
        pauseMenu.SetActive(false);
        MapSelection.SetActive(true); // Hiển thị MapSelection
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        mainMenu.SetActive(false);
        gameOverMenu.SetActive(false);
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }

}
