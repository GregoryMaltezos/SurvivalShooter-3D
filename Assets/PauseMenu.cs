using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public Button resumeButton;
    public Button quitButton;
    public Button restartButton;
    private bool isPaused = false;

    private void Start()
    {
        // Ensure the pause menu is hidden when the game starts
        pauseMenuUI.SetActive(false);

        // Add click event listeners to the buttons
        resumeButton.onClick.AddListener(ResumeGame);
        quitButton.onClick.AddListener(QuitGame);
        restartButton.onClick.AddListener(RestartGame);

        // Lock the cursor when the game starts
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f; // Unpause the game
        pauseMenuUI.SetActive(false);

        // Call the method to handle cursor visibility
        SetCursorState(false);
    }

    private void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f; // Pause the game
        pauseMenuUI.SetActive(true);

        // Call the method to handle cursor visibility
        SetCursorState(true);
    }

    public void RestartGame()
    {
        // Set isPaused to false before reloading the scene
        isPaused = false;

        // Unpause the game and set the time scale to normal before reloading the scene
        Time.timeScale = 1f;

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);

        // Ensure the pause menu is hidden when the game restarts
        pauseMenuUI.SetActive(false);

        // Call the method to handle cursor visibility
        SetCursorState(false);
    }

    public bool IsPaused()
    {
        return isPaused;
    }

    public void QuitGame()
    {
        // You can replace this with any quit functionality you prefer, such as quitting the application or returning to the main menu.
        Application.Quit();
    }

    private void SetCursorState(bool isVisible)
    {
        Cursor.lockState = isVisible ? CursorLockMode.Confined : CursorLockMode.Locked;
        Cursor.visible = isVisible;
    }
}
