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
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(mapName, LoadSceneMode.Single); // Load map mới, đóng scene cũ

        asyncLoad.allowSceneActivation = false; // Ngăn scene tự động kích hoạt khi chưa load xong

        while (asyncLoad.progress < 0.9f) // Load đến 90% trước
        {
            yield return null;
        }

        asyncLoad.allowSceneActivation = true; // Sau khi load xong mới kích hoạt scene mới

        gameManager.StartGame(); // Khi scene hoàn toàn tải xong, bắt đầu game
    }


    public void LoadMap2()
    {
        LoadMap("Map2"); // Load map 2
    }

    public void LoadMap3()
    {
        LoadMap("Map3"); // Load map 3
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ContinueGame()
    {
        gameManager.ResumeGame();
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void MapSelection()
    {
        gameManager.Map();
    }
}
