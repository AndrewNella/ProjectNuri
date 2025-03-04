using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

[System.Serializable]
public class Entity
{

    [SerializeField] EntityBase BaseContainer;
    [SerializeField] int LevelContainer;
    public EntityBase Base { get { return BaseContainer; } }
    public int Level { get { return LevelContainer; } }

    public float currentHP { get; set; }
    public float currentMana { get; set; }
    public float currentLust { get; set; }

    public List<Attack> knownAttacks { get; set; }

    public Dictionary<Stat, float> Stats { get; private set; }
    public Dictionary<Stat, int> StatModifications { get; private set; }

    public Queue<String> StatusChanges { get; private set; } = new Queue<string>();

    public void Init()
    {



        knownAttacks = new List<Attack>();

        foreach (var _attack in Base.LearnableAttacks)
        {
            if (_attack.Level <= Level)
            {
                knownAttacks.Add(new Attack(_attack.Base));
            }
        }
        CalculateStats();

        currentHP = MaxHp;
        currentMana = MaxMana;
        currentLust = 0;

        ResetStatModifications();
    }

    private void ResetStatModifications()
    {
        StatModifications = new Dictionary<Stat, int>(){
            {Stat.Attack,0},
            {Stat.Defense,0},
            {Stat.MagicAttack,0},
            {Stat.MagicDefense,0},
            {Stat.Speed,0},
        };
    }

    void CalculateStats()
    {
        Stats = new Dictionary<Stat, float>();
        Stats.Add(Stat.Attack, Base.Attack * Level);
        Stats.Add(Stat.Defense, Base.Defense * Level);
        Stats.Add(Stat.MagicAttack, Base.MagicAttack * Level);
        Stats.Add(Stat.MagicDefense, Base.MagicDefense * Level);
        Stats.Add(Stat.Speed, Base.Speed * Level);

        MaxHp = (Base.MaxHp + Level * 50);
        MaxMana = (Base.MaxMana + Level * 50);
        MaxLust = (Base.MaxLust + Level * 50);
    }
    float GetStat(Stat _incomingStat)
    {
        float statVal = Stats[_incomingStat];

        //Apply Stat Modification
        int _modification = StatModifications[_incomingStat];
        var _modificationValues = new float[] { 1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f };

        if (_modification >= 0)
        {
            statVal *= _modificationValues[_modification];
        }
        else
        {
            statVal /= _modificationValues[-_modification];
        }

        return statVal;
    }

    public void ApplyStatModifications(List<StatModifications> _incomingStatModifications)
    {
        foreach (var _modification in _incomingStatModifications)
        {
            var _stat = _modification.stat;
            var _statModification = _modification.modification;

            StatModifications[_stat] = (int)Mathf.Clamp((StatModifications[_stat] + _statModification), -10f, 10f);
            if (_statModification > 0)
            {
                StatusChanges.Enqueue($"{Base.Name}'s {_stat} was increased!");
            }
            else
            {
                StatusChanges.Enqueue($"{Base.Name}'s {_stat} was decreased!");

            }


            UnityEngine.Debug.Log($" {_stat} has been modified t0 be {StatModifications[_stat]}");
        }
    }
    public float MaxHp { get; private set; }

    public float MaxMana { get; private set; }

    public float MaxLust { get; private set; }

    public float Attack
    {
        get { return GetStat(Stat.Attack); }
    }
    public float Defense
    {
        get { return GetStat(Stat.Defense); }
    }
    public float MagicAttack
    {
        get { return GetStat(Stat.MagicAttack); }
    }
    public float MagicDefense
    {
        get { return GetStat(Stat.MagicDefense); }
    }
    public float Speed
    {
        get { return GetStat(Stat.Speed); }
    }

    public Attack GetRandomAttack()
    {
        int r = UnityEngine.Random.Range(0, knownAttacks.Count);
        return knownAttacks[r];
    }

    public DamageDetails TakeDamage(Attack _incomingAttack, Entity _incomingEntity)
    {

        float effectivenessModifier = TypeChart.GetEffectiveness(_incomingAttack.Base.DamageType, _incomingEntity.Base.EntityType1) * TypeChart.GetEffectiveness(_incomingAttack.Base.DamageType, _incomingEntity.Base.EntityType2);
        float _attack = 0;
        float _defense = 0;
        switch (_incomingAttack.Base.Category)
        {
            case AttackCategory.Physical:
                _attack = _incomingEntity.Attack;
                _defense = _incomingEntity.Defense;
                break;
            case AttackCategory.Magical:
                _attack = _incomingEntity.MagicAttack;
                _defense = _incomingEntity.MagicDefense;
                break;

            default:
                break;
        }
        var _DamageDetails = new DamageDetails()
        {
            Type = effectivenessModifier,
            Fainted = false,
            Aroused = false
        };

        float _damage = effectivenessModifier * _incomingAttack.Base.Power + (_attack - _defense) + 2;
        if (_damage < 0)
        {
            _damage = 0;
        }

        if (_incomingAttack.Base.DamageType == AttackType.Arousal)
        {
            currentLust += _damage;
            if (currentLust >= MaxLust)
            {
                currentLust = 0;
                _DamageDetails.Aroused = true;
            }
        }
        else
        {
            currentHP -= _damage;

            if (currentHP <= 0)
            {
                currentHP = 0;
                _DamageDetails.Fainted = true;
            }
        }



        return _DamageDetails;
    }

    public void OnBattleOver()
    {
        ResetStatModifications();
    }
}

public class DamageDetails
{
    public bool Fainted { get; set; }
    public float Type { get; set; }

    public bool Aroused { get; set; }
}
