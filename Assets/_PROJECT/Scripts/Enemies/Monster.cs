using System.Collections.Generic;
using UnityEngine;

public class Monster
{
    MonsterBase _base;
    int level;

    public float currentHP { get; set; }

    public List<AttackConstructor> knownAttacks { get; set; }

    public Monster(MonsterBase mBase, int mLevel)
    {
        _base = mBase;
        level = mLevel;
        currentHP = _base.MaxHp;
        
        knownAttacks = new List<AttackConstructor>();

        foreach (var _attack in _base.LearnableAttacks)
        {
            if (_attack.Level <= level)
            {
                knownAttacks.Add(new AttackConstructor(_attack.Base));
            }
        }
    }


    public float MaxHp
    {
        get { return (_base.MaxHp * level) + 10f; }
    }
    public float MaxMana
    {
        get { return (_base.MaxMana * level); }
    }
    public float Lust
    {
        get { return (_base.Lust); }
    }
    public float Attack
    {
        get { return (_base.Attack * level); }
    }
    public float Defense
    {
        get { return (_base.Defense * level); }
    }
    public float MagicAttack
    {
        get { return (_base.MagicAttack * level); }
    }
    public float MagicDefense
    {
        get { return (_base.MagicDefense * level); }
    }
    public float Speed
    {
        get { return (_base.Speed * level); }
    }

}
