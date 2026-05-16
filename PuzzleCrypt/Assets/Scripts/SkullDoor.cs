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
        Debug.Log($"Trying to activate {gameObject.name}");

        SkullKey keyScript = null;

        if (pairedSkull != null)
        {
            keyScript = pairedSkull.GetComponent<SkullKey>();
        }

        if (keyScript == null)
        {
            Debug.Log("No SkullKey found!");
            return;
        }

        Debug.Log($"Key collected = {keyScript.isCollected}");

        if (keyScript.isCollected)
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