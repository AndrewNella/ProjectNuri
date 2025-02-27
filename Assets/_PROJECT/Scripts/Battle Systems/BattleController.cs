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
    [SerializeField] BattleHUD playerHUD, enemyHUD;

    [SerializeField] BattleMenuControl battleMenuControlSystem;

    public event Action<bool> OnBattleOver;
    BattleState state;

    Attack currentSelectedAttack;


    Entity enemyEntity;


    private void Start()
    {
    }

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
        if (state == BattleState.PlayerAttack)
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
        PlayerAction();
    }
    void OpenInventoryScreen()
    {
        battleMenuControlSystem.EnableInventoryScreen(true);
    }


    private void PlayerAction()
    {
        UpdateCurrentlySelectedAttack(null);
        state = BattleState.PlayerAction;
        battleMenuControlSystem.SetDialogue("Choose an Action.");
        battleMenuControlSystem.EnableActionSelector(true);
    }
    public void PlayerMove()
    {
        state = BattleState.PlayerAttack;
        battleMenuControlSystem.EnableActionSelector(false);
        battleMenuControlSystem.EnableDialogueText(false);
        battleMenuControlSystem.EnableAttackSelector(true);
    }

    public void InitiateAttack()
    {
        if (currentSelectedAttack != null)
        {
            battleMenuControlSystem.EnableAttackSelector(false);
            battleMenuControlSystem.EnableDialogueText(true);
            StartCoroutine(PerformAttack());
        }
    }

    void PayCostsForAttack(BattleUnit _incomingUnit)
    {

        if (currentSelectedAttack.ManaCost > 0)
        {
            _incomingUnit.entity.currentMana -= currentSelectedAttack.ManaCost;
        }
        if (currentSelectedAttack.LustCost > 0)
        {
            _incomingUnit.entity.currentLust += currentSelectedAttack.LustCost;
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
        playerHUD.SetData(playerUnit.entity);
        enemyHUD.SetData(enemyUnit.entity);

        battleMenuControlSystem.SetDialogue($"You were spotted by a {enemyUnit.entity.Base.Name}. You cannot avoid a battle.");

        battleMenuControlSystem.SetAttacknames(playerUnit.entity.knownAttacks);


        yield return StartCoroutine(battleMenuControlSystem.TypeDialogue($"You were spotted by a {enemyUnit.entity.Base.Name}. You cannot avoid a battle."));
        yield return new WaitForSeconds(1f);

        PlayerAction();
    }
    public IEnumerator PerformAttack()
    {
        state = BattleState.Busy;
        yield return battleMenuControlSystem.TypeDialogue($"{playerUnit.entity.Base.name} used {currentSelectedAttack.Base.name}.");

        PayCostsForAttack(playerUnit);
        playerHUD.UpdateMana();
        playerHUD.UpdateLust();

        yield return new WaitForSeconds(1f);

        bool _isDefeated = enemyUnit.entity.TakeDamage(currentSelectedAttack, playerUnit.entity);
        enemyHUD.UpdateHP();



        if (_isDefeated)
        {
            yield return battleMenuControlSystem.TypeDialogue($"{enemyUnit.entity.Base.name} was defeated.");
            yield return new WaitForSeconds(2f);

            OnBattleOver(true);
        }
        else
        {
            StartCoroutine(EnemyAttack());
        }

    }
    IEnumerator EnemyAttack()
    {
        state = BattleState.EnemyAttack;
        var _attack = enemyUnit.entity.GetRandomAttack();

        yield return battleMenuControlSystem.TypeDialogue($"{enemyUnit.entity.Base.name} used {_attack.Base.name}.");
        PayCostsForAttack(enemyUnit, _attack);
        enemyHUD.UpdateMana();
        enemyHUD.UpdateLust();

        yield return new WaitForSeconds(1f);


        bool _isDefeated = playerUnit.entity.TakeDamage(currentSelectedAttack, playerUnit.entity);
        playerHUD.UpdateHP();

        if (_isDefeated)
        {
            yield return battleMenuControlSystem.TypeDialogue($"{playerUnit.entity.Base.name} was defeated.");
            yield return new WaitForSeconds(2f);

            OnBattleOver(false);
        }
        else
        {
            PlayerAction();
        }
    }


    #endregion
    public enum BattleState { Start, PlayerAction, PlayerAttack, EnemyAttack, Busy }
}
