using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class MazeTimer : MonoBehaviour
{
    [Header("Timer Settings")]
    [Tooltip("Seconds the player has to pull all 3 levers.")]
    public float timeLimit = 30f;

    [Header("References")]
    [Tooltip("All spike GameObjects in the room. They must have a MazeSpikeTrap component.")]
    public MazeSpikeTrap[] roomSpikes;

    [Tooltip("All 3 levers in the room. Used to reset them when the puzzle resets.")]
    public YellowSwitch[] roomLevers;

    [Tooltip("The door to open when all levers are pulled in time.")]
    public GameObject treasureDoor;

    [Header("Events")]
    public UnityEvent onTimerStart;
    public UnityEvent onTimerExpired;
    public UnityEvent onSuccess;
    public UnityEvent onReset;

    private bool isRunning = false;
    private bool isComplete = false;
    private float timeRemaining;

    public float TimeRemaining => timeRemaining;
    public bool IsRunning => isRunning;

    public void StartTimer()
    {
        if (isRunning || isComplete) return;

        timeRemaining = timeLimit;
        isRunning = true;
        onTimerStart?.Invoke();
        Debug.Log("Maze timer started!");
        StartCoroutine(Countdown());
    }

    public void RegisterLeverPulled(int leversPulled, int leversRequired)
    {
        if (isComplete) return;

        if (leversPulled >= leversRequired)
        {
            CompleteSuccess();
        }
    }

    /// <summary>
    /// Call this from your DeathManager or respawn logic to reset the entire puzzle.
    /// </summary>
    public void ResetPuzzle()
    {
        StopAllCoroutines();
        isRunning = false;
        isComplete = false;
        timeRemaining = 0f;

        foreach (var spike in roomSpikes)
        {
            if (spike != null)
                spike.ResetSpike();
        }

        foreach (var lever in roomLevers)
        {
            if (lever != null)
                lever.ResetLever();
        }

        onReset?.Invoke();
        Debug.Log("Maze puzzle reset.");
    }

    private IEnumerator Countdown()
    {
        while (timeRemaining > 0f && !isComplete)
        {
            yield return null;
            timeRemaining -= Time.deltaTime;
        }

        if (!isComplete)
        {
            TimerExpired();
        }
    }

    private void TimerExpired()
    {
        isRunning = false;
        Debug.Log("Timer expired! Spikes activating!");
        onTimerExpired?.Invoke();

        foreach (var spike in roomSpikes)
        {
            if (spike != null)
                spike.StartCycling();
        }
    }

    private void CompleteSuccess()
    {
        isComplete = true;
        isRunning = false;
        Debug.Log("All levers pulled! Door opening!");
        onSuccess?.Invoke();

        if (treasureDoor != null)
            treasureDoor.SetActive(false);
    }
}