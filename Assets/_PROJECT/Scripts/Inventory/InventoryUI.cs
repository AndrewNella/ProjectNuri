using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] GameObject itemListGameObject;
    [SerializeField] GameObject itemSlotPrefab;

    public List<GameObject> listOfUiButtons = new List<GameObject>();

    Inventory playerInventory;

    public Inventory PlayerInventory => playerInventory;

    [SerializeField] bool displayItemDetails;

    [SerializeField] TMP_Text itemName;
    [SerializeField] Image itemIcon;
    public Button firstButton;

    [ShowIf("displayItemDetails")]
    [SerializeField] TMP_Text itemDetailText;

    public event Action<ItemBase> OnItemUsed;

    BattleMenuControl connectedBattleMenu;


    // [SerializeField] RectTransform itemList
    private void Start()
    {
        playerInventory = Inventory.GetInventory();

        UpdateItemList();

        playerInventory.OnUpdated += UpdateItemList;
    }

    public void SetBattleMenu(BattleMenuControl _battleMenu)
    {
        connectedBattleMenu = _battleMenu;
    }

    //This updates the information of the Button that represents the item
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

    public void UseItemAndUpdateUI(ItemBase _item)
    {
        StartCoroutine(ItemUseCoroutine(_item));
    }

    IEnumerator ItemUseCoroutine(ItemBase _item)
    {
        var _usedItem = playerInventory.AttemptToUseItem(_item);
        if (_usedItem != null)
        {
            //If there is no battle menu connected to this UI,
            //then display the item text on the main Dialogue Manager.
            if (connectedBattleMenu == null)
            {
                yield return DialogueManager.Instance.ShowDialogueText($"You used {_usedItem.ItemName}");
            }
            else OnItemUsed?.Invoke(_item);

        }
        else
        {
            yield return DialogueManager.Instance.ShowDialogueText($"That item won't have any effect.");
        }

        yield return null;
    }

    public void CleanUIButtonList()
    {
        if (listOfUiButtons.Count > 0)
        {
            foreach (GameObject item in listOfUiButtons)
            {
                Destroy(item);
            }
            listOfUiButtons.Clear();

            connectedBattleMenu?.SetFirstInventoryButton(null);

        }
    }
    public void UpdateItemList()
    {
        //Clear Existing Items
        CleanUIButtonList();


        foreach (var _itemSlot in playerInventory.InventorySlotList)
        {
            GameObject _instantiatedObjectHolder = Instantiate(itemSlotPrefab, itemListGameObject.transform);
            _instantiatedObjectHolder.GetComponent<ItemSlotUI>().connectedInventoryUI = this;
            _instantiatedObjectHolder.GetComponent<ItemSlotUI>().SetData(_itemSlot);
            listOfUiButtons.Add(_instantiatedObjectHolder);
        }

        connectedBattleMenu?.SetFirstInventoryButton(listOfUiButtons[0]);


    }



}
