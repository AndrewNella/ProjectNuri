using System;
using System.Collections;
using Kisei.BattleSystem;
using Kisei.Player;
using UnityEngine;

public class BattleFunctions : MonoBehaviour
{
    BattleController battleStateController;
    BattleMenuControl battleUIController;
    InventoryUI battleInventory;
    public int escapeAttempts;
    public bool isImportantBattle = false;
    public FieldMonsterBase fieldMonster;




    Action onItemUsed;

    public event Action<bool, bool> SendBattleOverArg1;
    public event Action<BattleUnit> SendBattleOverArg2;
    public event Action<BattleAction> SendStartTurnArg;
    public event Action OnTurnFinished;



    #region General Functions
    private void Awake()
    {
        battleStateController = BattleInstanceHUB.Instance.BattleController;
        battleUIController = BattleInstanceHUB.Instance.BattleUI;
        battleInventory = BattleInstanceHUB.Instance.BattleInventory;
    }
    #endregion

    #region Battle Functions

    public void InitiateAttack()
    {
        if (battleStateController.playerUnit.entity.CurrentAttack != null)
        {
            if (!CheckIfAttackCanBeAfforded(battleStateController.playerUnit, battleStateController.playerUnit.entity.CurrentAttack))
            {
                // Debug.Log("Mana is too low");
            }
            else
            {
                Debug.Log("Execute Attack");
                battleUIController.EnableAttackSelector(false);
                battleUIController.EnableDialogueText(true);

                SendStartTurnArg?.Invoke(BattleAction.Attack);
                // Debug.Log("Attack is succesfull");
            }
        }
    }
    void PayCostsForAttack(BattleUnit _incomingUnit, Attack _incomingAttack)
    {
        if (_incomingAttack.ManaCost > 0)
        {
            _incomingUnit.entity.currentMana -= _incomingAttack.ManaCost;
            _incomingUnit.entity.InvokeManaChange();
        }
        if (_incomingAttack.LustCost > 0)
        {
            if (_incomingUnit.entity.currentLust + _incomingAttack.LustCost >= +_incomingUnit.entity.MaxLust)
            {
                _incomingUnit.entity.currentLust += _incomingAttack.LustCost;
                _incomingUnit.entity.currentLust -= _incomingUnit.entity.MaxLust;
            }
            else
            {
                _incomingUnit.entity.currentLust += _incomingAttack.LustCost;
                _incomingUnit.entity.InvokeManaChange();
            }
        }
    }
    bool CheckIfAttackHits(Attack _attack, Entity _sourceEntity, Entity _targetEntity)
    {
        if (_attack.Base.AlwaysHits) return true;

        float _attackAccuracy = _attack.Base.Accuracy;
        int _accuracy = _sourceEntity.StatModifications[Stat.Accuracy];
        int _evasion = _sourceEntity.StatModifications[Stat.Evasion];

        var _modificationValues = new float[] { 1f, 4f / 3f, 5f / 3f, 2f, 7f / 3f, 8f / 3f, 3f };

        //Calculate Accuracy
        if (_accuracy > 0) _attackAccuracy *= _modificationValues[_accuracy];
        else _attackAccuracy /= _modificationValues[-_accuracy];

        //Calculate Evasion
        if (_evasion > 0) _attackAccuracy /= _modificationValues[_evasion];
        else _attackAccuracy *= _modificationValues[-_evasion];

        return UnityEngine.Random.Range(1, 100) <= _attackAccuracy;
    }
    bool CheckIfAttackCanBeAfforded(BattleUnit _incomingUnit, Attack _incomingAttack)
    {
        if (_incomingAttack.ManaCost > 0)
        {
            Debug.Log("Payment Cost Check, mana is greater than 0");
            if (_incomingUnit.entity.currentMana - _incomingAttack.ManaCost <= 0)
            {
                Debug.Log("The cost is greater than the character's current mana pool");
                return false;
            }
            else return true;
        }
        else return true;
    }
    public void SetCurrentFieldMonster(FieldMonsterBase _monster)
    {
        fieldMonster = _monster;
    }
    #endregion

