using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class BattleMenuControl : MonoBehaviour
{
    [SerializeField] TMP_Text dialogText;
    [SerializeField] float dialogueLetterWaiterTimer;

    [SerializeField] GameObject actionSelector, attackSelector, moveDetails, inventoryScreen;

    public List<TMP_Text> actionTexts;
    public List<TMP_Text> attackText;

    public TMP_Text type1Text, type2Text, manaCostText, lustCostText, attackDescriptionText;

    [Header("First Selected Action")]
    [SerializeField] GameObject actionMenuFirst;
    [SerializeField] GameObject attackMenuFirst;
    [SerializeField] GameObject inventoryMenuFirst;

    public GameObject currentlySelectedGameObjectByEventSystem;


    public GameObject ActionSelection => actionSelector;
    public GameObject AttackSelector => attackSelector;
    public GameObject InventoryMenu => inventoryScreen;
    public void SetDialogue(string _incomingDialogue)
    {
        dialogText.text = _incomingDialogue;
    }

    public void EnableDialogueText(bool _incomingBool)
    {
        dialogText.enabled = _incomingBool;
        if (_incomingBool)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
    public void EnableInventoryScreen(bool _incomingBool)
    {
        inventoryScreen.SetActive(_incomingBool);
        if (_incomingBool)
        {

            EventSystem.current.SetSelectedGameObject(inventoryMenuFirst);
        }

    }
    public void EnableActionSelector(bool _incomingBool)
    {
        actionSelector.SetActive(_incomingBool);
        if (_incomingBool)
        {
            EventSystem.current.SetSelectedGameObject(actionMenuFirst);
        }
    }
    public void EnableAttackSelector(bool _incomingBool)
    {
        attackSelector.SetActive(_incomingBool);
        if (_incomingBool)
        {
            EventSystem.current.SetSelectedGameObject(attackMenuFirst);

        }
    }

    public IEnumerator TypeDialogue(string _dialogue)
    {
        dialogText.text = "";
        foreach (var letter in _dialogue.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(dialogueLetterWaiterTimer);
        }

    }

    public void SetAttacknames(List<Attack> attacks)
    {
        for (int i = 0; i < attacks.Count; i++)
        {
            if (i < attacks.Count)
            {
                attackText[i].text = attacks[i].Base.Attackname;
            }
            else
            {
                attackText[i].text = "-";
            }
        }
    }

    public void UpdateAttackDetails(Attack _incomingAttack)
    {
        manaCostText.text = $"Mana Cost: {_incomingAttack.ManaCost}";
        lustCostText.text = $"Lust Cost:  {_incomingAttack.Base.LustCost}";

        type1Text.text = $"Attack Type - {_incomingAttack.Base.DamageType}";



        attackDescriptionText.text = $"{_incomingAttack.Base.AttackDescription}";
    }
}
