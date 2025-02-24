using System;
using System.Collections;
using UnityEditor.Rendering;
using TMPro;

using UnityEngine;
using UnityEngine.EventSystems;

public class BattleController : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit, enemyUnit;
    [SerializeField] BattleHUD playerHUD, enemyHUD;

    [SerializeField] BattleMenuControl battleMenuControlSystem;

    BattleState state;

    bool detailsAreUpdated = false;

    int currentAction;
    void Start()
    {
        StartCoroutine(SetupBattle());
    }
    private void Update()
    {
        if (state == BattleState.PlayerAttack)
        {

            TMP_Text _textHolder = null;
            foreach (var text in battleMenuControlSystem.attackText)
            {
                if (EventSystem.current.currentSelectedGameObject.GetComponentInChildren<TextMeshProUGUI>() == text && _textHolder == null)
                {
                    _textHolder = EventSystem.current.currentSelectedGameObject.GetComponentInChildren<TMP_Text>();
                    
                    break;
                }

            }
            Debug.Log(_textHolder.text);


        }
    }
    public IEnumerator SetupBattle()
    {
        playerUnit.Setup();
        enemyUnit.Setup();
        playerHUD.SetData(playerUnit.entity);
        enemyHUD.SetData(enemyUnit.entity);

        battleMenuControlSystem.SetDialogue($"You were spotted by a {enemyUnit.entity.Base.Name}. You cannot avoid a battle.");

        battleMenuControlSystem.SetAttacknames(playerUnit.entity.knownAttacks);

        yield return StartCoroutine(battleMenuControlSystem.TypeDialogue($"You were spotted by a {enemyUnit.entity.Base.Name}. You cannot avoid a battle."));
        yield return new WaitForSeconds(1f);

        PlayerAction();

    }

    private void PlayerAction()
    {
        state = BattleState.PlayerAction;
        StartCoroutine(battleMenuControlSystem.TypeDialogue("Choose an Action."));
        battleMenuControlSystem.EnableActionSelector(true);
    }
    public void PlayerMove()
    {
        state = BattleState.PlayerAttack;
        battleMenuControlSystem.EnableActionSelector(false);
        battleMenuControlSystem.EnableDialogueText(false);
        battleMenuControlSystem.EnableAttackSelector(true);
    }

    void UpdateAttackDetails(){

        foreach (var Attackname in playerUnit.entity.knownAttacks)
        {
            
        }
        battleMenuControlSystem.typeText.text = $"Attack Type - ";
        detailsAreUpdated = true;
    }




    public enum BattleState { Start, PlayerAction, PlayerAttack, EnemyAttack, Busy }
}
