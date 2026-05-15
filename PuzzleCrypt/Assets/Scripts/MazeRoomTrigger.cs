using UnityEngine;

/// <summary>
/// Attach this to a trigger collider at the entrance of the maze room.
/// It registers/unregisters the MazeTimer with DeathManager so deaths
/// inside the room automatically reset the puzzle.
///
/// Setup:
///   1. Create an empty GameObject at the room entrance.
///   2. Add a BoxCollider2D, check "Is Trigger".
///   3. Attach this script and assign the MazeTimer reference in the Inspector.
///   4. Make sure your Player GameObject is tagged "Player".
/// </summary>
public class MazeRoomTrigger : MonoBehaviour
{
    [Tooltip("The MazeTimer for this room.")]
    public MazeTimer mazeTimer;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        DeathManager.RegisterMazeTimer(mazeTimer);
        Debug.Log("Player entered maze room — MazeTimer registered.");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        DeathManager.UnregisterMazeTimer();
        Debug.Log("Player left maze room — MazeTimer unregistered.");
    }

    private void OnValidate()
    {
        if (mazeTimer == null)
            Debug.LogWarning($"[MazeRoomTrigger] No MazeTimer assigned on '{gameObject.name}'.", this);

        Collider2D col = GetComponent<Collider2D>();
        if (col == null)
            Debug.LogWarning($"[MazeRoomTrigger] No Collider2D found on '{gameObject.name}'. Add one and enable Is Trigger.", this);
        else if (!col.isTrigger)
            Debug.LogWarning($"[MazeRoomTrigger] Collider2D on '{gameObject.name}' is not set to Is Trigger.", this);
    }
}
