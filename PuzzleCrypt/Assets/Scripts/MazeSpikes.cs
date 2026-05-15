using UnityEngine;
using System.Collections;

public class MazeSpikeTrap : MonoBehaviour
{
    [Header("Sprite Settings")]
    public Sprite deactivatedSprite;
    public Sprite activatedSprite;

    [Header("Cycle Settings")]
    [Tooltip("How long the spikes stay extended before retracting.")]
    public float activeDuration = 1.5f;

    [Header("References")]
    [Tooltip("The MazeTimer that owns this spike. Used to trigger a puzzle reset after the spike retracts.")]
    public MazeTimer mazeTimer;

    private SpriteRenderer spriteRenderer;
    private SpikeTrap spikeTrap;
    private bool isActive = false;
    private bool isCycling = false;

    public bool IsActive => isActive;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spikeTrap = GetComponent<SpikeTrap>();
        SetSprite(false);
    }

    public void StartCycling()
    {
        if (isCycling) return;
        isCycling = true;
        StartCoroutine(SpikeSequence());
    }

    public void ResetSpike()
    {
        StopAllCoroutines();
        isCycling = false;
        isActive = false;
        SetSprite(false);
    }

    private IEnumerator SpikeSequence()
    {
        // Extend
        isActive = true;
        SetSprite(true);

        // Delegate the stationary-player check to SpikeTrap so the fix
        // lives in one place for all spike variants.
        spikeTrap?.CheckForPlayerOnActivation();

        yield return new WaitForSeconds(activeDuration);

        // Retract
        isActive = false;
        SetSprite(false);
        isCycling = false;

        if (mazeTimer != null)
            mazeTimer.ResetPuzzle();
        else
            Debug.LogWarning($"[MazeSpikeTrap] No MazeTimer assigned on '{gameObject.name}' — puzzle won't auto-reset.");
    }

    private void SetSprite(bool active)
    {
        if (spriteRenderer == null) return;

        if (active && activatedSprite != null)
            spriteRenderer.sprite = activatedSprite;
        else if (!active && deactivatedSprite != null)
            spriteRenderer.sprite = deactivatedSprite;
    }
}