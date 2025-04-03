using System;
using System.Collections;
using UnityEditor.Rendering;
using TMPro;

using UnityEngine;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using UnityEngine.UI;
using Unity.Multiplayer.Center.Common;


public enum BattleState { Start, ActionSelection, AttackSelection, RunningTurn, Busy, Inventory, BattleOver, Journal }
public enum BattleAction { Attack, UseItem, Run }
public class BattleController : MonoBehaviour
{
    public static BattleController instance;
    [SerializeField] BattleUnit playerUnit, enemyUnit;
    [SerializeField] BattleMenuControl battleMenuControlSystem;

    Action onItemUsed;
    public event Action<bool, bool> OnBattleOver;
    BattleState state;
    BattleState preState;

    public bool isImportantBattle = false;
    Entity enemyEntity;
    int escapeAttempts;
    FieldMonsterBase fieldMonster;
    ItemBase itemHolder;
    [SerializeField] InventoryUI inventoryUI;
    public BattleState GetCurrentBattleState => state;
    private void Awake()
    {
        instance = this;
    }


    public void SetPreviousState()
    {
        preState = state;
    }
    public void StartBattle(Entity _enemyEntity)
    {

        enemyEntity = _enemyEntity;
        StartCoroutine(SetupBattle());
    }
    public void StartBattle(Entity _enemyEntity, FieldMonsterBase _enemyFieldBase)
    {

        enemyEntity = _enemyEntity;
        fieldMonster = _enemyFieldBase;
        StartCoroutine(SetupBattle());
    }


    public void HandleUpdate()
    {
        if (state == BattleState.AttackSelection)
        {
            if (EventSystem.current.currentSelectedGameObject.TryGetComponent<Button>(out Button _button))
            {
                TMP_Text _textHolder = _button.GetComponentInChildren<TextMeshProUGUI>();
                foreach (var knownAttack in playerUnit.entity.knownAttacks)
                {
                    if (_textHolder.text == knownAttack.Base.Attackname)
                    {
                        battleMenuControlSystem.UpdateAttackDetails(knownAttack);
                        UpdateCurrentlySelectedAttack(knownAttack);
                    }
                }
            }
        }
    }
    void UpdateCurrentlySelectedAttack(Attack _incomingAttack)
    {
        playerUnit.entity.CurrentAttack = _incomingAttack;
    }
    public void ReturnToMainBattleMenu()
    {
        if (state == BattleState.ActionSelection || state == BattleState.AttackSelection || state == BattleState.Inventory || state == BattleState.Journal)
        {

            if (battleMenuControlSystem.AttackSelector.activeSelf)
            {
                battleMenuControlSystem.EnableAttackSelector(false);
            }
            if (battleMenuControlSystem.InventoryMenu.activeSelf)
            {
                battleMenuControlSystem.EnableInventoryScreen(false);
            }
            battleMenuControlSystem.EnableDialogueText(true);
            ActionSelection();
        }
    }
    #region Button Commands
    public void TryEscapeAttempt()
    {
        battleMenuControlSystem.EnableActionSelector(false);
        battleMenuControlSystem.EnableDialogueText(true);
        StartCoroutine(RunTurns(BattleAction.Run));
    }
    public void UseItem(ItemBase _usedItem)
    {

        // Debug.Log("Execute Battle Logic when Item is used");

        itemHolder = _usedItem;
        battleMenuControlSystem.EnableInventoryScreen(false);
        battleMenuControlSystem.EnableActionSelector(false);
        battleMenuControlSystem.EnableDialogueText(true);
        StartCoroutine(RunTurns(BattleAction.UseItem));
    }

    public void InitiateAttack()
    {
        if (playerUnit.entity.CurrentAttack != null)
        {
            if (!CheckIfAttackCanBeAfforded(playerUnit, playerUnit.entity.CurrentAttack))
            {
                // Debug.Log("Mana is too low");
            }
            else
            {

                battleMenuControlSystem.EnableAttackSelector(false);
                battleMenuControlSystem.EnableDialogueText(true);
                StartCoroutine(RunTurns(BattleAction.Attack));
                // Debug.Log("Attack is succesfull");
            }
        }
    }
    public void OpenInventoryScreen()
    {

        UpdateCurrentlySelectedAttack(null);
        state = BattleState.Inventory;
        battleMenuControlSystem.EnableInventoryScreen(true);

    }
    #endregion


    private void ActionSelection()
    {

        UpdateCurrentlySelectedAttack(null);
        state = BattleState.ActionSelection;
        battleMenuControlSystem.SetDialogue("Choose an Action.");
        battleMenuControlSystem.EnableActionSelector(true);
    }
    public void AttackSelection()
    {
        state = BattleState.AttackSelection;
        battleMenuControlSystem.EnableActionSelector(false);
        battleMenuControlSystem.EnableDialogueText(false);
        battleMenuControlSystem.EnableAttackSelector(true);
    }

