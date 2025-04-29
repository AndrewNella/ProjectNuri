using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

[System.Serializable]
public class Entity
{

    [SerializeField] EntityBase BaseContainer;
    [SerializeField] int level;
    public EntityBase Base { get { return BaseContainer; } }
    public int Level { get { return level; } }

    public float exp { get; set; }

    public float currentHP { get; set; }
    public float currentMana { get; set; }
    public float currentLust { get; set; }

    public List<Attack> knownAttacks { get; set; }
    public Attack CurrentAttack { get; set; }

    public Dictionary<Stat, float> Stats { get; private set; }
    public Dictionary<Stat, int> StatModifications { get; private set; }

    public Condition Status { get; set; }

    public Queue<String> StatusChanges { get; private set; } = new Queue<string>();

    public event Action OnStatusConditionChanged;

    public int StatusTime { get; set; }

    public Condition VolitileStatus { get; private set; }
    public int VolitileStatusTime { get; set; }

    public event Action OnHPChanged;
    public event Action OnManaChanged;
    public event Action OnLustChanged;
    public LearnableAttacks GetLearnableAttackeAtCurrentLevel()
    {
        return Base.LearnableAttacks.Where(x => x.Level == level).FirstOrDefault();
    }

    public void LearnNewAttack(LearnableAttacks _attackToLearn)
    {
        knownAttacks.Add(new Attack(_attackToLearn.Base));
    }

    public void ForgetAttack(Attack _attackToForget)
    {
        foreach (var _attack in knownAttacks)
        {
            if (_attack.Base.Attackname == _attackToForget.Base.Attackname)
            {
                knownAttacks.Remove(_attack);
            }
        }
    }
    public void Init()
    {
        //Generate Moves
        knownAttacks = new List<Attack>();

        foreach (var _attack in Base.LearnableAttacks)
        {
            if (_attack.Level <= Level)
            {
                knownAttacks.Add(new Attack(_attack.Base));
            }
        }

        exp = Base.GetExpForLevel(Level);

        CalculateStats();

        currentHP = MaxHp;
        currentMana = MaxMana;
        currentLust = 0;

        StatusChanges = new Queue<string>();
        ResetStatModifications();

        Status = null;
        VolitileStatus = null;
    }

    public EntitySaveData GetSaveData()
    {
        var _saveData = new EntitySaveData()
        {
            //Basic Data
            entityName = Base.EntityName,
            entityCurrentHP = currentHP,
            entityCurrentMana = currentMana,
            entityCurrentLust = currentLust,

            //Level Data
            entityLevel = level,
            entityEXP = exp,

            //Special Data
            entityStatusID = Status?.ID,

            attackData = knownAttacks.Select(a => a.GetAttackSaveData()).ToList()
        };

        return _saveData;
    }

    public Entity(EntitySaveData _saveData)
    {
        //Basic Data
        BaseContainer = EntityDataBase.GetEntityByName(_saveData.entityName);

        currentHP = _saveData.entityCurrentHP;
        currentMana = _saveData.entityCurrentMana;
        currentLust = _saveData.entityCurrentLust;

        //Level Data
        level = _saveData.entityLevel;
        exp = _saveData.entityEXP;

        //Special Data
        if (_saveData.entityStatusID != null)
            Status = ConditionDataBase.Conditions[_saveData.entityStatusID.Value];
        else
            Status = null;


        //Additional Functionality for data restoration

        knownAttacks = _saveData.attackData.Select(s => new Attack(s)).ToList();


        CalculateStats();
        StatusChanges = new Queue<string>();
        ResetStatModifications();
        VolitileStatus = null;

    }

    public bool CheckForLevelUp()
    {
        if (exp > Base.GetExpForLevel(level + 1))
        {
            ++level;
            return true;
        }
        return false;
    }
    private void ResetStatModifications()
    {
        StatModifications = new Dictionary<Stat, int>(){
            {Stat.Attack,0},
            {Stat.Defense,0},
            {Stat.MagicAttack,0},
            {Stat.MagicDefense,0},
            {Stat.Speed,0},
            {Stat.Accuracy,0},
            {Stat.Evasion,0},
        };
    }

