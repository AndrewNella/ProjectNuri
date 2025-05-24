using UnityEngine;

public class GameLayers : MonoBehaviour
{
    [SerializeField] LayerMask solidObjectLayer, dangerLayer, interactableLayer, playerLayer, monsterFieldOfViewlayer, portalLayer;

    public LayerMask SolidLayer => solidObjectLayer;
    public LayerMask DangerLayer => dangerLayer;
    public LayerMask InteractableLayer => interactableLayer;
    public LayerMask PlayerLayer => playerLayer;
    public LayerMask MFOVLayer => monsterFieldOfViewlayer;
    public LayerMask Portal => portalLayer;
    public LayerMask TriggerableLayer => portalLayer | monsterFieldOfViewlayer | dangerLayer;

    public static GameLayers Instance;
    private void Awake()
    {
        Instance = this;
    }
}
