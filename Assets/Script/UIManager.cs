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
        mainMenu.SetActive(true);
        gameOverMenu.SetActive(false);
        pauseMenu.SetActive(false);
        MapSelection.SetActive(false);
        Time.timeScale = 1f;
    }
    public void MainMenuFromMap2()
    {
        mainMenu.SetActive(true);
        gameOverMenu.SetActive(false);
        pauseMenu.SetActive(false);
        MapSelection.SetActive(false);
        Time.timeScale = 1f;
    }

    public void MainMenuFromMap3()
    {
        mainMenu.SetActive(true);
        gameOverMenu.SetActive(false);
        pauseMenu.SetActive(false);
        MapSelection.SetActive(false);
        Time.timeScale = 1f;
    }
    public void TryAgian()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1f;
    }

    public void mapSelection()
    {
        MapSelection?.SetActive(true);
        mainMenu?.SetActive(false);
        gameOverMenu?.SetActive(false);
        pauseMenu?.SetActive(false);
        LockMap2?.SetActive(true);
        LockMap3?.SetActive(true);
        Time.timeScale = 0f;
    }
    public void mapSelectionFromMap2()
    {
        MapSelection?.SetActive(true);
        mainMenu?.SetActive(false);
        gameOverMenu?.SetActive(false);
        pauseMenu?.SetActive(false);

        LockMap2?.SetActive(false);
        buttonMap2?.SetActive(true);

        LockMap3?.SetActive(true);
        Time.timeScale = 1f;
    }
    public void mapSelectionFromMap3()
    {
        mainMenu?.SetActive(false);
        gameOverMenu?.SetActive(false);
        pauseMenu?.SetActive(false);
        MapSelection?.SetActive(true);

        buttonMap2?.SetActive(true);
        LockMap3?.SetActive(false);
        buttonMap3?.SetActive(true);

        UpdateMapSelectionUI();

        Time.timeScale = 1f;
    }
    public void UpdateMapSelectionUI()
    {
        if (LockMap2 == null || buttonMap2 == null || LockMap3 == null || buttonMap3 == null)
        {
            Debug.LogError("Một hoặc nhiều tham chiếu trong GameManager chưa được gán!");
            return;
        }
        // Cập nhật trạng thái map 2
        if (PlayerPrefs.GetInt("Map2Unlocked", 0) == 1)
        {
            LockMap2.SetActive(false);
            buttonMap2.SetActive(true);

        }
        else
        {
            LockMap2.SetActive(true);
            buttonMap2.SetActive(false);
        }

        // Cập nhật trạng thái map 3
        if (PlayerPrefs.GetInt("Map3Unlocked", 0) == 1)
        {
            LockMap3.SetActive(false);
            buttonMap3.SetActive(true);
        }
       

    }

    public void GameOverMenu()
    {
        gameOverMenu?.SetActive(true);
        pauseMenu?.SetActive(false);
        mainMenu?.SetActive(false);
        //Time.timeScale = 0f;
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

        UpdateMapSelectionUI();
        MapSelection?.SetActive(true);

        LockMap2.SetActive(true);
        buttonMap2.SetActive(false);
        LockMap3?.SetActive(true);

        //Time.timeScale = 0f;
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

        buttonMap3?.SetActive(false);
        LockMap3?.SetActive(true);
       
            PlayerPrefs.SetInt("Map2Unlocked", 1);
            PlayerPrefs.Save();
   
    }

    public void UnlockMap3()
    {
        buttonMap3?.SetActive(true);
        LockMap3?.SetActive(false);
        buttonMap2?.SetActive(true);
       
            PlayerPrefs.SetInt("Map3Unlocked", 1);
            PlayerPrefs.Save();
      

    }

    public void loadMap2()
    {

        SceneManager.LoadScene("Map2");
    }

    public void loadMap3()
    {

        SceneManager.LoadScene("Map3");
    }


}
