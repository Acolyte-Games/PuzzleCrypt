using UnityEngine;

public class SkullKey : MonoBehaviour, IInteractable
{
    public bool isCollected { get; private set; }
    public string SkullID;
    public Sprite collectedSprite;

    [Header("Door Reference")]
    public SkullDoor connectedDoor; // Add reference to the door this key unlocks

    private void Start()
    {
        if (SaveManager.IsSkullKeyCollected(SkullID))
        {
            setCollected(true);
        }
    }

    public bool CanInteract()
    {
        return !isCollected;
    }

    public void Interact()
    {
        if (!CanInteract()) return;
        else CollectKey();
    }

    private void CollectKey()
    {
        setCollected(true);
        SaveManager.SaveSkullKey(SkullID);
    }

    public void setCollected(bool collected)
    {
        if (isCollected == collected) return; // Early exit if already in this state

        isCollected = collected;

        if (isCollected)
        {
            GetComponent<SpriteRenderer>().sprite = collectedSprite;

            // Notify the connected door
            if (connectedDoor != null)
            {
                connectedDoor.TryActivate();
            }
        }
    }
}