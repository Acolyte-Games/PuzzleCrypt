using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    private static SaveManager saveManager;

    void Start()
    {
        saveManager = Object.FindFirstObjectByType<SaveManager>();
    }

    public static Vector3 GetRespawnPosition()
    {
        if (saveManager == null)
        {
            saveManager = Object.FindFirstObjectByType<SaveManager>();
        }

        if (saveManager != null)
        {
            return saveManager.GetRespawnPosition();
        }

        return Vector3.zero;
    }
}