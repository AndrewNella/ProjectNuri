using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackBase", menuName = "Scriptable Objects/AttackBase")]
public class AttackBase : ScriptableObject
{
    [SerializeField] string attackname;

    [TextArea]
    [SerializeField] string attackDescription;

    [SerializeField] float power, accuracy, manaCost, lustCost;

    [SerializeField] bool alwaysHits;

    [SerializeField] AttackType damageType;

    [SerializeField] AttackCategory category;

    [SerializeField] AttackEffects effects;
    [SerializeField] List<SecondaryEffects> secondaryEffects;
    [SerializeField] AttackTarget target;



    public string Attackname => attackname;
    public string AttackDescription => attackDescription;
    public float Power => power;

    public AttackType DamageType => damageType;
    public float Accuracy => accuracy;
    public float ManaCost => manaCost;
    public float LustCost => lustCost;
    public bool AlwaysHits => alwaysHits;

    public AttackCategory Category => category;

    public AttackTarget Target => target;

    public AttackEffects Effects => effects;
    public List<SecondaryEffects> SecondaryEffects => secondaryEffects;




}
[System.Serializable]
public class AttackEffects
{
    [SerializeField] List<StatModifications> modifications;
    [SerializeField] ConditionID status;
    [SerializeField] ConditionID volitileStatus;

    public List<StatModifications> Modifications => modifications;
    public ConditionID Status => status;
    public ConditionID VolitileStatus => volitileStatus;
}

[System.Serializable]
public class SecondaryEffects : AttackEffects
{
    [SerializeField] float chance;
    [SerializeField] AttackTarget target;

    public float Chance => chance;
    public AttackTarget Target => target;
}

[System.Serializable]
public class StatModifications
{
    public Stat stat;
    public float modification;
}
public enum AttackCategory
{
    Physical,
    Magical,
    Status
}

public enum AttackTarget
{
    enemy, self
}
public enum AttackType
{
    None,
    Slash,
    Impact,
    Fire,
    Water,
    Earth,
    Electricity,
    MagicalInfliction,
    Arousal,
    SelfBuff,
    Heal,
    TrueDamage

}