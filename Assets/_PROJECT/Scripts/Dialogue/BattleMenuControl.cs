using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class BattleMenuControl : MonoBehaviour
{
    [SerializeField] TMP_Text dialogText;
    [SerializeField] float dialogueLetterWaiterTimer;

    [SerializeField] GameObject actionSelector, attackSelector, moveDetails;

    public List<TMP_Text> actionTexts;
    public List<TMP_Text> attackText;

    public TMP_Text typeText, manaCostText, lustCostText;

    [Header("First Selected Action")]
    [SerializeField] GameObject actionMenuFirst;
    [SerializeField] GameObject attackMenuFirst;



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
        if (_incomingAttack.ManaCost == 0)
        {
            manaCostText.text = "Mana Cost: 0";
        }
        else manaCostText.text = $"Mana Cost: {_incomingAttack.ManaCost}";

        if (_incomingAttack.Base.LustCost == 0)
        {
            manaCostText.text = "Lust Cost: 0";

        }
        else manaCostText.text = $"Lust Cost:  {_incomingAttack.Base.LustCost}";

        if (_incomingAttack.Base.DamageType2 == AttackType.None)
        {
            manaCostText.text = $"Attack Type - {_incomingAttack.Base.DamageType1}";

        }
        else manaCostText.text = $"Attack Type - {_incomingAttack.Base.DamageType1} - {_incomingAttack.Base.DamageType2}";
    }
}
