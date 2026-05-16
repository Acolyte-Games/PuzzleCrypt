using UnityEngine;
using UnityEngine.SceneManagement;

public class WinScreen : MonoBehaviour
{
    public GameObject winPanel;

    private bool hasWon = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Something entered trigger: " + other.name);

        if (other.CompareTag("Player"))
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