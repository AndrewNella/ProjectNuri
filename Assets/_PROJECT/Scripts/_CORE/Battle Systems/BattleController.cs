using System;
using TMPro;

using UnityEngine;
using UnityEngine.EventSystems;
using Kisei.Player;
using UnityEngine.UI;
using Kisei.BattleSystem;
using System.Collections;
using Unity.VisualScripting;


public enum BattleState { Start, ActionSelection, AttackSelection, RunningTurn, Busy, Inventory, BattleOver, Journal }
public enum BattleAction { Attack, UseItem, Run }
public class BattleController : MonoBehaviour
{

    Entity enemyEntity;

    public ItemBase itemHolder;

    BattleStateMachine mainBattleStateMachine;

    public BattleUnit playerUnit, enemyUnit;
    BattleMenuControl BattleUI;
    BattleFunctions battleLogic;

    InventoryUI battleInventory;

    public event Action<bool, bool> OnBattleOver;
    BattleState state;
    BattleState preState;



    public BattleState GetCurrentBattleState => state;
    public BattleUnit GetCurrentPlayerUnit => playerUnit;
    public BattleUnit GetCurrentEnemyUnit => enemyUnit;


    private void Awake()
    {

        mainBattleStateMachine = new BattleStateMachine(BattleInstanceHUB.Instance);

        BattleUI = BattleInstanceHUB.Instance.BattleUI;
        battleLogic = BattleInstanceHUB.Instance.BattleLogic;
        battleInventory = BattleInstanceHUB.Instance.BattleInventory;

        // Debug.Log($"Battle Logic is {battleLogic}.");


        battleLogic.SendBattleOverArg1 += BattleOver;
        battleLogic.SendBattleOverArg2 += EndBattleWithDefeatedUnit;
        battleLogic.SendStartTurnArg += RunTurns;
    }
    void SetPreviousState()
    {
        preState = state;
    }
    public void SetCurrentState(BattleState _newState)
    {
        SetPreviousState();
        state = _newState;
    }


