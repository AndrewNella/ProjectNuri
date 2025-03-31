using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Create New Recovery Item")]
public class RecoveryItems : ItemBase
{
    [Header("Recovery Item Settings")]
    [SerializeField] bool isHealingItem;
    [SerializeField] bool isManaRestoreItem;
    [SerializeField] bool isLustResetItem;
    [SerializeField] bool isStatusHealingItem;
    [SerializeField] bool isRevivalItem;

    [ShowIf("isHealingItem")]
    [Header("Health Recovery Options")]
    [SerializeField] float hpRecoveryAmount;

    [ShowIf("isHealingItem")]
    [SerializeField] bool restoreMaxHP;

    [ShowIf("isManaRestoreItem")]
    [Header("Mana Recovery Options")]
    [SerializeField] float manaRecoveryAmount;

    [ShowIf("isManaRestoreItem")]
    [SerializeField] bool restoreMaxMana;

    [ShowIf("isLustResetItem")]
    [Header("Lust Recovery Options")]
    [SerializeField] float lustRecoveryAmount;

    [ShowIf("isLustResetItem")]
    [SerializeField] bool resetCurrentLust;

    [ShowIf("isStatusHealingItem")]
    [Header("Status Effect Recovery Options")]
    [SerializeField] ConditionID statusEffect;

    [ShowIf("isStatusHealingItem")]
    [SerializeField] bool recoverAllStatusEffects;


    [ShowIf("isRevivalItem")]
    [Header("Revival Options")]
    [SerializeField] bool revive;

    [ShowIf("isRevivalItem")]
    [SerializeField] bool maxRevive;


}
