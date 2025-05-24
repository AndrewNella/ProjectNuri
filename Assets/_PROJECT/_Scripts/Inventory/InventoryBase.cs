using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System;

public class InventoryBase : MonoBehaviour
{
    public event Action OnUpdated;

    [SerializeField] List<ItemSlot> inventoryList;
    public List<ItemSlot> InventorySlotList => inventoryList;
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

    public void AddItem(ItemBase _item)
    {
        bool _isItemFound = false;
        foreach (var _itemSlot in inventoryList)
        {
            if (_itemSlot.Item == _item)
            {
                _itemSlot.ItemCount++;
                _isItemFound = true;
                break;
            }
        }

        if (_isItemFound) return;

        
        // var _itemSlot = InventorySlotList.First(x => x.Item == _item);

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
