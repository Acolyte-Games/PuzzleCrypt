using UnityEngine;
using UnityEngine.UI;

public class DeathMenuController : MonoBehaviour
{
    [Header("Menu References")]
    public GameObject deathMenuPanel;
    public Button respawnButton;
    public Button quitButton;

    void Awake()
    {
        if (deathMenuPanel != null)
        {
            deathMenuPanel.SetActive(false);
        }
    }

    void Start()
    {
        if (deathMenuPanel != null)
        {
            deathMenuPanel.SetActive(false);
        }

        DeathManager.OnPlayerDeath += ShowDeathMenu;
        DeathManager.OnPlayerRespawn += HideDeathMenu;

        if (respawnButton != null)
        {
            respawnButton.onClick.AddListener(OnRespawnClicked);
        }

        if (quitButton != null)
        {
            quitButton.onClick.AddListener(OnQuitClicked);
        }

        Time.timeScale = 1f;
    }

    private void ShowDeathMenu()
    {
        if (deathMenuPanel != null)
        {
            deathMenuPanel.SetActive(true);
        }

        Time.timeScale = 0f;
    }

    private void HideDeathMenu()
    {
        if (deathMenuPanel != null)
        {
            deathMenuPanel.SetActive(false);
        }

        Time.timeScale = 1f;
    }

    public void OnRespawnClicked()
    {
        DeathManager.RespawnPlayer();
    }

    public void OnQuitClicked()
    {
        DeathManager.QuitGame();
    }

    void OnDestroy()
    {
        DeathManager.OnPlayerDeath -= ShowDeathMenu;
        DeathManager.OnPlayerRespawn -= HideDeathMenu;

        if (respawnButton != null)
        {
            respawnButton.onClick.RemoveListener(OnRespawnClicked);
        }

        if (quitButton != null)
        {
            quitButton.onClick.RemoveListener(OnQuitClicked);
        }

        Time.timeScale = 1f;
    }
}