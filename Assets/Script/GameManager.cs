using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

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
    public static GameManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.Log("Xóa GameManager trùng lặp!");
            Destroy(gameObject);
        }
    }


    void Start()
    {
        /**
         * thêm cái này trước khi build game để xóa data lưu map
         *  1. Thâm cái này save + unity chạy game + tắt game
         *  2. Build game
         *   PlayerPrefs.DeleteKey("Map2Unlocked");
        PlayerPrefs.DeleteKey("Map3Unlocked");
        PlayerPrefs.Save();
        Debug.Log("Đã xóa dữ liệu mở khóa map");
         * 
         * 
         */


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
        else
        {
            LockMap3.SetActive(true);
            buttonMap3.SetActive(false);
        }

        //LockMap2?.SetActive(true);
        //LockMap3?.SetActive(true);
        MainMenu();
        Time.timeScale = 1f;
    }
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
        mainMenu?.SetActive(true);
        gameOverMenu?.SetActive(false);
        pauseMenu?.SetActive(false);
        MapSelection.SetActive(false);
        Time.timeScale = 0;
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

  
   

   

    public void OnPauseButtonClick()
    {
        Scene gameUIScene = SceneManager.GetSceneByName("GameUI");

        if (!gameUIScene.isLoaded)
        {
            Debug.Log("GameUI chưa load, đang load Scene...");
            SceneManager.LoadScene("GameUI", LoadSceneMode.Additive);
            StartCoroutine(WaitForSceneAndPause()); // Chờ Scene load xong rồi mới gọi PauseMenu
        }
        else
        {
            Debug.Log("GameUI đã được load! Gọi PauseMenu()");
            PauseMenu(); // Nếu Scene đã load sẵn thì gọi luôn
        }
    }



    private System.Collections.IEnumerator WaitForSceneAndPause()
    {
        yield return new WaitUntil(() => SceneManager.GetSceneByName("GameUI").isLoaded); // Đợi Scene load

        GameManager gameManager = FindObjectOfType<GameManager>();

        if (gameManager != null)
        {
            Debug.Log("GameManager đã được tìm thấy, tìm PauseMenu...");
            gameManager.pauseMenu = GameObject.Find("PauseGameMenu"); // Gán lại PauseMenu nếu bị null

            if (gameManager.pauseMenu != null)
            {
                Debug.Log("PauseMenu đã được tìm thấy! Hiển thị menu.");
                gameManager.PauseMenu();
            }
            else
            {
                Debug.LogError("Không tìm thấy PauseMenu trong GameUI!");
            }
        }
        else
        {
            Debug.LogError("Không tìm thấy GameManager trong GameUI!");
        }
    }


   


}
