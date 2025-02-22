using System;
using System.Collections;
using UnityEditor.Rendering;
using UnityEngine;

public class BattleController : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit, enemyUnit;
    [SerializeField] BattleHUD playerHUD, enemyHUD;

    [SerializeField] BattleMenuControl battleControlSystem;

    BattleState state;

    int currentAction;
    void Start()
    {
        StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle()
    {
        playerUnit.Setup();
        enemyUnit.Setup();
        playerHUD.SetData(playerUnit.entity);
        enemyHUD.SetData(enemyUnit.entity);

        battleControlSystem.SetDialogue($"You were spotted by a {enemyUnit.entity.Base.Name}. You cannot avoid a battle.");

        battleControlSystem.SetAttacknames(playerUnit.entity.knownAttacks);

        yield return StartCoroutine(battleControlSystem.TypeDialogue($"You were spotted by a {enemyUnit.entity.Base.Name}. You cannot avoid a battle."));
        yield return new WaitForSeconds(1f);

        PlayerAction();

    }

    private void PlayerAction()
    {
        state = BattleState.PlayerAction;
        StartCoroutine(battleControlSystem.TypeDialogue("Choose an Action."));
        battleControlSystem.EnableActionSelector(true);
    }
    public void PlayerMove()
    {
        state = BattleState.PlayerAttack;
        battleControlSystem.EnableActionSelector(false);
        battleControlSystem.EnableDialogueText(false);
        battleControlSystem.EnableAttackSelector(true);

    }




    public enum BattleState { Start, PlayerAction, PlayerAttack, EnemyAttack, Busy }
}
