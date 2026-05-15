using UnityEngine;
using System;

public class DeathManager : MonoBehaviour
{
    private static DeathManager instance;

    public static event Action OnPlayerDeath;
    public static event Action OnPlayerRespawn;

    private static GameObject currentPlayer;

    // Tracks the active MazeTimer so it can be reset on death.
    // Assign via RegisterMazeTimer() when the player enters a maze room,
    // and clear it with UnregisterMazeTimer() when they leave.
    private static MazeTimer activeMazeTimer;

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

    /// <summary>
    /// Call this when the player enters a maze room so DeathManager knows
    /// which MazeTimer to reset on death.
    /// </summary>
    public static void RegisterMazeTimer(MazeTimer timer)
    {
        activeMazeTimer = timer;
    }

    /// <summary>
    /// Call this when the player leaves a maze room to stop DeathManager
    /// from resetting a timer that is no longer relevant.
    /// </summary>
    public static void UnregisterMazeTimer()
    {
        activeMazeTimer = null;
    }

    public static void TriggerDeath(GameObject player)
    {
        currentPlayer = player;

        DisablePlayer(player);

        // Reset the active maze puzzle (stops the countdown, resets levers
        // and spikes) before firing the general death event.
        if (activeMazeTimer != null)
        {
            activeMazeTimer.ResetPuzzle();
            Debug.Log("Maze puzzle reset due to player death.");
        }

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