    #region Battle Coroutines
    //Performs the entire Logic for an attack.
    public IEnumerator PerformAttack(BattleUnit _incomingSourceUnit, BattleUnit _incomingTargetUnit, Attack _attack)
    {
        bool canPerformAttack = _incomingSourceUnit.entity.OnBeforeAttack();

        if (!canPerformAttack)
        {
            yield return battleUIController.ShowStatusChanges(_incomingSourceUnit.entity);
            _incomingSourceUnit.HUD.UpdateAll();
            yield break;
        }


        yield return battleUIController.ShowStatusChanges(_incomingSourceUnit.entity);


        yield return battleUIController.TypeDialogue($"{_incomingSourceUnit.entity.Base.EntityName} used {_attack.Base.Attackname}.");



        PayCostsForAttack(_incomingSourceUnit, _attack);
        _incomingSourceUnit.HUD.UpdateMana();
        _incomingSourceUnit.HUD.UpdateLust();

        if (CheckIfAttackHits(_attack, _incomingSourceUnit.entity, _incomingTargetUnit.entity))
        {
            yield return new WaitForSeconds(1f);

            if (_attack.Base.Category == AttackCategory.Status)
            {
                yield return RunAttackEffects(_attack.Base.Effects, _incomingSourceUnit.entity, _incomingTargetUnit.entity, _attack.Base.Target);
            }
            else
            {
                //TODO
                DamageDetails _damageDetails = _incomingTargetUnit.entity.TakeDamage(_attack, _incomingSourceUnit.entity);
                _incomingTargetUnit.HUD.UpdateHP();
            }

            if (_attack.Base.SecondaryEffects != null && _attack.Base.SecondaryEffects.Count > 0 && _incomingTargetUnit.entity.currentHP > 0)
            {
                foreach (var _secondary in _attack.Base.SecondaryEffects)
                {
                    float _rnd = UnityEngine.Random.Range(0, 101);
                    if (_rnd <= _secondary.Chance) yield return RunAttackEffects(_secondary, _incomingSourceUnit.entity, _incomingTargetUnit.entity, _secondary.Target);
                }
            }

            if (_incomingTargetUnit.entity.currentHP <= 0) yield return HandleDefeatedEntity(_incomingTargetUnit);

        }
        else yield return battleUIController.TypeDialogue($"{_incomingSourceUnit.entity.Base.EntityName}'s attack missed.");


        _incomingSourceUnit.entity.OnAfterTurn();
    }


    IEnumerator HandleDefeatedEntity(BattleUnit _defeatedUnit)
    {
        yield return battleUIController.TypeDialogue($"{_defeatedUnit.entity.Base.EntityName} was defeated.");
        yield return new WaitForSeconds(2f);

        if (!_defeatedUnit.IsPlayerUnit)
        {
            // Gain Exp
            float _gainedEXP = _defeatedUnit.entity.Base.EXPYield;
            int _enemyLevel = _defeatedUnit.entity.Level;

            _gainedEXP *= _enemyLevel;

            battleStateController.playerUnit.entity.exp += _gainedEXP;
            yield return battleUIController.TypeDialogue($"{battleStateController.playerUnit.entity.Base.EntityName} gained {_gainedEXP} exp.");


            //Check if Player has enough EXP to Level up.

            while (battleStateController.playerUnit.entity.CheckForLevelUp())
            {
                battleStateController.playerUnit.HUD.SetLevel();
                yield return battleUIController.TypeDialogue($"{battleStateController.playerUnit.entity.Base.EntityName} become Level {battleStateController.playerUnit.entity.Level}.");

                //Check for new move to learn, if possible
                var _newAttack = battleStateController.playerUnit.entity.GetLearnableAttackeAtCurrentLevel();

                if (_newAttack != null)
                {
                    battleStateController.playerUnit.entity.LearnNewAttack(_newAttack);
                    yield return battleUIController.TypeDialogue($"You have unlocked a new move!.");
                    yield return battleUIController.TypeDialogue($"You can now use  {_newAttack.Base.Attackname}.");

                }

            }


            yield return new WaitForSeconds(1f);
        }
        SendBattleOverArg2?.Invoke(_defeatedUnit);
        // EndBattle(_defeatedUnit);
    }


