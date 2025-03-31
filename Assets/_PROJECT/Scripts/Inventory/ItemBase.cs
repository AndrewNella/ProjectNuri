using UnityEngine;

public class ItemBase : ScriptableObject
{

    [SerializeField] string itemName;

    [TextArea]
    [SerializeField] string itemDescription;

    [SerializeField] Sprite itemIcon;

    public string ItemName => itemName;
    public string ItemDescription => itemDescription;
    public Sprite ItemSprite => itemIcon;

}
