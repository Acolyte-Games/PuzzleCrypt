using UnityEngine;

public class YellowSwitch : MonoBehaviour, IInteractable
{
    public bool isActive { get; private set; }
    public string SwitchID;

    [Header("Maze Lever Settings")]
    [Tooltip("Assign this on all 3 levers in the maze room.")]
    public MazeTimer mazeTimer;

    [Tooltip("How many levers are required in total to stop the timer (should be 3).")]
    public int leversRequired = 3;

    private bool isFlipped = false;
    private static int leversPulled = 0;

    void OnEnable()
    {
        leversPulled = 0;
    }

    public bool CanInteract()
    {
        return !isActive;
    }

    public void Interact()
    {
        if (!CanInteract()) return;
        ActivateSwitch();
    }

    private void ActivateSwitch()
    {
        SetActive(true);
    }

    /// <summary>
    /// Called by MazeTimer.ResetPuzzle() to un-flip this lever so it can be pulled again.
    /// </summary>
    public void ResetLever()
    {
        isActive = false;
        isFlipped = false;

        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x); // restore original orientation
        transform.localScale = scale;

        leversPulled = 0;
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

            leversPulled++;
            Debug.Log($"Yellow switch activated. Levers pulled: {leversPulled}/{leversRequired}");

            if (mazeTimer != null)
            {
                if (leversPulled == 1)
                    mazeTimer.StartTimer();

                mazeTimer.RegisterLeverPulled(leversPulled, leversRequired);
            }
        }
    }
}