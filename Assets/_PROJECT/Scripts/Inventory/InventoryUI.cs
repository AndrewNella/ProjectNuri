using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] GameObject itemListGameObject;
    [SerializeField] GameObject itemSlotPrefab;

    Inventory playerInventory;

    [SerializeField] bool displayItemDetails;

    [SerializeField] TMP_Text itemName;
    [SerializeField] Image itemIcon;

    [ShowIf("displayItemDetails")]
    [SerializeField] TMP_Text itemDetailText;




    // [SerializeField] RectTransform itemList
    private void Start()
    {
        playerInventory = Inventory.GetInventory();

        UpdateItemList();
    }



    public void UpdateItemInformation(ItemSlot _item)
    {
        itemName.text = _item.Item.ItemName;
        itemIcon.sprite = _item.Item.ItemSprite;

        if (displayItemDetails)
        {
            itemDetailText.text = _item.Item.ItemDescription;
        }
        else
        {
            itemDetailText.text = "";
        }


    }


    public void UpdateItemList()
    {
        //Clear Existing Items
        foreach (Transform _itemTransform in itemListGameObject.transform)
        {
            Destroy(_itemTransform.gameObject);
        }

        foreach (var _itemSlot in playerInventory.InventorySlotList)
        {
            GameObject _instantiatedObjectHolder = Instantiate(itemSlotPrefab, itemListGameObject.transform);
            _instantiatedObjectHolder.GetComponent<ItemSlotUI>().connectedInventoryUI = this;
            _instantiatedObjectHolder.GetComponent<ItemSlotUI>().SetData(_itemSlot);
        }
    }

}
