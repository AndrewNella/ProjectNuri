using UnityEngine;

public class GameLayers : MonoBehaviour
{
    [SerializeField] LayerMask solidObjectLayer, dangerLayer, interactableLayer, playerLayer, monsterFieldOfViewlayer;

    public LayerMask SolidLayer => solidObjectLayer;
    public LayerMask DangerLayer => dangerLayer;
    public LayerMask InteractableLayer => interactableLayer;
    public LayerMask PlayerLayer => playerLayer;
    public LayerMask MFOVLayer => monsterFieldOfViewlayer;

    public static GameLayers Instance;
    private void Awake()
    {
        Instance = this;
    }
}
