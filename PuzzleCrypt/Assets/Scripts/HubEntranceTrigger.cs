using UnityEngine;

public class HubEntranceTrigger : MonoBehaviour
{
    private bool hubActivated = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hubActivated)
        {
            hubActivated = true;
            SaveManager.SetActiveRespawnPoint("Hub");
            Debug.Log("Player entered hub! Future respawns will be in hub.");
        }
    }
}