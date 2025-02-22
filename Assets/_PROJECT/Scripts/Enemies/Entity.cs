using System.Collections.Generic;
using UnityEngine;

public class Entity
{
    public EntityBase Base { get; set; }
    public int Level { get; set; }

    public float currentHP { get; set; }
    public float currentMana { get; set; }
    public float currentLust { get; set; }

    public List<Attack> knownAttacks { get; set; }

    public Entity(EntityBase mBase, int mLevel)
    {
        Base = mBase;
        Level = mLevel;
        currentHP = MaxHp;
        currentMana = MaxMana;
        currentLust = 0;

        knownAttacks = new List<Attack>();

        foreach (var _attack in Base.LearnableAttacks)
        {
            if (_attack.Level <= Level)
            {
                knownAttacks.Add(new Attack(_attack.Base));
            }
        }
    }


    public float MaxHp
    {
        get { return (Base.MaxHp + Level * 50); }
    }
    public float MaxMana
    {
        get { return (Base.MaxMana + Level * 50); }
    }
    public float MaxLust
    {
        get { return (Base.Lust + Level * 50); }
    }
    public float Attack
    {
        get { return (Base.Attack * Level); }
    }
    public float Defense
    {
        get { return (Base.Defense * Level); }
    }
    public float MagicAttack
    {
        get { return (Base.MagicAttack * Level); }
    }
    public float MagicDefense
    {
        get { return (Base.MagicDefense * Level); }
    }
    public float Speed
    {
        get { return (Base.Speed * Level); }
    }

}