    public IEnumerator RunAfterTurn(BattleUnit _sourceUnit)
    {

        if (battleStateController.GetCurrentBattleState == BattleState.BattleOver) yield break;

        // Status effects can alter a unit's values, so additional checks are needed for the unit after the turn is done.
        yield return battleUIController.ShowStatusChanges(_sourceUnit.entity);

        _sourceUnit.HUD.UpdateAll();

        if (_sourceUnit.entity.currentHP <= 0)
        {
            yield return HandleDefeatedEntity(_sourceUnit);
        }
    }
    IEnumerator RunAttackEffects(AttackEffects _incomingAttackEffect, Entity _sourceEntity, Entity _targetEntity, AttackTarget _attackTarget)
    {

        //Stat Modification such as increasing Attack or other values.
        if (_incomingAttackEffect.Modifications != null)
        {
            if (_attackTarget == AttackTarget.self) _sourceEntity.ApplyStatModifications(_incomingAttackEffect.Modifications);
            else _targetEntity.ApplyStatModifications(_incomingAttackEffect.Modifications);
        }


        //Status Condition management. 

        //Normal Status Effect
        if (_incomingAttackEffect.Status != ConditionID.none)
        {
            _targetEntity.SetStatusCondition(_incomingAttackEffect.Status);
        }

        //Volitile Status Effect
        if (_incomingAttackEffect.VolitileStatus != ConditionID.none)
        {
            _targetEntity.SetVolitileStatusCondition(_incomingAttackEffect.VolitileStatus);
        }


        yield return battleUIController.ShowStatusChanges(_sourceEntity);
        yield return battleUIController.ShowStatusChanges(_targetEntity);


    }

    public IEnumerator StartEscapeTurn()
    {
        battleStateController.SetCurrentState(BattleState.Busy);

        if (isImportantBattle)
        {
            //Skip func
            yield return battleUIController.TypeDialogue($"You cannot escape from this battle. You must fight it.");
            battleStateController.SetCurrentState(BattleState.RunningTurn);
            yield break;
        }

        escapeAttempts++;
        float _playerSpeed = battleStateController.playerUnit.entity.Speed;
        float _enemySpeed = battleStateController.enemyUnit.entity.Speed;

        if (_enemySpeed < _playerSpeed)
        {
            yield return battleUIController.TypeDialogue($"You escaped safely.");
            SendBattleOverArg1?.Invoke(true, true);
            // battleStateController.BattleOver(true, true);
        }
        else
        {
            float f = (_playerSpeed * 128) / _enemySpeed + 30 * escapeAttempts;
            f %= 256;

            if (UnityEngine.Random.Range(0, 256) < f)
            {
                yield return battleUIController.TypeDialogue($"You escaped safely.");
                SendBattleOverArg1?.Invoke(true, true);
                // BattleOver(true, true);

            }
            else
            {
                yield return battleUIController.TypeDialogue($"You could not escape!");
                battleStateController.SetCurrentState(BattleState.RunningTurn);
            }
        }
        OnTurnFinished?.Invoke();
    }
    public IEnumerator StartAttackTurn()
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
        yield return PerformAttack(_firstUnit, _secondUnit, _firstUnit.entity.CurrentAttack);
        yield return RunAfterTurn(_firstUnit);
        if (battleStateController.GetCurrentBattleState == BattleState.BattleOver) yield break;


        //Second Unit
        if (_secondentity.currentHP > 0)
        {
            yield return PerformAttack(_secondUnit, _firstUnit, _secondUnit.entity.CurrentAttack);
            yield return RunAfterTurn(_secondUnit);
            if (battleStateController.GetCurrentBattleState == BattleState.BattleOver) yield break;
        }

        OnTurnFinished?.Invoke();
    }

    public IEnumerator StartItemUseTurn()
    {
        battleStateController.SetCurrentState(BattleState.Busy);
        yield return battleUIController.TypeDialogue($"You used {battleStateController.itemHolder.ItemName}");
        battleStateController.itemHolder = null;
        yield return RunAfterTurn(battleStateController.GetCurrentPlayerUnit);

        battleStateController.SetCurrentState(BattleState.RunningTurn);

        //EnemyTurn
        battleStateController.GetCurrentEnemyUnit.entity.CurrentAttack = battleStateController.GetCurrentEnemyUnit.entity.GetRandomAttack();
        yield return PerformAttack(battleStateController.GetCurrentEnemyUnit, battleStateController.GetCurrentPlayerUnit, battleStateController.GetCurrentEnemyUnit.entity.CurrentAttack);
        yield return RunAfterTurn(battleStateController.GetCurrentEnemyUnit);
        if (battleStateController.GetCurrentBattleState == BattleState.BattleOver) yield break;

        OnTurnFinished?.Invoke();
    }
    #endregion
}
