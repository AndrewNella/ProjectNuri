using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MonsterBase", menuName = "Scriptable Objects/MonsterBase")]
public class MonsterBase : ScriptableObject
{
    [SerializeField] string monsterName;

    [TextArea]
    [SerializeField] string monsterDescription;

    [SerializeField] Sprite frontSprite, backSprite;

    [SerializeField] MonsterType enemyType1, enemyType2;

    //Basic Data
    [SerializeField] float maxHP, maxMana, lust, attack, defense, magicAttack, magicDefense, speed;

    [SerializeField] List<LearnableAttacks> listOfUsableAttacks;

    public string Name => name;


    public string Description => monsterDescription;

    public Sprite FrontSprite => frontSprite;
    public Sprite BackSprite => backSprite;

    public MonsterType EnemyType1 => enemyType1;
    public MonsterType EnemyType2 => enemyType2;



    public float MaxHp => maxHP;
    public float MaxMana => maxMana;
    public float Lust => lust;


    public float Attack => attack;

    public float Defense => defense;


    public float MagicAttack => magicAttack;
    public float MagicDefense => magicDefense;

    public float Speed => speed;

    public List<LearnableAttacks> LearnableAttacks
    {
        get { return LearnableAttacks; }
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
public enum MonsterType
{
    None,
    Slime,
    LatexParasite,
    Tentacle,
    Beast,
    Human,
    Beastkin

}
