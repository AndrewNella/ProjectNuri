using UnityEngine;
using Kisei.Player;
public class PersistantObjectSpawner : MonoBehaviour
{
    [SerializeField] GameObject persistantObjectPrefab;

    [SerializeField] bool useSpesificSpawnPointForPlayer, useDefaultAreaMap;

    [SerializeField] Transform customPlayerTransform;

    void Awake()
    {

        var _existingObjects = FindObjectsByType<PersistantObjects>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        if (_existingObjects.Length == 0)
        {
            Instantiate(persistantObjectPrefab);
            if (customPlayerTransform != null && useSpesificSpawnPointForPlayer)
            {

                PlayerInstanceHUB.Instance.PlayerCharacter.gridparentTransform.position = customPlayerTransform.position;
            }

            GameController.instance.SetCurrentMapAreaToDefault();
            GameController.instance.SetIsDataLoaded();
        }


    }
}
