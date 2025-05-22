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
        hub.StartCoroutine(hub.BattleLogic.StartItemUseTurn());
    }
 

}
