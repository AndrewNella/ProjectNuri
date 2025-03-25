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
    [SerializeField] float maxHP, maxMana, maxLust, attack, defense, magicAttack, magicDefense, speed, expYield;

    [SerializeField] List<LearnableAttacks> learnableAttacks;


    public float GetExpForLevel(int _level)
    {
        return _level * _level * _level;
    }

    public string Name => entityName;


    public string Description => entityDescription;

    public Sprite FrontSprite => frontSprite;
    public Sprite BackSprite => backSprite;

    public EntityType EntityType1 => entityType1;
    public EntityType EntityType2 => entityType2;



    public float MaxHp => maxHP;
    public float MaxMana => maxMana;
    public float MaxLust => maxLust;


    public float Attack => attack;

    public float Defense => defense;


    public float MagicAttack => magicAttack;
    public float MagicDefense => magicDefense;

    public float Speed => speed;
    public float EXPYield => expYield;

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
    Merfolk,
    Beastkin

}
public enum Stat
{
    Attack,
    Defense,
    MagicAttack,
    MagicDefense,
    Speed,

    //Stats used to boost attack accuracy.
    Accuracy,
    Evasion

}
public class TypeChart
{
    static float[][] chart =
    {        
    //                      Slash , Impact , Fire , Water , Earth , Elec , Arousal                
     /*SLM*/   new float[] {1f,     0.5f,   1f,     1f,     1f,     2f,     1f},
     /*LTX*/   new float[] {1f,     0.5f,   1f,     1f,     1f,     1f,     0.5f},
     /*TTL*/   new float[] {1.5f,   1f,     0.5f,   0.5f,   0.5f,   0.5f,   1.5f},
     /*BST*/   new float[] {1f,     1f,     1f,     1f,     1f,     1f,     1.5f},
     /*HMN*/   new float[] {1f,     1f,     1f,     1f,     1f,     1f,     1f},
     /*MFK*/   new float[] {1f,     1f,     2f,     0.5f,     1f,     2f,     0.5f},
     /*BSK*/   new float[] {0.5f,   1f,     1f,     1f,     1f,     1f,     1.5f}
    };

    public static float GetEffectiveness(AttackType attackType, EntityType defenseType)
    {
        if (attackType == AttackType.None || defenseType == EntityType.None)
        {
            return 1f;
        }
        int _row = (int)defenseType - 1;
        int _col = (int)attackType - 1;

        return chart[_row][_col];

    }
}
