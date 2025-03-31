using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour, ISelectHandler
{
    [SerializeField] TMP_Text itemNameText, itemCountText;

    ItemSlot _itemInformation;
    public InventoryUI connectedInventoryUI;



    public void OnSelect(BaseEventData eventData)
    {
        var _scrollRect = GetComponentInParent<ScrollRect>();
        if (_scrollRect != null)
        {
            // _scrollRect.

        }

        if (connectedInventoryUI != null)
        {
            connectedInventoryUI.UpdateItemInformation(_itemInformation);
        }
    }

    public void SetData(ItemSlot _itemSlot)
    {
        itemNameText.text = _itemSlot.Item.ItemName;
        itemCountText.text = $" X {_itemSlot.ItemCount}";

        _itemInformation = _itemSlot;


    }






}


