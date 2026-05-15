using UnityEngine;

/// <summary>
/// SpikeTrap — unchanged public API, with PuzzleSpikeTrap guard added to TryKill().
/// See the comment block at the bottom of PuzzleSpikeTrap.cs for the three-line diff
/// if you prefer to patch your existing file manually.
/// </summary>
public class SpikeTrap : MonoBehaviour
{
    [Header("Death Effects")]
    public GameObject deathEffect;
    public AudioClip deathSound;

    private TutorialSpikes tutorialSpikes;
    private MazeSpikeTrap mazeSpikeTrap;
    private PuzzleSpikeTrap puzzleSpikeTrap;   // ← NEW
    private Collider2D col;

    void Start()
    {
        tutorialSpikes = GetComponent<TutorialSpikes>();
        mazeSpikeTrap = GetComponent<MazeSpikeTrap>();
        puzzleSpikeTrap = GetComponent<PuzzleSpikeTrap>();  // ← NEW
        col = GetComponent<Collider2D>();
    }

    // Fires instantly when the player walks onto an active spike.
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        TryKill(other.gameObject);
    }

    // Backup for slow/sliding movement that might not fire Enter reliably.
    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        TryKill(other.gameObject);
    }

    /// <summary>
    /// Call this from any spike variant the moment the spike becomes active.
    /// Handles the case where the player is already standing still on the spike
    /// when it extends — Unity's OnTriggerStay2D won't fire reliably then.
    /// </summary>
    public void CheckForPlayerOnActivation()
    {
        if (col == null) return;

        Collider2D[] hits = Physics2D.OverlapBoxAll(
            col.bounds.center,
            col.bounds.size,
            0f
        );

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                Debug.Log($"[SpikeTrap] Spike on '{gameObject.name}' extended under standing player.");
                KillPlayer(hit.gameObject);
                return;
            }
        }
    }

    private void TryKill(GameObject player)
    {
        if (tutorialSpikes != null && !tutorialSpikes.IsActive) return;
        if (mazeSpikeTrap != null && !mazeSpikeTrap.IsActive) return;
        if (puzzleSpikeTrap != null && !puzzleSpikeTrap.IsActive) return;  // ← NEW

        KillPlayer(player);
    }

    private void KillPlayer(GameObject player)
    {
        Debug.Log("Player hit spikes!");

        if (deathEffect != null)
            Instantiate(deathEffect, player.transform.position, Quaternion.identity);

        if (deathSound != null)
            AudioSource.PlayClipAtPoint(deathSound, player.transform.position);

        DeathManager.TriggerDeath(player);
    }
}