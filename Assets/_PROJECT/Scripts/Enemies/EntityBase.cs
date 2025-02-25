using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Entitybase", menuName = "Scriptable Objects/Entitybase")]
public class EntityBase : ScriptableObject
{
    [SerializeField] string entityName;

    [TextArea]
    [SerializeField] string entityDescription;

    [SerializeField] Sprite frontSprite, backSprite;

    [SerializeField] EntityType entityType1, entityType2;

    //Basic Data
    [SerializeField] float maxHP, maxMana, maxLust, attack, defense, magicAttack, magicDefense, speed;

    [SerializeField] List<LearnableAttacks> learnableAttacks;

    public string Name => name;


    public string Description => entityDescription;

    public Sprite FrontSprite => frontSprite;
    public Sprite BackSprite => backSprite;

    public EntityType EnemyType1 => entityType1;
    public EntityType EnemyType2 => entityType2;



    public float MaxHp => maxHP;
    public float MaxMana => maxMana;
    public float MaxLust => maxLust;


    public float Attack => attack;

    public float Defense => defense;


    public float MagicAttack => magicAttack;
    public float MagicDefense => magicDefense;

    public float Speed => speed;

    public List<LearnableAttacks> LearnableAttacks
    {
        get { return learnableAttacks; }
    }


}
[System.Serializable]
public class LearnableAttacks
{
    [SerializeField] AttackBase attackBase;
    [SerializeField] int level;

    public AttackBase Base
    {
        get { return attackBase; }
    }

    public int Level
    {
        get { return level; }
    }
}
public enum EntityType
{
    None,
    Slime,
    LatexParasite,
    Tentacle,
    Beast,
    Human,
    Beastkin

}
