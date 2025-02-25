using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

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

    public Attack GetRandomAttack()
    {
        int r = Random.Range(0, knownAttacks.Count);
        return knownAttacks[r];
    }

    public bool TakePhysicalDamage(Attack _incomingAttack, Entity _incomingEntity)
    {
        float _damage = _incomingAttack.Base.Power + (_incomingEntity.Attack - Defense) + 2;
        if (_damage < 0)
        {
            _damage = 0;
        }

        currentHP -= _damage;

        if (currentHP <= 0)
        {
            currentHP = 0;
            return true;
        }

        return false;
    }

    public bool TakeMagicDamage(Attack _incomingAttack, Entity _incomingEntity)
    {
        float randomModifier = Random.Range(0.85f, 1.25f);
        float _damage = _incomingAttack.Base.Power + (_incomingEntity.MagicAttack - MagicDefense) + 2;
        if (_damage < 0)
        {
            _damage = 0;
        }

        _damage *= randomModifier;

        currentHP -= _damage;

        if (currentHP <= 0)
        {
            currentHP = 0;
            return true;
        }

        return false;
    }

    public bool TakeArousalDamage(Attack _incomingAttack, Entity _incomingEntity)
    {
        float _damage = _incomingAttack.Base.Power + (_incomingEntity.Attack - Defense) + 2;
        if (_damage < 0)
        {
            _damage = 0;
        }

        currentLust += _damage;

        if (currentLust >= MaxLust)
        {
            currentHP = MaxLust;
            return true;
        }

        return false;
    }
}
