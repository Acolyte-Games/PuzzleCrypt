using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class PauseMenu : MonoBehaviour
{
    public GameObject pausePanel;

    private bool isPaused = false;

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeGame()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("ArtMenu");
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;

#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}