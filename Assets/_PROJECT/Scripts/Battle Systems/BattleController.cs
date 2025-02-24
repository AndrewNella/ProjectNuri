using System;
using System.Collections;
using UnityEditor.Rendering;
using TMPro;

using UnityEngine;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using UnityEngine.UI;

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
            if (EventSystem.current.currentSelectedGameObject != battleMenuControlSystem.currentlySelectedGameObjectByEventSystem)
            {

                TMP_Text _textHolder = null;

                foreach (var text in battleMenuControlSystem.attackText)
                {
                    if (EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject.TryGetComponent<Button>(out Button _button))
                    {

                        if (_button.GetComponentInChildren<TextMeshProUGUI>() == text && _textHolder == null)
                        {
                            _textHolder = EventSystem.current.currentSelectedGameObject.GetComponentInChildren<TMP_Text>();

                        }
                    }

                }
                if (_textHolder != null)
                {
                    foreach (var knownAttack in playerUnit.entity.knownAttacks)
                    {
                        if (_textHolder.text == knownAttack.Base.Attackname)
                        {
                            battleMenuControlSystem.UpdateAttackDetails(knownAttack);

                        }
                    }
                }

            }





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




    public enum BattleState { Start, PlayerAction, PlayerAttack, EnemyAttack, Busy }
}
