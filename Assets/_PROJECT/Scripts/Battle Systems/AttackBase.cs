using UnityEngine;

[CreateAssetMenu(fileName = "AttackBase", menuName = "Scriptable Objects/AttackBase")]
public class AttackBase : ScriptableObject
{
    [SerializeField] string attackname;

    [TextArea]
    [SerializeField] string attackDescription;

    [SerializeField] float power, accuracy, manaCost, lustCost;

    [SerializeField] AttackType damageType;

    public string Attackname => attackname;
    public string AttackDescription => attackDescription;
    public float Power => power;

    public AttackType DamageType1 => damageType;
    public float Accuracy => accuracy;
    public float ManaCost => manaCost;
    public float LustCost => lustCost;

    public bool isMagicalAttack
    {
        get
        {
            if (damageType == AttackType.Fire || damageType == AttackType.Water || damageType == AttackType.Electricity)
            {
                return true;
            }
            else return false;
        }
    }
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
    Arousal,
    SelfBuff,
    Heal,
    TrueDamage

}