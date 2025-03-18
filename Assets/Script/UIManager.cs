using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;

    private int score = 0;

    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject gameOverMenu;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject MapSelection;
    public GameObject InfoButton;
    public GameObject guidePanel;

    [SerializeField] private GameObject buttonMap2;
    [SerializeField] private GameObject buttonMap3;
    [SerializeField] private GameObject LockMap2;
    [SerializeField] private GameObject LockMap3;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseMenu();
        }
    }

    public void ToggleGuidePanel()
    {
        bool isActive = !guidePanel.activeSelf;
        guidePanel.SetActive(isActive);
    }

    public void AddScore(int point)
    {
        score += point;
    }

    public void MainMenu()
    {
       SceneManager.LoadScene(0);
    }
    public void TryAgian()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void mapSelection()
    {
        MapSelection?.SetActive(true);
        mainMenu?.SetActive(false);
        gameOverMenu?.SetActive(false);
        pauseMenu?.SetActive(false);
        Time.timeScale = 0f;
    }

    public void GameOverMenu()
    {
        gameOverMenu?.SetActive(true);
        pauseMenu?.SetActive(false);
        mainMenu?.SetActive(false);
        Time.timeScale = 0f;
    }

    public void PauseMenu()
    {
        pauseMenu?.SetActive(true);
        gameOverMenu?.SetActive(false);
        mainMenu?.SetActive(false);
        Time.timeScale = 0f;
    }

    public void StartGame()
    {
        mainMenu?.SetActive(false);
        gameOverMenu?.SetActive(false);
        pauseMenu?.SetActive(false);
        MapSelection?.SetActive(false);
        Time.timeScale = 1f;
    }

    public void Map()
    {
        mainMenu?.SetActive(false);
        gameOverMenu?.SetActive(false);
        pauseMenu?.SetActive(false);
        MapSelection?.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        mainMenu?.SetActive(false);
        gameOverMenu?.SetActive(false);
        pauseMenu?.SetActive(false);
        Time.timeScale = 1f;
    }

    public void UnlockMap2()
    {
        buttonMap2?.SetActive(true);
        LockMap2?.SetActive(false);
    }

    public void UnlockMap3()
    {
        buttonMap3?.SetActive(true);
        LockMap3?.SetActive(false);
    }


}
