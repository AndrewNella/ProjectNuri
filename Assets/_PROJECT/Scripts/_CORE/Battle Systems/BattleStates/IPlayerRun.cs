using Kisei.BattleSystem;
using UnityEngine;

public class IPlayerRun :  IBattleState
{
    BattleInstanceHUB hub;

    public IPlayerRun(BattleInstanceHUB _hub)
    {
        hub = _hub;
    }
    public void ExecuteTurn()
    {
        hub.StartCoroutine(hub.BattleLogic.TryToEscape());
    }
}
