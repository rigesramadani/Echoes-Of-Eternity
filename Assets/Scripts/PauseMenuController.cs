using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour {
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject mainPausePanel;
    [SerializeField] private GameObject mainMenuConfirmationPanel;
    [SerializeField] private GameObject exitConfirmationPanel;
    public static bool isGamePaused;

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (isGamePaused) {
                ResumeGame();
            } else {
                PauseGame();
            }
        }
    }

    private void PauseGame() {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isGamePaused = true;
    }

    public void ResumeGame() {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isGamePaused = false;
    }

    public void MainMenuConfirmation() {
        mainPausePanel.SetActive(false);
        mainMenuConfirmationPanel.SetActive(true);
    }

    public void ExitConfirmation() {
        mainPausePanel.SetActive(false);
        exitConfirmationPanel.SetActive(true);
    }

    public void YesMainMenuConfirmation() {
        pauseMenu.SetActive(false);
        mainMenuConfirmationPanel.SetActive(false);

        SceneManager.LoadScene("MainMenu");
        ResumeGame();
    } 

    public void NoMainMenuConfirmation() {
        mainMenuConfirmationPanel.SetActive(false);
        mainPausePanel.SetActive(true);
    }

    public void YesExitConfirmation() {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
				Application.Quit();
        #endif
    }

    public void NoExitConfirmation() {
        exitConfirmationPanel.SetActive(false);
        mainPausePanel.SetActive(true);
    }
}