using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Text scoreText; // Reference to the Text UI element for current score
    public Text highScoreText; // Reference to the Text UI element for high score

    private float timeSurvived = 0f;
    private int zombiesKilled = 0;
    private int score = 0;
    private int highScore = 0;

    // PlayerPrefs key for storing and retrieving high score
    private const string HighScoreKey = "HighScore";

    void Start()
    {
        // Load the high score from PlayerPrefs on game start
        LoadHighScore();

        // Update the high score UI
        UpdateHighScoreUI();
    }

    void Update()
    {
        UpdateTimeSurvived();
        UpdateScore();

        // Check if the current score is higher than the stored high score
        if (score > highScore)
        {
            highScore = score;
            // Update the high score UI and save the new high score
            UpdateHighScoreUI();
            SaveHighScore();
        }
    }

    void UpdateTimeSurvived()
    {
        timeSurvived += Time.deltaTime;
    }

    void UpdateScore()
    {
        int timeScore = Mathf.RoundToInt(timeSurvived);
        int zombiesScore = zombiesKilled * 100;
        score = timeScore + zombiesScore;

        scoreText.text = "Score: " + score.ToString();
    }

    public void IncrementZombiesKilled()
    {
        zombiesKilled++;
    }

    // Function to update the high score UI
    void UpdateHighScoreUI()
    {
        highScoreText.text = "High Score: " + highScore.ToString();
    }

    // Function to save the high score
    void SaveHighScore()
    {
        PlayerPrefs.SetInt(HighScoreKey, highScore);
    }

    // Function to load the high score
    void LoadHighScore()
    {
        if (PlayerPrefs.HasKey(HighScoreKey))
        {
            highScore = PlayerPrefs.GetInt(HighScoreKey);
        }
    }

    // Function to reset high score (optional)
    public void ResetHighScore()
    {
        PlayerPrefs.DeleteKey(HighScoreKey);
        highScore = 0;
        UpdateHighScoreUI();
    }
}
