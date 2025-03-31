using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] List<ItemSlot> inventoryList;

    public List<ItemSlot> InventorySlotList => inventoryList;
    public static Inventory GetInventory()
    {
        return PlayerController.instance.gameObject.GetComponent<Inventory>();
    }

}



[System.Serializable]
public class ItemSlot
{
    [SerializeField] ItemBase item;
    [SerializeField] int itemCount;

    public ItemBase Item => item;
    public int ItemCount => itemCount;
}
