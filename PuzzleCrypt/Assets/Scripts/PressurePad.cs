using UnityEngine;
using System.Collections;

/// <summary>
/// One of the nine pressure pads in the memory puzzle room.
/// Handles its own visual state and reports player contact to PressurePadPuzzle.
///
/// Sprite states
///   spriteIdle    – default, unlit
///   spriteHint    – lit up during the "watch the sequence" phase
///   spritePressed – player has successfully stepped on this pad this round
///   spriteWrong   – brief flash on failure
///   spriteVictory – flashing during the win celebration
///
/// The puzzle manager drives all state changes via the public API below;
/// this MonoBehaviour only handles physics detection and rendering.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class PressurePad : MonoBehaviour
{
    // ── Inspector ─────────────────────────────────────────────────────────────
    [Header("Identity")]
    [Tooltip("0-based index. Assign 0–8 for the nine pads.")]
    public int padIndex;

    [Header("Sprites")]
    public Sprite spriteIdle;
    public Sprite spriteHint;     // lit during sequence display
    public Sprite spritePressed;  // correct step confirmed
    public Sprite spriteWrong;    // failure flash
    public Sprite spriteVictory;  // win flash

    [Header("References")]
    public PressurePadPuzzle puzzle;

    // ── Private ───────────────────────────────────────────────────────────────
    private SpriteRenderer sr;
    private bool playerIsOn = false;

    // Prevents immediate retrigger after hint display
    private bool inputLocked = false;
    private const float hintReleaseDelay = 0.15f;

    // ─────────────────────────────────────────────────────────────────────────
    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();

        // Ensure the collider is a trigger so we get OnTrigger* callbacks.
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;

        SetIdle();
    }

    // ── Visual state API (called by PressurePadPuzzle) ────────────────────────

    public void SetIdle()
    {
        ApplySprite(spriteIdle);

        // Brief delay prevents hint flicker under player's feet
        if (gameObject.activeInHierarchy)
            StartCoroutine(UnlockAfterDelay());
    }

    public void SetHint() => ApplySprite(spriteHint);
    public void SetPressed() => ApplySprite(spritePressed);
    public void SetWrong() => ApplySprite(spriteWrong);
    public void SetVictory() => ApplySprite(spriteVictory);

    private void ApplySprite(Sprite s)
    {
        if (sr != null && s != null)
            sr.sprite = s;
    }

    // ── Delayed Unlock ────────────────────────────────────────────────────────

    private IEnumerator UnlockAfterDelay()
    {
        inputLocked = true;

        yield return new WaitForSeconds(hintReleaseDelay);

        inputLocked = false;

        // If player is already standing on the pad when unlock happens,
        // trigger it manually so they don't need to step off and back on.
        Collider2D player = Physics2D.OverlapBox(
            transform.position,
            GetComponent<Collider2D>().bounds.size,
            0f
        );

        if (player != null &&
            player.CompareTag("Player") &&
            !playerIsOn)
        {
            playerIsOn = true;

            if (puzzle != null)
                puzzle.OnPadStepped(padIndex);
        }
    }

    // ── Physics ───────────────────────────────────────────────────────────────

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (inputLocked) return;
        if (playerIsOn) return;

        playerIsOn = true;

        if (puzzle != null)
            puzzle.OnPadStepped(padIndex);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerIsOn = false;

        // Revert to idle sprite when player steps off
        SetIdle();
    }
}