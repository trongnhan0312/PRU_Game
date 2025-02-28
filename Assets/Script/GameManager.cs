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


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateScore();
        MainMenu();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ToggleGuidePanel()
    {
        bool isActive = !guidePanel.activeSelf;
        guidePanel.SetActive(isActive);
        //foreach (GameObject obj in objectsToDisable)
        //{
        //    obj.SetActive(!isActive);
        //}
    }
    public void AddScore(int point)
    {
        score += point;
        UpdateScore();
    }

    public void UpdateScore()
    {
        scoreText.text=score.ToString();
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
        gameOverMenu.SetActive(false);
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

