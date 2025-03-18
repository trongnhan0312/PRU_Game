using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameUI : MonoBehaviour
{
    private GameManager gameManager;

    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    public void StartGame()
    {
        LoadMap1(); // Khi bấm Play, mở MapSelection
    }

    public void LoadMap(string mapName)
    {
        StartCoroutine(LoadMapAsync(mapName));
    }

    private IEnumerator LoadMapAsync(string mapName)
    {
        if (!SceneManager.GetSceneByName("GameUI").isLoaded)
        {
            SceneManager.LoadScene("GameUI", LoadSceneMode.Additive);
            yield return new WaitForSeconds(0.5f); // Chờ load UI trước
        }

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(mapName, LoadSceneMode.Single);
        asyncLoad.allowSceneActivation = false;

        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }

        asyncLoad.allowSceneActivation = true;

        if (gameManager != null)
        {
            gameManager.StartGame();
        }
    }

    public void LoadMap1()
    {
        LoadMap("Map1");
    }
    public void LoadMap2()
    {
        LoadMap("Map2");
    }
    public void LoadMap3()
    {
        LoadMap("Map3");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ContinueGame()
    {
        gameManager?.ResumeGame();
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void MapSelection()
    {
        gameManager?.Map();
    }
}
