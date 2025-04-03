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

    Button connectedButton;

    private void Awake()
    {

        connectedButton = GetComponent<Button>();
        connectedButton.onClick.AddListener(ClickButtonFunction);
    }

    private void OnDisable()
    {
        connectedButton.onClick.RemoveAllListeners();
    }

    private void OnDestroy()
    {
        connectedButton.onClick.RemoveAllListeners();
    }

    void ClickButtonFunction()
    {
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


