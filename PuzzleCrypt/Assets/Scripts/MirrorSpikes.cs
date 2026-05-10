using UnityEngine;
using System.Collections;

public class ProximitySpikes : MonoBehaviour
{
    [Header("Sprite Settings")]
    public Sprite deactivatedSprite;
    public Sprite activatedSprite;

    [Header("Timing Settings")]
    public float resetDelay = 2f;

    private SpriteRenderer spriteRenderer;
    private Collider2D spikeCollider;
    private bool isResetting = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spikeCollider = GetComponent<Collider2D>();

        // Start deactivated
        SetSpikesDeactivated();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isResetting)
        {
            StartCoroutine(ActivateSpikes());
        }
    }

    private IEnumerator ActivateSpikes()
    {
        // Switch to activated sprite
        if (spriteRenderer != null && activatedSprite != null)
        {
            spriteRenderer.sprite = activatedSprite;
        }

        // Player death is handled by SpikeTrap.cs on the same GameObject

        // Wait for reset delay
        yield return new WaitForSeconds(resetDelay);

        // Reset back to deactivated
        SetSpikesDeactivated();
    }

    private void SetSpikesDeactivated()
    {
        isResetting = false;

        if (spriteRenderer != null && deactivatedSprite != null)
        {
            spriteRenderer.sprite = deactivatedSprite;
        }
    }
}