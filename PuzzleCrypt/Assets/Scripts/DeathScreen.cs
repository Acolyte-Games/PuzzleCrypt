using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class DeathScreen : MonoBehaviour
{
    public GameObject deathPanel;

    private bool isDead = false;

    public void OnDeathTest(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            ShowDeathScreen();
        }
    }

    public void ShowDeathScreen()
    {
        if (isDead)
            return;

        deathPanel.SetActive(true);
        Time.timeScale = 0f;
        isDead = true;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("ArtMenu");
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        Debug.Log("Quit Game");
        Application.Quit();
    }
}