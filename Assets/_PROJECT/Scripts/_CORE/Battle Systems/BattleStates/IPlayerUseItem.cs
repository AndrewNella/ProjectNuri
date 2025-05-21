using System.Collections;
using Kisei.BattleSystem;
using UnityEngine;

public class IPlayerUseItem :  IBattleState
{
    BattleInstanceHUB hub;

    BattleController battleStateController;
    BattleMenuControl battleUI;
    BattleFunctions battleLogic;

    public IPlayerUseItem(BattleInstanceHUB _hub)
    {
        hub = _hub;

        battleStateController = hub.BattleController;
        battleUI = hub.BattleUI;
        battleLogic = hub.BattleLogic;
    }

    public void ExecuteTurn()
    {
        hub.StartCoroutine(HandlePlayerAction());
    }
    IEnumerator HandlePlayerAction()
    {
        battleStateController.SetCurrentState(BattleState.Busy);
        yield return battleUI.TypeDialogue($"You used {battleStateController.itemHolder.ItemName}");
        battleStateController.itemHolder = null;
        yield return battleLogic.RunAfterTurn(battleStateController.GetCurrentPlayerUnit);

        battleStateController.SetCurrentState(BattleState.RunningTurn);

        //EnemyTurn
        battleStateController.GetCurrentEnemyUnit.entity.CurrentAttack = battleStateController.GetCurrentEnemyUnit.entity.GetRandomAttack();
        yield return battleLogic.PerformAttack(battleStateController.GetCurrentEnemyUnit, battleStateController.GetCurrentPlayerUnit, battleStateController.GetCurrentEnemyUnit.entity.CurrentAttack);
        yield return battleLogic.RunAfterTurn(battleStateController.GetCurrentEnemyUnit);
        if (battleStateController.GetCurrentBattleState == BattleState.BattleOver) yield break;

    }

}
