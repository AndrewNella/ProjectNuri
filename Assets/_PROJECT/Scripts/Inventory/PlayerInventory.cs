using System;
using System.Linq;
using UnityEngine;
using Kisei.Player;

public class PlayerInventory : InventoryBase
{

    public static PlayerInventory GetPlayerInventory()
    {
        return PlayerInstanceHUB.Instance.PlayerController.gameObject.GetComponent<PlayerInventory>();
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

   
}




