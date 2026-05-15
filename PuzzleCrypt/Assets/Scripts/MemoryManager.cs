using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Central manager for the 9-pad memory puzzle.
///
/// ── How it works ──────────────────────────────────────────────────────────
///
///  IDLE          Waiting for the lever (MemorySwitch) to be pulled.
///
///  SHOWING       Playing back the current sequence to the player.
///                Pads light up one at a time (spriteHint), then go idle.
///                After the full sequence is shown, transitions to WAITING.
///
///  WAITING       Player must step on the pads IN THE DISPLAYED ORDER.
///                • Correct pad  → spritePressed, advance to next step.
///                                 If all steps done → next round (or WIN).
///                • Wrong pad    → FAIL sequence.
///
///  FAIL          All pads flash spriteWrong briefly, puzzle resets,
///                lever unflips, back to IDLE.
///
///  VICTORY       All pads flash spriteVictory three times, then
///                PuzzleSpikeTrap.OnPuzzleSolved() is called.
///                State locks at SOLVED; lever stays flipped.
///
/// ── Sequence generation ───────────────────────────────────────────────────
///   Round 1 → 1 pad shown
///   Round 2 → same pad + 1 new pad (2 shown)
///   …
///   Round 5 → 5 pads shown  → WIN on correct input
///
/// </summary>
public class PressurePadPuzzle : MonoBehaviour
{
    // ── Inspector ─────────────────────────────────────────────────────────────

    [Header("Pads (assign all 9 in order 0–8)")]
    public PressurePad[] pads;          // length must be 9

    [Header("Spike Traps to Disable on Win")]
    [Tooltip("Assign all PuzzleSpikeTrap components surrounding the treasure.")]
    public PuzzleSpikeTrap[] spikeTraps;

    [Header("Sequence Settings")]
    [Tooltip("How many rounds the player must complete (default 5).")]
    public int totalRounds = 5;

    [Header("Timing (seconds)")]
    [Tooltip("How long each pad stays lit during the hint sequence.")]
    public float hintOnDuration = 0.6f;

    [Tooltip("Gap between pads during the hint sequence.")]
    public float hintOffDuration = 0.25f;

    [Tooltip("Pause after the full sequence is shown, before player input.")]
    public float postShowDelay = 0.4f;

    [Tooltip("How long the wrong-pad flash lasts on failure.")]
    public float failFlashDuration = 0.8f;

    [Tooltip("How long each victory flash on/off lasts.")]
    public float victoryFlashTime = 0.25f;

    [Tooltip("How many times pads flash on victory.")]
    public int victoryFlashCount = 3;

    // ── Public state ─────────────────────────────────────────────────────────
    public bool IsSolved { get; private set; }

    // ── Private ───────────────────────────────────────────────────────────────

    private enum State { Idle, Showing, Waiting, Fail, Victory, Solved }
    private State state = State.Idle;

    private List<int> sequence = new List<int>();   // full growing sequence
    private int inputStep = 0;                 // how far the player is through input
    private int currentRound = 0;                 // 1-based
    private MemorySwitch activatingSwitch;

    // ─────────────────────────────────────────────────────────────────────────
    //  Public API
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>Called by MemorySwitch when the lever is pulled.</summary>
    public void BeginPuzzle(MemorySwitch source)
    {
        if (state != State.Idle) return;

        activatingSwitch = source;
        sequence.Clear();
        currentRound = 0;

        Debug.Log("[PressurePadPuzzle] Puzzle started.");
        StartCoroutine(AdvanceRound());
    }

    /// <summary>Called by each PressurePad when the player steps on it.</summary>
    public void OnPadStepped(int padIndex)
    {
        if (state != State.Waiting) return;

        if (padIndex == sequence[inputStep])
        {
            // ── Correct ──────────────────────────────────────────────────────
            pads[padIndex].SetPressed();
            inputStep++;

            if (inputStep >= sequence.Count)
            {
                // Completed this round's sequence
                if (currentRound >= totalRounds)
                    StartCoroutine(VictorySequence());
                else
                    StartCoroutine(AdvanceRound());
            }
        }
        else
        {
            // ── Wrong ────────────────────────────────────────────────────────
            Debug.Log($"[PressurePadPuzzle] Wrong pad ({padIndex}), expected {sequence[inputStep]}. Failing.");
            StartCoroutine(FailSequence());
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  Coroutines
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>Adds one new pad to the sequence then plays it back to the player.</summary>
    private IEnumerator AdvanceRound()
    {
        state = State.Showing;
        currentRound++;

        // Add a new random pad (can repeat — just like Simon Says)
        int next = Random.Range(0, pads.Length);
        sequence.Add(next);

        // Brief pause before showing
        yield return new WaitForSeconds(0.3f);

        Debug.Log($"[PressurePadPuzzle] Round {currentRound}/{totalRounds} — showing sequence of {sequence.Count}.");

        // Reset all pads to idle before showing
        foreach (var p in pads) p.SetIdle();

        // Play the sequence
        for (int i = 0; i < sequence.Count; i++)
        {
            PressurePad pad = pads[sequence[i]];
            pad.SetHint();
            yield return new WaitForSeconds(hintOnDuration);
            pad.SetIdle();
            yield return new WaitForSeconds(hintOffDuration);
        }

        yield return new WaitForSeconds(postShowDelay);

        // Now wait for player input
        inputStep = 0;
        state = State.Waiting;
        Debug.Log("[PressurePadPuzzle] Waiting for player input.");
    }

    private IEnumerator FailSequence()
    {
        state = State.Fail;

        // Flash all pads with spriteWrong
        foreach (var p in pads) p.SetWrong();
        yield return new WaitForSeconds(failFlashDuration);
        foreach (var p in pads) p.SetIdle();

        Debug.Log("[PressurePadPuzzle] Puzzle failed. Resetting.");

        // Reset puzzle state
        sequence.Clear();
        currentRound = 0;
        inputStep = 0;

        // Reset the lever so the player can try again
        if (activatingSwitch != null)
            activatingSwitch.ResetLever();

        state = State.Idle;
    }

    private IEnumerator VictorySequence()
    {
        state = State.Victory;
        Debug.Log("[PressurePadPuzzle] PUZZLE SOLVED! Playing victory effect.");

        // Flash all pads victoryFlashCount times
        for (int i = 0; i < victoryFlashCount; i++)
        {
            foreach (var p in pads) p.SetVictory();
            yield return new WaitForSeconds(victoryFlashTime);
            foreach (var p in pads) p.SetIdle();
            yield return new WaitForSeconds(victoryFlashTime);
        }

        // Final state: all pads stay in pressed/victory
        foreach (var p in pads) p.SetVictory();

        // Solve the puzzle — drop all spikes permanently
        IsSolved = true;
        state = State.Solved;

        foreach (var trap in spikeTraps)
        {
            if (trap != null)
                trap.OnPuzzleSolved();
        }

        Debug.Log("[PressurePadPuzzle] All spikes deactivated. Treasure is accessible.");
    }
}