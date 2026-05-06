using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    [Header("Death Effects")]
    public GameObject deathEffect;
    public AudioClip deathSound;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            KillPlayer(other.gameObject);
        }
    }

    private void KillPlayer(GameObject player)
    {
        Debug.Log("Player hit spikes!");

        if (deathEffect != null)
        {
            Instantiate(deathEffect, player.transform.position, Quaternion.identity);
        }

        if (deathSound != null)
        {
            AudioSource.PlayClipAtPoint(deathSound, player.transform.position);
        }

        DeathManager.TriggerDeath(player);
    }
}