using UnityEngine;
using Kisei.Player;
public class AttackTrigger : MonoBehaviour, IPlayerTriggerable
{
    [SerializeField] FieldMonsterBase fieldBase;




    private void Awake()
    {

    }



    public void OnPlayerTrigger(PlayerController _player)
    {
        if (!fieldBase.GetIsBattleDisabled())
        {
            PlayerInstanceHUB.Instance.PlayerController.StopPlayerAnimator();

            fieldBase.TriggerAttackFromThisEntity();
        }
    }
}
