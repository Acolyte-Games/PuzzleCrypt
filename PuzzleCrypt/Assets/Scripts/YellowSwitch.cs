using UnityEngine;

public class YellowSwitch : MonoBehaviour, IInteractable
{
    public bool isActive { get; private set; }
    public string SwitchID;
    private bool isFlipped = false;

    public bool CanInteract()
    {
        return !isActive;
    }

    public void Interact()
    {
        if (!CanInteract()) return;
        else ActivateSwitch();
    }

    private void ActivateSwitch()
    {
        SetActive(true);
    }

    public void SetActive(bool active)
    {
        if (isActive == active) return;

        isActive = active;

        if (isActive && !isFlipped)
        {
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;

            isFlipped = true;
            Debug.Log("Yellow switch activated");
        }
    }
}