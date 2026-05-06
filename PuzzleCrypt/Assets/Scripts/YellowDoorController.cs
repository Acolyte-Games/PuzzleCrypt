using UnityEngine;

public class YellowSwitchDoorController : MonoBehaviour
{
    [Header("Door Settings")]
    public YellowSwitch[] requiredSwitches;
    public bool destroyDoor = true;

    [Header("Optional Effects")]
    public GameObject destructionEffect;
    public AudioClip destructionSound;

    private bool doorOpened = false;

    private void Start()
    {
        if (requiredSwitches == null || requiredSwitches.Length == 0)
        {
            Debug.LogWarning($"No switches assigned to {gameObject.name}. The door will never open.");
            return;
        }
    }

    private void Update()
    {
        if (doorOpened) return;

        if (CheckAllSwitchesActivated())
        {
            OpenDoor();
        }
    }

    private bool CheckAllSwitchesActivated()
    {
        foreach (YellowSwitch switchObj in requiredSwitches)
        {
            if (switchObj == null || !switchObj.isActive)
            {
                return false;
            }
        }

        return true;
    }

    private void OpenDoor()
    {
        doorOpened = true;
        Debug.Log($"All {requiredSwitches.Length} yellow switches activated! Opening door: {gameObject.name}");

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