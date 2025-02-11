using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    private int score = 0;
    [SerializeField] private TextMeshProUGUI scoreText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateScore();
    }

    // Update is called once per frame
    void Update()
    {
        
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
}
