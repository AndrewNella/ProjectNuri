using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] List<ItemSlot> inventoryList;

    public List<ItemSlot> InventorySlotList => inventoryList;

    public event Action OnUpdated;

    public static Inventory GetInventory()
    {
        return PlayerController.instance.gameObject.GetComponent<Inventory>();
    }

    public ItemBase AttemptToUseItem(int _itemIndex)
    {
        var _item = InventorySlotList[_itemIndex].Item;
        bool _isItemUsed = _item.Use();

        if (_isItemUsed)
        {
            RemoveItem(_item);
            return _item;
        }

        return null;
    }
    public ItemBase AttemptToUseItem(ItemBase _item)
    {
        bool _isItemUsed = _item.Use();

        if (_isItemUsed)
        {
            RemoveItem(_item);
            return _item;
        }

        return null;
    }

    public void RemoveItem(ItemBase _item)
    {
        var _itemSlot = InventorySlotList.First(x => x.Item == _item);
        _itemSlot.ItemCount--;
        if (_itemSlot.ItemCount == 0)
        {
            InventorySlotList.Remove(_itemSlot);
        }

        OnUpdated?.Invoke();
    }
}



[System.Serializable]
public class ItemSlot
{
    [SerializeField] ItemBase item;
    [SerializeField] int itemCount;

    public ItemBase Item => item;
    public int ItemCount
    {
        get => itemCount;
        set => itemCount = value;
    }
}