    #region Battle Related Functions
    void BattleOver(bool _didThePlayerWin, bool _isThisAnEscape)
    {
        state = BattleState.BattleOver;
        playerUnit.entity.OnBattleOver();
        fieldMonster = null;

        battleMenuControlSystem.DestroyAttackButtons();

        OnBattleOver(_didThePlayerWin, _isThisAnEscape);
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
            else
                return true;

        }
        else
            return true;
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

    #endregion
    #region Core Battle Functions
    public IEnumerator SetupBattle()
    {
        escapeAttempts = 0;
        playerUnit.Setup(PlayerController.instance.GetPlayerEntity());
        enemyUnit.Setup(enemyEntity);

        battleMenuControlSystem.SetDialogue($"You were spotted by a {enemyUnit.entity.Base.EntityName}. You cannot avoid a battle.");
        battleMenuControlSystem.PopulateAttackButtons(playerUnit.entity.knownAttacks);

        inventoryUI.SetBattleMenu(battleMenuControlSystem);

        // Debug.Log(inventoryUI.firstButton);

        inventoryUI.OnItemUsed += UseItem;


        // yield return EnableButtons(true);

        yield return StartCoroutine(battleMenuControlSystem.TypeDialogue($"You were spotted by a {enemyUnit.entity.Base.EntityName}. You cannot avoid a battle."));
        yield return new WaitForSeconds(1f);

        ActionSelection();
    }
    IEnumerator RunTurns(BattleAction _playerAction)
    {
        state = BattleState.RunningTurn;

        //Perform an action depending on the Player's Action
        switch (_playerAction)
        {
            case BattleAction.Attack:
                enemyUnit.entity.CurrentAttack = enemyUnit.entity.GetRandomAttack();
                int _playerAttackPriority = playerUnit.entity.CurrentAttack.Base.Priority;
                int _enemyAttackPriority = enemyUnit.entity.CurrentAttack.Base.Priority;

                //Check Who goes first
                bool _playerGoesFirst = true;

                if (_enemyAttackPriority > _playerAttackPriority)
                    _playerGoesFirst = false;
                else if (_enemyAttackPriority == _playerAttackPriority)
                    _playerGoesFirst = playerUnit.entity.Speed >= enemyUnit.entity.Speed;



                var _firstUnit = _playerGoesFirst ? playerUnit : enemyUnit;
                var _secondUnit = _playerGoesFirst ? enemyUnit : playerUnit;

                var _secondentity = _secondUnit.entity;

                //First Unit
                yield return PerformAttack(_firstUnit, _secondUnit, _firstUnit.entity.CurrentAttack);
                yield return RunAfterTurn(_firstUnit);
                if (state == BattleState.BattleOver) yield break;


                //Second Unit
                if (_secondentity.currentHP > 0)
                {
                    yield return PerformAttack(_secondUnit, _firstUnit, _secondUnit.entity.CurrentAttack);
                    yield return RunAfterTurn(_secondUnit);
                    if (state == BattleState.BattleOver) yield break;
                }
                break;

            case BattleAction.UseItem:
                state = BattleState.Busy;
                yield return battleMenuControlSystem.TypeDialogue($"You used {itemHolder.ItemName}");
                itemHolder = null;
                yield return RunAfterTurn(playerUnit);

                state = BattleState.RunningTurn;

                //EnemyTurn
                enemyUnit.entity.CurrentAttack = enemyUnit.entity.GetRandomAttack();
                yield return PerformAttack(enemyUnit, playerUnit, enemyUnit.entity.CurrentAttack);
                yield return RunAfterTurn(enemyUnit);
                if (state == BattleState.BattleOver) yield break;

                break;

            case BattleAction.Run:
                // yield return EnableButtons(false);
                yield return TryToEscape();

                //EnemyTurn
                // enemyUnit.entity.CurrentAttack = enemyUnit.entity.GetRandomAttack();
                // yield return PerformAttack(enemyUnit, playerUnit, enemyUnit.entity.CurrentAttack);
                // yield return RunAfterTurn(enemyUnit);
                // if (state == BattleState.BattleOver) yield break;

                break;
            default:
                break;
        }


        if (state != BattleState.BattleOver)
        {
            ActionSelection();
        }

    }


