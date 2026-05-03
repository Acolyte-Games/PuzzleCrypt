using UnityEngine;

public class DoorController : MonoBehaviour
{
    [Header("Door Settings")]
    public SkullDoor[] requiredSkullDoors; // Direct reference to SkullDoor objects
    public bool destroyDoor = true;

    [Header("Optional Effects")]
    public GameObject destructionEffect;
    public AudioClip destructionSound;

    private bool doorOpened = false;

    private void Start()
    {
        if (requiredSkullDoors == null || requiredSkullDoors.Length == 0)
        {
            Debug.LogWarning($"No SkullDoors assigned to {gameObject.name}. The door will never open.");
            return;
        }
    }

    private void Update()
    {
        if (doorOpened) return;

        if (CheckAllDoorsActivated())
        {
            OpenDoor();
        }
    }

    private bool CheckAllDoorsActivated()
    {
        foreach (SkullDoor door in requiredSkullDoors)
        {
            if (door == null || !door.isActive)
            {
                return false;
            }
        }

        return true;
    }

    private void OpenDoor()
    {
        doorOpened = true;
        Debug.Log($"All {requiredSkullDoors.Length} SkullDoors activated! Opening door: {gameObject.name}");

        if (destructionEffect != null)
        {
            Instantiate(destructionEffect, transform.position, Quaternion.identity);
        }

        if (destructionSound != null)
        {
            AudioSource.PlayClipAtPoint(destructionSound, transform.position);
        }

        if (destroyDoor)
        {
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}