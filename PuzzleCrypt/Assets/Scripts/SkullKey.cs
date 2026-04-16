using UnityEngine;

public class SkullKey : MonoBehaviour, IInteractable
{
    public bool isCollected { get; private set; }
    public string SkullID { get; private set; }
    public Sprite collectedSprite;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SkullID ??= GlobalHelper.GenerateUniqueID(gameObject);
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
        //Additional logic for when the key is collected (setting skull in front of door to collected)
    }

    public void setCollected(bool collected)
    {
        if (isCollected = collected)
        {
            GetComponent<SpriteRenderer>().sprite = collectedSprite;
        }
    }

}
