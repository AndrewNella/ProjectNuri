using UnityEngine;

public class DangerAreaTrigger : MonoBehaviour, IPlayerTriggerable
{
    [SerializeField] float randomEncounterChance;
    public void OnPlayerTrigger(PlayerController _player)
    {
        if (UnityEngine.Random.Range(1, 101) < randomEncounterChance)
        {
            GameController.instance.StartRandomizedAreaBattle();
        }
    }
}
