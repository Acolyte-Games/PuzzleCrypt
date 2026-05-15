using UnityEngine;
using System.Collections;

public class TutorialSpikes : MonoBehaviour
{
    [Header("Sprite Settings")]
    public Sprite deactivatedSprite;
    public Sprite activatedSprite;

    [Header("Timing Settings")]
    [Tooltip("How long the spikes stay retracted before popping out again (seconds).")]
    public float interval = 2f;

    [Tooltip("How long the spikes remain extended before retracting (seconds).")]
    public float activeDuration = 1f;

    private SpriteRenderer spriteRenderer;
    private SpikeTrap spikeTrap;
    private bool isActive = false;

    public bool IsActive => isActive;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spikeTrap = GetComponent<SpikeTrap>();
        SetSpikesDeactivated();
        StartCoroutine(SpikeCycle());
    }

    private IEnumerator SpikeCycle()
    {
        while (true)
        {
            yield return new WaitForSeconds(interval);
            SetSpikesActivated();
            yield return new WaitForSeconds(activeDuration);
            SetSpikesDeactivated();
        }
    }

    private void SetSpikesActivated()
    {
        isActive = true;

        if (spriteRenderer != null && activatedSprite != null)
            spriteRenderer.sprite = activatedSprite;

        // Kill any player already standing still on the spike.
        spikeTrap?.CheckForPlayerOnActivation();
    }

    private void SetSpikesDeactivated()
    {
        isActive = false;

        if (spriteRenderer != null && deactivatedSprite != null)
            spriteRenderer.sprite = deactivatedSprite;
    }
}