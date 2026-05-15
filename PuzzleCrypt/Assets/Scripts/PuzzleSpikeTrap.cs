using System.Collections;
using UnityEngine;

/// <summary>
/// Companion component to SpikeTrap for the memory-puzzle treasure room.
/// Attach BOTH SpikeTrap and PuzzleSpikeTrap to the same spike GameObject.
///
/// ── Behaviour ────────────────────────────────────────────────────────────
///
///   On start   : Spike is UP (active, lethal). The sprite should show the
///                extended spikes. SpikeTrap.TryKill checks IsActive on this
///                component, so killing works automatically.
///
///   On solved  : PressurePadPuzzle calls OnPuzzleSolved().
///                • IsActive → false  (SpikeTrap will no longer kill)
///                • Plays retract animation (sprite swap + optional coroutine)
///                • Disables the Collider2D so no further trigger events fire
///                • State is PERMANENT — no way to re-activate
///
/// ── Integration with SpikeTrap ────────────────────────────────────────────
///   SpikeTrap.TryKill() checks:
///       if (mazeSpikeTrap != null && !mazeSpikeTrap.IsActive) return;
///   PuzzleSpikeTrap exposes the same IsActive property so SpikeTrap
///   can reference it via the same pattern — just add a field to SpikeTrap:
///
///       private PuzzleSpikeTrap puzzleSpikeTrap;
///       // in Start():  puzzleSpikeTrap = GetComponent<PuzzleSpikeTrap>();
///       // in TryKill(): if (puzzleSpikeTrap != null && !puzzleSpikeTrap.IsActive) return;
///
///   See the bottom of this file for the exact SpikeTrap diff to apply.
/// </summary>
[RequireComponent(typeof(SpikeTrap))]
public class PuzzleSpikeTrap : MonoBehaviour
{
    // ── Public state ─────────────────────────────────────────────────────────

    /// <summary>
    /// True  = spikes are up and lethal.
    /// False = spikes are retracted and permanently safe.
    /// SpikeTrap reads this via its TryKill() guard.
    /// </summary>
    public bool IsActive { get; private set; } = true;

    // ── Inspector ─────────────────────────────────────────────────────────────

    [Header("Sprites")]
    [Tooltip("Sprite displayed while spikes are extended (dangerous).")]
    public Sprite spriteUp;

    [Tooltip("Sprite displayed after spikes retract (safe).")]
    public Sprite spriteDown;

    [Header("Retract Animation")]
    [Tooltip("If true, the sprite swap is preceded by a short shake effect.")]
    public bool playRetractShake = true;

    [Tooltip("Duration of the shake before the sprite swaps.")]
    public float shakeDuration = 0.35f;

    [Tooltip("Magnitude of the shake offset in world units.")]
    public float shakeMagnitude = 0.06f;

    [Header("Audio")]
    [Tooltip("Optional sound played when spikes retract.")]
    public AudioClip retractSound;

    // ── Private ───────────────────────────────────────────────────────────────

    private SpriteRenderer sr;
    private Collider2D      col;
    private Vector3         originPos;

    // ─────────────────────────────────────────────────────────────────────────
    void Awake()
    {
        sr        = GetComponent<SpriteRenderer>();
        col       = GetComponent<Collider2D>();
        originPos = transform.localPosition;

        // Make sure the spike starts in the "up" state visually.
        if (sr != null && spriteUp != null)
            sr.sprite = spriteUp;
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  Public API
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Called by PressurePadPuzzle.VictorySequence() when the puzzle is solved.
    /// Permanently deactivates this spike trap.
    /// </summary>
    public void OnPuzzleSolved()
    {
        if (!IsActive) return;  // already retracted — nothing to do
        StartCoroutine(RetractSpike());
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  Internals
    // ─────────────────────────────────────────────────────────────────────────

    private IEnumerator RetractSpike()
    {
        // 1. Disable killing FIRST so the player can safely walk over during animation.
        IsActive = false;

        // 2. Optional shake
        if (playRetractShake)
            yield return StartCoroutine(ShakeRoutine());

        // 3. Swap to retracted sprite
        if (sr != null && spriteDown != null)
            sr.sprite = spriteDown;

        // 4. Play sound
        if (retractSound != null)
            AudioSource.PlayClipAtPoint(retractSound, transform.position);

        // 5. Disable collider permanently so no trigger callbacks fire at all
        if (col != null)
            col.enabled = false;

        // Restore position in case shake drifted it
        transform.localPosition = originPos;

        Debug.Log($"[PuzzleSpikeTrap] '{gameObject.name}' retracted permanently.");
    }

    private IEnumerator ShakeRoutine()
    {
        float elapsed = 0f;
        while (elapsed < shakeDuration)
        {
            float offsetX = Random.Range(-shakeMagnitude, shakeMagnitude);
            float offsetY = Random.Range(-shakeMagnitude, shakeMagnitude);
            transform.localPosition = originPos + new Vector3(offsetX, offsetY, 0f);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = originPos;
    }
}


// =============================================================================
//  REQUIRED CHANGE IN SpikeTrap.cs
// =============================================================================
//
//  1. Add a new private field alongside the existing ones:
//
//       private PuzzleSpikeTrap puzzleSpikeTrap;
//
//  2. In Start(), add:
//
//       puzzleSpikeTrap = GetComponent<PuzzleSpikeTrap>();
//
//  3. In TryKill(), add a guard alongside the existing checks:
//
//       private void TryKill(GameObject player)
//       {
//           if (tutorialSpikes  != null && !tutorialSpikes.IsActive)  return;
//           if (mazeSpikeTrap   != null && !mazeSpikeTrap.IsActive)   return;
//           if (puzzleSpikeTrap != null && !puzzleSpikeTrap.IsActive) return;  // <-- ADD THIS
//           KillPlayer(player);
//       }
//
//  That's it. No other changes to SpikeTrap.cs are needed.
// =============================================================================
