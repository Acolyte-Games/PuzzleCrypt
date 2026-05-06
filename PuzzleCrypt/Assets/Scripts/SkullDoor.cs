using UnityEngine;

public class SkullDoor : MonoBehaviour
{
    public bool isActive { get; private set; }
    public Sprite activeSprite;
    public GameObject pairedSkull; // Alternative if you prefer this approach

    private void Start()
    {
        isActive = false;
    }

    public void TryActivate()
    {
        SkullKey keyScript = null;

        // Check both possible ways of getting the key reference
        if (pairedSkull != null)
        {
            keyScript = pairedSkull.GetComponent<SkullKey>();
        }

        if (keyScript != null && keyScript.isCollected)
        {
            SetActive(true);
        }
    }

    public void SetActive(bool active)
    {
        if (isActive == active) return; // Early exit if already in this state

        isActive = active;

        if (isActive)
        {
            GetComponent<SpriteRenderer>().sprite = activeSprite;
        }
    }
}