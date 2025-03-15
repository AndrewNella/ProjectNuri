using UnityEngine;

public class GameLayers : MonoBehaviour
{
    [SerializeField] LayerMask solidObjectLayer, dangerLayer, interactableLayer;

    public LayerMask SolidLayer => solidObjectLayer;
    public LayerMask DangerLayer => dangerLayer;
    public LayerMask InteractableLayer => interactableLayer;

    public static GameLayers Instance;
    private void Awake()
    {
        Instance = this;
    }
}
