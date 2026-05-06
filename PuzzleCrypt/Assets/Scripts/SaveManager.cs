using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private static SaveManager instance;

    [Header("Spawn Points")]
    public Transform tutorialRespawnPoint;
    public Transform hubRespawnPoint;

    [Header("Player Reference")]
    public GameObject player; // Drag player here in Inspector

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Position player at correct spawn on game start
        PositionPlayerAtSpawn();
    }

    private void PositionPlayerAtSpawn()
    {
        if (player != null)
        {
            Vector3 spawnPosition = GetRespawnPosition();
            player.transform.position = spawnPosition;
            Debug.Log($"Player positioned at spawn: {spawnPosition}");
        }
        else
        {
            // Try to find player if not assigned
            player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                Vector3 spawnPosition = GetRespawnPosition();
                player.transform.position = spawnPosition;
                Debug.Log($"Player found and positioned at spawn: {spawnPosition}");
            }
        }
    }

    // === Skull Key Methods ===
    public static void SaveSkullKey(string skullID)
    {
        PlayerPrefs.SetInt("SkullKey_" + skullID, 1);
        PlayerPrefs.Save();
        Debug.Log($"Saved skull key: {skullID}");
    }

    public static bool IsSkullKeyCollected(string skullID)
    {
        return PlayerPrefs.GetInt("SkullKey_" + skullID, 0) == 1;
    }

    // === Spawn Point Methods ===
    public static void SetActiveRespawnPoint(string respawnID)
    {
        PlayerPrefs.SetString("ActiveRespawn", respawnID);
        PlayerPrefs.Save();
        Debug.Log($"Active respawn point set to: {respawnID}");
    }

    public static string GetActiveRespawnPoint()
    {
        return PlayerPrefs.GetString("ActiveRespawn", "Tutorial");
    }

    public Vector3 GetRespawnPosition()
    {
        string activeRespawn = GetActiveRespawnPoint();

        if (activeRespawn == "Hub" && hubRespawnPoint != null)
        {
            Debug.Log("Respawning at Hub");
            return hubRespawnPoint.position;
        }
        else if (tutorialRespawnPoint != null)
        {
            Debug.Log("Respawning at Tutorial");
            return tutorialRespawnPoint.position;
        }

        return Vector3.zero;
    }

    // === Clear Data ===
    [ContextMenu("Reset All Save Data")]
    public void ClearAllData()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("All save data cleared");
    }

    public static void ClearSkullKey(string skullID)
    {
        PlayerPrefs.DeleteKey("SkullKey_" + skullID);
        PlayerPrefs.Save();
    }
}