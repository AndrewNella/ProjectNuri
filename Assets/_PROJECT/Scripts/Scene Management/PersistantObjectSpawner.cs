using UnityEngine;

public class PersistantObjectSpawner : MonoBehaviour
{
    [SerializeField] GameObject persistantObjectPrefab;

    [SerializeField] bool useSpesificSpawnPointForPlayer, useDefaultAreaMap;

    [SerializeField] Transform customPlayerTransform;

    void Awake()
    {

        var _existingObjects = FindObjectsByType<PersistantObjcets>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        if (_existingObjects.Length == 0)
        {
            Instantiate(persistantObjectPrefab);
            if (customPlayerTransform != null && useSpesificSpawnPointForPlayer)
            {

                PlayerController.instance.PlayerCharacter.gridparentTransform.position = customPlayerTransform.position;
            }

            GameController.instance.SetCurrentMapAreaToDefault();

        }


    }
}