    public void SetStatusCondition(ConditionID _incomingCondition)
    {

        if (Status != null) return;

        Status = ConditionDataBase.Conditions[_incomingCondition];
        Status?.OnStart?.Invoke(this);
        StatusChanges.Enqueue($"{Base.EntityName} {Status.StartMessage}");

        OnStatusConditionChanged?.Invoke();
    }
    public void SetVolitileStatusCondition(ConditionID _incomingCondition)
    {

        if (VolitileStatus != null) return;

        VolitileStatus = ConditionDataBase.Conditions[_incomingCondition];
        VolitileStatus?.OnStart?.Invoke(this);
        StatusChanges.Enqueue($"{Base.EntityName} {VolitileStatus.StartMessage}");

    }

    public void CureStatusCondition()
    {
        Status = null;
        OnStatusConditionChanged?.Invoke();

    }
    public void CureVolitileStatusCondition()
    {
        VolitileStatus = null;

    }
    void CalculateStats()
    {
        Stats = new Dictionary<Stat, float>();
        Stats.Add(Stat.Attack, Base.Attack * Level);
        Stats.Add(Stat.Defense, Base.Defense * Level);
        Stats.Add(Stat.MagicAttack, Base.MagicAttack * Level);
        Stats.Add(Stat.MagicDefense, Base.MagicDefense * Level);
        Stats.Add(Stat.Speed, Base.Speed * Level);

        MaxHp = (Base.MaxHp * Level + 10 * Level);
        MaxMana = (Base.MaxHp * Level + 10 * Level);
        MaxLust = (Base.MaxHp * Level + 10 * Level);
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
                StatusChanges.Enqueue($"{Base.EntityName}'s {_stat} was increased!");
            }
            else
            {
                StatusChanges.Enqueue($"{Base.EntityName}'s {_stat} was decreased!");

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

        if (_incomingAttack.Base.DamageType == AttackType.Arousal) InflictLust(_damage);
        else DamageHP(_damage);

        return _DamageDetails;
    }

    public void OnBattleOver()
    {
        VolitileStatus = null;
        ResetStatModifications();
    }

    public void DamageHP(float _incomingFloat)
    {
        currentHP = Mathf.Clamp(currentHP - _incomingFloat, 0, MaxHp);

        OnHPChanged?.Invoke();
    }

    public void HealHP(float _incomingFloat)
    {
        currentHP = Mathf.Clamp(currentHP + _incomingFloat, 0, MaxHp);
        OnHPChanged?.Invoke();
    }
    public void InflictLust(float _incomingFloat)
    {
        currentLust = Mathf.Clamp(currentLust + _incomingFloat, 0, MaxHp);
        OnLustChanged?.Invoke();
    }

    public void InvokeManaChange()
    {
        OnManaChanged?.Invoke();
    }

    public void OnAfterTurn()
    {
        Status?.OnAfterTurn?.Invoke(this);
        VolitileStatus?.OnAfterTurn?.Invoke(this);
    }

    public bool OnBeforeAttack()
    {
        bool _canPerformMove = true;
        if (Status?.OnBeforeAttack != null)
        {
            if (!Status.OnBeforeAttack(this))
            {
                _canPerformMove = false;
            }
            if (!VolitileStatus.OnBeforeAttack(this))
            {
                _canPerformMove = false;
            }
        }
        return _canPerformMove;
    }

    public Attack GetRandomAttack()
    {
        var _affordableAttacks = knownAttacks.Where(x => x.ManaCost <= currentMana).ToList();
        int r = UnityEngine.Random.Range(0, _affordableAttacks.Count);
        return _affordableAttacks[r];

    }
}

public class DamageDetails
{
    public bool Fainted { get; set; }
    public float Type { get; set; }

    public bool Aroused { get; set; }
}

[System.Serializable]
public class EntitySaveData
{
    //Basic values that must be saved.

    public string entityName;
    public float entityCurrentHP, entityCurrentMana, entityCurrentLust;

    //Level related values that must be saved.
    public int entityLevel;

    public float entityEXP;

    //Special values that must be saved.
    public ConditionID? entityStatusID;

    public List<AttackSaveData> attackData;
}