    void OnDestroy()
    {
        if (battleLogic != null)
        {
            battleLogic.SendBattleOverArg1 -= BattleOver;
            battleLogic.SendBattleOverArg2 -= EndBattleWithDefeatedUnit;
            battleLogic.SendStartTurnArg -= RunTurns;

        }
    }
    public void HandleUpdate()
    {
        if (state == BattleState.AttackSelection)
        {
            if (EventSystem.current.currentSelectedGameObject == null) return;

            if (EventSystem.current.currentSelectedGameObject.TryGetComponent<Button>(out Button _button))
            {
                TMP_Text _textHolder = _button.GetComponentInChildren<TextMeshProUGUI>();
                foreach (var knownAttack in playerUnit.entity.knownAttacks)
                {
                    if (_textHolder.text == knownAttack.Base.Attackname)
                    {
                        BattleUI.UpdateAttackDetails(knownAttack);
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

    #region Button Commands
    public void TryEscapeAttempt()
    {
        BattleUI.EnableActionSelector(false);
        BattleUI.EnableDialogueText(true);
        RunTurns(BattleAction.Run);
    }
    public void UseItem(ItemBase _usedItem)
    {
        // Debug.Log("Execute Battle Logic when Item is used");
        itemHolder = _usedItem;
        BattleUI.EnableInventoryScreen(false);
        BattleUI.EnableActionSelector(false);
        BattleUI.EnableDialogueText(true);
        RunTurns(BattleAction.UseItem);
    }


    public void OpenInventoryScreen()
    {
        UpdateCurrentlySelectedAttack(null);
        state = BattleState.Inventory;
        BattleUI.EnableInventoryScreen(true);
    }
    #endregion


    private void ActionSelection()
    {
        UpdateCurrentlySelectedAttack(null);
        state = BattleState.ActionSelection;
        BattleUI.SetDialogue("Choose an Action.");
        BattleUI.EnableActionSelector(true);
    }

    public void ActionSelection(BattleMenuControl _menuControllerCheck = null)
    {
        if (_menuControllerCheck == null) return;
        UpdateCurrentlySelectedAttack(null);
        state = BattleState.ActionSelection;
        BattleUI.SetDialogue("Choose an Action.");
        BattleUI.EnableActionSelector(true);
    }
    public void AttackSelection()
    {
        state = BattleState.AttackSelection;
        BattleUI.EnableActionSelector(false);
        BattleUI.EnableDialogueText(false);
        BattleUI.EnableAttackSelector(true);
    }
    #region Battle Start Functions
    public void StartBattle(Entity _enemyEntity)
    {
        enemyEntity = _enemyEntity;
        StartCoroutine(SetupBattle());
    }
    public void StartBattle(Entity _enemyEntity, FieldMonsterBase _enemyFieldBase)
    {
        enemyEntity = _enemyEntity;
        battleLogic.fieldMonster = _enemyFieldBase;
        StartCoroutine(SetupBattle());
    }
    public IEnumerator SetupBattle()
    {
        // mainBattleStateMachine.Initialize();

        battleLogic.escapeAttempts = 0;

        enemyUnit.Setup(enemyEntity);
        playerUnit.Setup(PlayerInstanceHUB.Instance.PlayerController.PlayerEntity);

        BattleUI.SetDialogue($"You were spotted by a {enemyUnit.entity.Base.EntityName}. You cannot avoid a battle.");
        Debug.Log(playerUnit.entity.knownAttacks);
        BattleUI.PopulateAttackButtons(playerUnit.entity.knownAttacks);

        battleInventory.SetBattleMenu(BattleUI);
        battleInventory.OnItemUsed += UseItem;

        // yield return EnableButtons(true);

        yield return BattleUI.TypeDialogue($"You were spotted by a {enemyUnit.entity.Base.EntityName}. You cannot avoid a battle.");
        yield return new WaitForSeconds(1f);

        ActionSelection();
    }
    #endregion
    #region Battle End Functions
    void BattleOver(bool _didThePlayerWin, bool _isThisAnEscape)
    {
        state = BattleState.BattleOver;

        playerUnit.entity.OnBattleOver();

        battleLogic.SetCurrentFieldMonster(null);

        BattleUI.DestroyAttackButtons();
        OnBattleOver(_didThePlayerWin, _isThisAnEscape);
    }
    void EndBattleWithDefeatedUnit(BattleUnit _defeatedUnit)
    {
        BattleInstanceHUB.Instance.BattleInventory.OnItemUsed -= UseItem;

        if (_defeatedUnit.IsPlayerUnit)
        {
            BattleOver(false, false);
        }
        else BattleOver(true, false);

    }

    #endregion
    #region Core State Functions

    public void RunTurns(BattleAction _playerAction)
    {
        state = BattleState.RunningTurn;

        //Perform an action depending on the Player's Action
        switch (_playerAction)
        {
            case BattleAction.Attack:
                mainBattleStateMachine.SetCurrentState(mainBattleStateMachine.playerAttackState);
                mainBattleStateMachine.ExecuteCurrentState();
                break;

            case BattleAction.UseItem:
                mainBattleStateMachine.SetCurrentState(mainBattleStateMachine.PlayerItemState);
                mainBattleStateMachine.ExecuteCurrentState();
                break;

            case BattleAction.Run:
                mainBattleStateMachine.SetCurrentState(mainBattleStateMachine.playerRunState);
                mainBattleStateMachine.ExecuteCurrentState();

                //EnemyTurn
                break;
            default:
                break;
        }


        if (state != BattleState.BattleOver)
        {
            ActionSelection();
        }

    }
    #endregion

}


[Serializable]
public class BattleStateMachine
{
    public IBattleState currentBattleState { get; private set; }
    public IPlayerAttack playerAttackState;
    public IPlayerRun playerRunState;
    public IPlayerUseItem PlayerItemState;


    public BattleStateMachine(BattleInstanceHUB _hub)
    {

        this.playerAttackState = new IPlayerAttack(_hub);
        this.playerRunState = new IPlayerRun(_hub);
        this.PlayerItemState = new IPlayerUseItem(_hub);
    }
    public void SetCurrentState(IBattleState startingState)
    {
        currentBattleState = startingState;

    }

    public void ExecuteCurrentState()
    {
        if (currentBattleState != null)
        {
            currentBattleState.ExecuteTurn();
        }
    }
}
