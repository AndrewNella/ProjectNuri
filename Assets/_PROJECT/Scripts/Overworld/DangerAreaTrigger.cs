using UnityEngine;
using Kisei.Player;

public class DangerAreaTrigger : MonoBehaviour, IPlayerTriggerable
{
    [SerializeField] float randomEncounterChance;
    public void OnPlayerTrigger(PlayerController _player)
    {
        if (UnityEngine.Random.Range(1, 101) < randomEncounterChance)
        {
            PlayerInstanceHUB.Instance.PlayerController.StopPlayerAnimator();
            GameController.instance.StartRandomizedAreaBattle();
        }
    }
}
