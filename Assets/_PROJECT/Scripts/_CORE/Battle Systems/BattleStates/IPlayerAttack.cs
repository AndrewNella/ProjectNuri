using System;
using System.Collections;
using Kisei.BattleSystem;
using Kisei.Player;
using UnityEngine;
public class IPlayerAttack : IBattleState
{
    BattleInstanceHUB hub;
    public IPlayerAttack(BattleInstanceHUB _hub)
    {
        hub = _hub;
    }


    public void ExecuteTurn()
    {
        hub.StartCoroutine(hub.BattleLogic.StartAttackTurn());
    }


}
