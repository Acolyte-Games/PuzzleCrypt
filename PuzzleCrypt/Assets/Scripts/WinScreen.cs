using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class WinScreen : MonoBehaviour
{
    public GameObject winPanel;

    private bool hasWon = false;

    public void OnWinTest(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            ShowWinScreen();
        }
    }

    public void ShowWinScreen()
    {
        if (hasWon) return;

        winPanel.SetActive(true);
        Time.timeScale = 0f;
        hasWon = true;
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