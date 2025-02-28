using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameUI : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    public void StartGame()
    {
        gameManager.StartGame(); // Khi bấm Play, mở MapSelection

    }


    public void LoadMap(string mapName)
    {
        StartCoroutine(LoadMapAsync(mapName));
    }

    private IEnumerator LoadMapAsync(string mapName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(mapName);

        while (!asyncLoad.isDone)
        {
            yield return null; // Chờ cho đến khi Scene load xong
        }

        gameManager.StartGame(); // Chỉ chạy khi Scene mới đã hoàn toàn tải xong
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void continueGame()
    {
        gameManager.ResumeGame();
    }

    public void mainMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void map()
    {
       gameManager.Map();
    }
}