    IEnumerator PerformAttack(BattleUnit _incomingSourceUnit, BattleUnit _incomingTargetUnit, Attack _attack)
    {
        bool canPerformAttack = _incomingSourceUnit.entity.OnBeforeAttack();

        if (!canPerformAttack)
        {
            yield return ShowStatusChanges(_incomingSourceUnit.entity);
            _incomingSourceUnit.HUD.UpdateAll();
            yield break;
        }


        yield return ShowStatusChanges(_incomingSourceUnit.entity);


        yield return battleMenuControlSystem.TypeDialogue($"{_incomingSourceUnit.entity.Base.EntityName} used {_attack.Base.Attackname}.");



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
                    if (_rnd <= _secondary.Chance)
                    {
                        yield return RunAttackEffects(_secondary, _incomingSourceUnit.entity, _incomingTargetUnit.entity, _secondary.Target);
                    }
                }
            }

            if (_incomingTargetUnit.entity.currentHP <= 0)
            {

                yield return HandleDefeatedEntity(_incomingTargetUnit);
            }
        }
        else
        {
            yield return battleMenuControlSystem.TypeDialogue($"{_incomingSourceUnit.entity.Base.EntityName}'s attack missed.");
        }

        _incomingSourceUnit.entity.OnAfterTurn();
    }
    IEnumerator HandleDefeatedEntity(BattleUnit _defeatedUnit)
    {
        yield return battleMenuControlSystem.TypeDialogue($"{_defeatedUnit.entity.Base.EntityName} was defeated.");
        yield return new WaitForSeconds(2f);

        if (!_defeatedUnit.IsPlayerUnit)
        {
            // Gain Exp
            float _gainedEXP = _defeatedUnit.entity.Base.EXPYield;
            int _enemyLevel = _defeatedUnit.entity.Level;

            _gainedEXP *= _enemyLevel;

            playerUnit.entity.exp += _gainedEXP;
            yield return battleMenuControlSystem.TypeDialogue($"{playerUnit.entity.Base.EntityName} gained {_gainedEXP} exp.");


            //Check if Player has enough EXP to Level up.

            while (playerUnit.entity.CheckForLevelUp())
            {
                playerUnit.HUD.SetLevel();
                yield return battleMenuControlSystem.TypeDialogue($"{playerUnit.entity.Base.EntityName} become Level {playerUnit.entity.Level}.");

                //Check for new move to learn, if possible
                var _newAttack = playerUnit.entity.GetLearnableAttackeAtCurrentLevel();

                if (_newAttack != null)
                {
                    playerUnit.entity.LearnNewAttack(_newAttack);
                    yield return battleMenuControlSystem.TypeDialogue($"You have unlocked a new move!.");
                    yield return battleMenuControlSystem.TypeDialogue($"You can now use  {_newAttack.Base.Attackname}.");

                }

            }


            yield return new WaitForSeconds(1f);
        }

        EndBattle(_defeatedUnit);
    }
    IEnumerator RunAfterTurn(BattleUnit _sourceUnit)
    {

        if (state == BattleState.BattleOver) yield break;

        // Status effects can alter a unit's values, so additional checks are needed for the unit after the turn is done.
        yield return ShowStatusChanges(_sourceUnit.entity);

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


        yield return ShowStatusChanges(_sourceEntity);
        yield return ShowStatusChanges(_targetEntity);


    }
    IEnumerator ShowStatusChanges(Entity _incomingEntity)
    {
        while (_incomingEntity.StatusChanges.Count > 0)
        {
            var _message = _incomingEntity.StatusChanges.Dequeue();
            yield return battleMenuControlSystem.TypeDialogue(_message);
        }
    }

    IEnumerator TryToEscape()
    {
        state = BattleState.Busy;

        if (isImportantBattle)
        {
            //Skip func
            yield return battleMenuControlSystem.TypeDialogue($"You cannot escape from this battle. You must fight it.");
            state = BattleState.RunningTurn;
            yield break;
        }

        escapeAttempts++;
        float _playerSpeed = playerUnit.entity.Speed;
        float _enemySpeed = enemyUnit.entity.Speed;

        if (_enemySpeed < _playerSpeed)
        {
            yield return battleMenuControlSystem.TypeDialogue($"You escaped safely.");
            BattleOver(true, true);
        }
        else
        {
            float f = (_playerSpeed * 128) / _enemySpeed + 30 * escapeAttempts;
            f %= 256;

            if (UnityEngine.Random.Range(0, 256) < f)
            {
                yield return battleMenuControlSystem.TypeDialogue($"You escaped safely.");
                BattleOver(true, true);

            }
            else
            {
                yield return battleMenuControlSystem.TypeDialogue($"You could not escape!");
                state = BattleState.RunningTurn;
            }
        }


    }

    #endregion
    void EndBattle(BattleUnit _defeatedUnit)
    {
        inventoryUI.OnItemUsed -= UseItem;


        if (_defeatedUnit.IsPlayerUnit)
        {
            BattleOver(false, false);
        }
        else BattleOver(true, false);

    }

}
