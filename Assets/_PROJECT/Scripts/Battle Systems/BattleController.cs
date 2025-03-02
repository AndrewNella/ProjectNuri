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
        }
        if (_incomingAttack.LustCost > 0)
        {
            _incomingUnit.entity.currentLust += _incomingAttack.LustCost;
        }
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

        ActionSelection();
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
        yield return battleMenuControlSystem.TypeDialogue($"{_incomingSourceUnit.entity.Base.name} used {_attack.Base.name}.");
        PayCostsForAttack(_incomingSourceUnit, _attack);
        _incomingSourceUnit.HUD.UpdateMana();
        _incomingSourceUnit.HUD.UpdateLust();

        yield return new WaitForSeconds(1f);


        bool _isDefeated = _incomingTargetUnit.entity.TakeDamage(_attack, _incomingSourceUnit.entity);
        _incomingTargetUnit.HUD.UpdateHP();

        if (_isDefeated)
        {
            yield return battleMenuControlSystem.TypeDialogue($"{_incomingTargetUnit.entity.Base.name} was defeated.");
            yield return new WaitForSeconds(2f);
            CheckForBattleOver(_incomingTargetUnit);

        }

    }
    void CheckForBattleOver(BattleUnit _defeatedUnit)
    {
        if (_defeatedUnit.IsPlayerUnit)
        {
            BattleOver(false);
        }
        else BattleOver(true);

    }

    #endregion
    public enum BattleState { Start, ActionSelection, AttackSelection, PerformAttack, Busy, Inventory, BattleOver }
}
