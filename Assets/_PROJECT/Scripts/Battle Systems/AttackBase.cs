using UnityEngine;

[CreateAssetMenu(fileName = "AttackBase", menuName = "Scriptable Objects/AttackBase")]
public class AttackBase : ScriptableObject
{
    [SerializeField] string attackname;

    [TextArea]
    [SerializeField] string attackDescription;

    [SerializeField] float power, accuracy, manaCost;

    [SerializeField] AttackType damageType1, damageType2;

    public string Attackname => attackname;
    public string AttackDescription => attackDescription;
    public float Power => power;
    public float Accuracy => accuracy;
    public float ManaCost => manaCost;


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
    TrueDamage

}