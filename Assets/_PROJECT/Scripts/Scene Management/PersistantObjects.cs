using UnityEngine;

public class PersistantObjects : MonoBehaviour
{
    // [SerializeField] GameObject essentialItemsPrefab;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    
}
