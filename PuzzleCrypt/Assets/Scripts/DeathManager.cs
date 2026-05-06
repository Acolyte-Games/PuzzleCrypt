using UnityEngine;
using System;

public class DeathManager : MonoBehaviour
{
    private static DeathManager instance;

    public static event Action OnPlayerDeath;
    public static event Action OnPlayerRespawn;

    private static GameObject currentPlayer;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static void TriggerDeath(GameObject player)
    {
        currentPlayer = player;

        DisablePlayer(player);

        OnPlayerDeath?.Invoke();

        Debug.Log("Player died! Waiting for menu choice...");
    }

    public static void RespawnPlayer()
    {
        if (currentPlayer != null)
        {
            Vector3 respawnPosition = RespawnManager.GetRespawnPosition();

            currentPlayer.transform.position = respawnPosition;

            EnablePlayer(currentPlayer);

            OnPlayerRespawn?.Invoke();

            Debug.Log($"Player respawned at: {respawnPosition}");
        }
    }

    public static void QuitGame()
    {
        Debug.Log("Player chose to quit");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    private static void DisablePlayer(GameObject player)
    {
        SpriteRenderer spriteRenderer = player.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) spriteRenderer.enabled = false;

        Collider2D col = player.GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
        }

        MonoBehaviour[] scripts = player.GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            if (script != null && script.enabled)
            {
                script.enabled = false;
            }
        }
    }

    private static void EnablePlayer(GameObject player)
    {
        SpriteRenderer spriteRenderer = player.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) spriteRenderer.enabled = true;

        Collider2D col = player.GetComponent<Collider2D>();
        if (col != null) col.enabled = true;

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null) rb.simulated = true;

        MonoBehaviour[] scripts = player.GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            if (script != null)
            {
                script.enabled = true;
            }
        }
    }
}