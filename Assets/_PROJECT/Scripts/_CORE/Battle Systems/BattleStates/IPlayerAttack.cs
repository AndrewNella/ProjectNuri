using System;
using System.Collections;
using Kisei.BattleSystem;
using Kisei.Player;
using UnityEngine;
public class IPlayerAttack :  IBattleState
{
    BattleInstanceHUB hub;

    BattleController battleStateController;

    BattleFunctions battleLogic;


    public IPlayerAttack(BattleInstanceHUB _hub)
    {
        hub = _hub;
        battleStateController = hub.BattleController;
        battleLogic = hub.BattleLogic;

    }

    public void ExecuteTurn()
    {
        hub.StartCoroutine(HandlePlayerAction());
    }


    IEnumerator HandlePlayerAction()
    {
        battleStateController.GetCurrentEnemyUnit.entity.CurrentAttack = battleStateController.GetCurrentEnemyUnit.entity.GetRandomAttack();
        int _playerAttackPriority = battleStateController.GetCurrentPlayerUnit.entity.CurrentAttack.Base.Priority;
        int _enemyAttackPriority = battleStateController.GetCurrentEnemyUnit.entity.CurrentAttack.Base.Priority;

        //Check Who goes first
        bool _playerGoesFirst = true;

        if (_enemyAttackPriority > _playerAttackPriority)
            _playerGoesFirst = false;
        else if (_enemyAttackPriority == _playerAttackPriority)
            _playerGoesFirst = battleStateController.GetCurrentPlayerUnit.entity.Speed >= battleStateController.GetCurrentEnemyUnit.entity.Speed;



        var _firstUnit = _playerGoesFirst ? battleStateController.GetCurrentPlayerUnit : battleStateController.GetCurrentEnemyUnit;
        var _secondUnit = _playerGoesFirst ? battleStateController.GetCurrentEnemyUnit : battleStateController.GetCurrentPlayerUnit;

        var _secondentity = _secondUnit.entity;
        //First Unit
        yield return battleLogic.PerformAttack(_firstUnit, _secondUnit, _firstUnit.entity.CurrentAttack);
        yield return battleLogic.RunAfterTurn(_firstUnit);
        if (battleStateController.GetCurrentBattleState == BattleState.BattleOver) yield break;


        //Second Unit
        if (_secondentity.currentHP > 0)
        {
            yield return battleLogic.PerformAttack(_secondUnit, _firstUnit, _secondUnit.entity.CurrentAttack);
            yield return battleLogic.RunAfterTurn(_secondUnit);
            if (battleStateController.GetCurrentBattleState == BattleState.BattleOver) yield break;
        }
    }

}
