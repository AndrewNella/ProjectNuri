using System;
using System.Collections;
using UnityEditor.Rendering;
using TMPro;

using UnityEngine;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using UnityEngine.UI;
using Unity.Multiplayer.Center.Common;

public class BattleController : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit, enemyUnit;

    [SerializeField] BattleMenuControl battleMenuControlSystem;

    public event Action<bool> OnBattleOver;
    BattleState state;

    Attack currentSelectedAttack;


    Entity enemyEntity;


    public BattleState GetCurrentBattleState => state;

    private void OnEnable()
    {
        MainInputActionController.instance.OnPauseTrigger += ReturnToMainBattleMenu;
    }

    private void OnDisable()
    {
        MainInputActionController.instance.OnPauseTrigger -= ReturnToMainBattleMenu;

    }
    public void StartBattle(Entity _enemyEntity)
    {
        Debug.Log($"Current Player HP is {PlayerController.instance.GetPlayerEntity().currentHP}");
        Debug.Log($"Current Player Mana is {PlayerController.instance.GetPlayerEntity().currentMana}");
        Debug.Log($"Current Player Lust is {PlayerController.instance.GetPlayerEntity().currentLust}");
        enemyEntity = _enemyEntity;
        StartCoroutine(SetupBattle());
    }
    public void HandleUpdate()
    {
        if (state == BattleState.AttackSelection)
        {
            if (EventSystem.current.currentSelectedGameObject != battleMenuControlSystem.currentlySelectedGameObjectByEventSystem)
            {

                TMP_Text _textHolder = null;

                foreach (var text in battleMenuControlSystem.attackText)
                {
                    if (EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject.TryGetComponent<Button>(out Button _button))
                    {

                        if (_button.GetComponentInChildren<TextMeshProUGUI>() == text && _textHolder == null)
                        {
                            _textHolder = EventSystem.current.currentSelectedGameObject.GetComponentInChildren<TMP_Text>();

                        }
                    }

                }
                if (_textHolder != null)
                {
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
    }

    void UpdateCurrentlySelectedAttack(Attack _incomingAttack)
    {
        currentSelectedAttack = _incomingAttack;
    }
    public void ReturnToMainBattleMenu()
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
    public void OpenInventoryScreen()
    {

        UpdateCurrentlySelectedAttack(null);
        state = BattleState.Inventory;
        battleMenuControlSystem.EnableInventoryScreen(true);
    }


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

    void BattleOver(bool _incomingBool)
    {
        state = BattleState.BattleOver;
        playerUnit.entity.OnBattleOver();
        OnBattleOver(_incomingBool);
    }

    public void InitiateAttack()
    {
        if (currentSelectedAttack != null)
        {
            battleMenuControlSystem.EnableAttackSelector(false);
            battleMenuControlSystem.EnableDialogueText(true);
            StartCoroutine(PlayerAttack());
        }
    }

    void PayCostsForAttack(BattleUnit _incomingUnit, Attack _incomingAttack)
    {
        if (_incomingAttack.ManaCost > 0)
        {
            _incomingUnit.entity.currentMana -= _incomingAttack.ManaCost;
            _incomingUnit.entity.manaChanged = true;
        }
        if (_incomingAttack.LustCost > 0)
        {
            _incomingUnit.entity.currentLust += _incomingAttack.LustCost;
            _incomingUnit.entity.lustChanged = true;
        }
    }

    void ChooseFirstTurn()
    {

        if (playerUnit.entity.Speed >= enemyUnit.entity.Speed)
        {
            ActionSelection();
        }
        else
        {
            StartCoroutine(EnemyAttack());
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
    #region Coroutines
    public IEnumerator SetupBattle()
    {
        playerUnit.Setup(PlayerController.instance.GetPlayerEntity());
        enemyUnit.Setup(enemyEntity);

        battleMenuControlSystem.SetDialogue($"You were spotted by a {enemyUnit.entity.Base.Name}. You cannot avoid a battle.");

        battleMenuControlSystem.SetAttacknames(playerUnit.entity.knownAttacks);


        yield return StartCoroutine(battleMenuControlSystem.TypeDialogue($"You were spotted by a {enemyUnit.entity.Base.Name}. You cannot avoid a battle."));
        yield return new WaitForSeconds(1f);

        ChooseFirstTurn();
    }
    public IEnumerator PlayerAttack()
    {
        state = BattleState.PerformAttack;
        yield return PerformAttack(playerUnit, enemyUnit, currentSelectedAttack);

        if (state == BattleState.PerformAttack)
        {
            StartCoroutine(EnemyAttack());

        }


    }
    IEnumerator EnemyAttack()
    {
        state = BattleState.PerformAttack;
        var _attack = enemyUnit.entity.GetRandomAttack();

        yield return PerformAttack(enemyUnit, playerUnit, _attack);
        if (state == BattleState.PerformAttack)
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


        yield return battleMenuControlSystem.TypeDialogue($"{_incomingSourceUnit.entity.Base.name} used {_attack.Base.name}.");
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
                yield return battleMenuControlSystem.TypeDialogue($"{_incomingTargetUnit.entity.Base.name} was defeated.");
                yield return new WaitForSeconds(2f);
                CheckForBattleOver(_incomingTargetUnit);

            }
        }
        else
        {
            yield return battleMenuControlSystem.TypeDialogue($"{_incomingSourceUnit}'s attack missed.");
        }

        _incomingSourceUnit.entity.OnAfterTurn();


        // Status effects can alter a unit's values, so additional checks are needed for the unit after the turn is done.
        yield return ShowStatusChanges(_incomingSourceUnit.entity);

        _incomingSourceUnit.HUD.UpdateHP();
        _incomingSourceUnit.HUD.UpdateLust();
        _incomingSourceUnit.HUD.UpdateMana();

        if (_incomingSourceUnit.entity.currentHP <= 0)
        {
            yield return battleMenuControlSystem.TypeDialogue($"{_incomingTargetUnit.entity.Base.name} was defeated.");
            yield return new WaitForSeconds(2f);
            CheckForBattleOver(_incomingTargetUnit);

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
    #endregion
    void CheckForBattleOver(BattleUnit _defeatedUnit)
    {
        if (_defeatedUnit.IsPlayerUnit)
        {
            BattleOver(false);
        }
        else BattleOver(true);

    }

    public enum BattleState { Start, ActionSelection, AttackSelection, PerformAttack, Busy, Inventory, BattleOver }
}
