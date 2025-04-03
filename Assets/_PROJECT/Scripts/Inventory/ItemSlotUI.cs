using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour, ISelectHandler
{
    [SerializeField] TMP_Text itemNameText, itemCountText;

    ItemSlot itemInformation;

    public ItemSlot ItemInformation => itemInformation;
    public InventoryUI connectedInventoryUI;

    public void ClickButtonFunction()
    {
        Debug.Log("Button is clicked");
        connectedInventoryUI.UseItemAndUpdateUI(itemInformation.Item);
    }
    public void OnSelect(BaseEventData eventData)
    {
        var _scrollRect = GetComponentInParent<ScrollRect>();

        if (connectedInventoryUI != null)
        {
            connectedInventoryUI.UpdateItemInformation(itemInformation);
        }
    }



    public void SetData(ItemSlot _itemSlot)
    {
        itemNameText.text = _itemSlot.Item.ItemName;
        itemCountText.text = $" X {_itemSlot.ItemCount}";

        itemInformation = _itemSlot;


    }






}


