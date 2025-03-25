using UnityEngine;

public class PersistantObjcets : MonoBehaviour
{
    [SerializeField] GameObject essentialItemsPrefab;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
