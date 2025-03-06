using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackBase", menuName = "Scriptable Objects/AttackBase")]
public class AttackBase : ScriptableObject
{
    [SerializeField] string attackname;

    [TextArea]
    [SerializeField] string attackDescription;

    [SerializeField] float power, accuracy, manaCost, lustCost;

    [SerializeField] AttackType damageType;

    [SerializeField] AttackCategory category;

    [SerializeField] AttackEffects effects;
    [SerializeField] AttackTarget target;



    public string Attackname => attackname;
    public string AttackDescription => attackDescription;
    public float Power => power;

    public AttackType DamageType => damageType;
    public float Accuracy => accuracy;
    public float ManaCost => manaCost;
    public float LustCost => lustCost;

    public AttackCategory Category => category;

    public AttackTarget Target => target;

    public AttackEffects Effects => effects;





}
[System.Serializable]
public class AttackEffects
{
    [SerializeField] List<StatModifications> modifications;
    [SerializeField] ConditionID _status;

    public List<StatModifications> Modifications => modifications;
    public ConditionID Status => _status;
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