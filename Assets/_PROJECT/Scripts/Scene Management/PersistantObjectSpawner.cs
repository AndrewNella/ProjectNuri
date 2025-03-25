using UnityEngine;

public class PersistantObjectSpawner : MonoBehaviour
{
    [SerializeField] GameObject persistantObjectPrefab;

    void Awake()
    {
        var _existingObjects = FindObjectsByType<PersistantObjcets>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        if (_existingObjects.Length == 0)
        {
            Instantiate(persistantObjectPrefab);
        }
    }
}
