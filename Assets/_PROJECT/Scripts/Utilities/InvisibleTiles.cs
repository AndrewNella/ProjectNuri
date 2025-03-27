using UnityEngine;
using UnityEngine.Tilemaps;

public class InvisibleTiles : MonoBehaviour
{

    [SerializeField] TilemapRenderer tileRenderer;

    private void Awake()
    {
        if (tileRenderer == null)
            tileRenderer = gameObject.GetComponent<TilemapRenderer>();

        tileRenderer.enabled = false;


    }
}
