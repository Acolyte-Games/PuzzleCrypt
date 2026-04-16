using Unity.VisualScripting;
using UnityEngine;

public static class GlobalHelper
{
    public static string GenerateUniqueID(GameObject obj)
    {
        return $"{obj.scene.name}_{obj.transform.position.x}_{obj.transform.position.y}"; //Example: Key_1_5 where "Key" is the object name, and "1" and "5" are the x and y coords respectively.
    }
}
