using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class ConditionDataBase
{

    public static void Init()
    {
        foreach (var _keyValuePair in Conditions)
        {
            var _conditionID = _keyValuePair.Key;
            var _condition = _keyValuePair.Value;

            _condition.ID = _conditionID;
        }
    }
    public static Dictionary<ConditionID, Condition> Conditions { get; set; } = new Dictionary<ConditionID, Condition>(){
        {
        ConditionID.psn, new Condition(){
            Name = "Poison",
            StartMessage = "has been poisoned",
            OnAfterTurn = (Entity _entity) =>
                {
                    _entity.DamageHP( _entity.MaxHp/5);
                    _entity.StatusChanges.Enqueue($"{_entity.Base.EntityName} was hurt due to poison");
                }
            }
        },
        {
        ConditionID.brn, new Condition(){
            Name = "Burn",
            StartMessage = " is burning",
            OnAfterTurn = (Entity _entity) =>
                {
                    _entity.DamageHP( _entity.MaxHp/10);
                    _entity.StatusChanges.Enqueue($"{_entity.Base.EntityName} was hurt due to burning");
                }
            }
        },
        {
        ConditionID.ars, new Condition(){
            Name = "Aroused",
            StartMessage = "has been aroused",
            OnAfterTurn = (Entity _entity) =>
                {
                    _entity.InflictLust( 20/_entity.MaxLust*100);
                    _entity.StatusChanges.Enqueue($"{_entity.Base.EntityName}'s lust increases due to arousal.");
                }
            }
        },
        {
        ConditionID.par, new Condition(){
            Name = "Paralyzed",
            StartMessage = "has been paralyzed",
            OnBeforeAttack = (Entity _entity) =>
                {
                    if (Random.Range(1,5) == 1)
                    {
                    _entity.StatusChanges.Enqueue($"{_entity.Base.EntityName}'s paralasys prevented it from moving.");

                    return false;

                    } else return true;

                }
            }
        },
        {
        ConditionID.cld, new Condition(){
            Name = "Frozen",
            StartMessage = "has been frozen",
            OnBeforeAttack = (Entity _entity) =>
                {
                    if (Random.Range(1,5) == 1)
                    {
                    _entity.CureStatusCondition();
                    _entity.StatusChanges.Enqueue($"{_entity.Base.EntityName} has been unfrozen.");
                    return true;

                    } else
                    {
                    _entity.StatusChanges.Enqueue($"{_entity.Base.EntityName} is forzen and cant move.");
                     return false;
                    }
                }
            }
        },
        {
        ConditionID.slp, new Condition(){
            Name = "Sleep",
            StartMessage = " is asleep.",
            OnStart = (Entity _entity) =>{
                //Sleep for 1-3 turns
                _entity.StatusTime = Random.Range(1,4);
            },
            OnBeforeAttack = (Entity _entity) =>
                {

                    if (_entity.StatusTime <= 0)
                    {
                        _entity.CureStatusCondition();
                        _entity.StatusChanges.Enqueue($"{_entity.Base.EntityName} has woken up.");
                        return true;
                    }
                    _entity.StatusTime--;
                    _entity.StatusChanges.Enqueue($"{_entity.Base.EntityName} is sleeping.");

                    return false;

                }
            }
        },

        //Volatile Status Conditions
{
        ConditionID.iht, new Condition(){
            Name = "In Heat",
            StartMessage = " has gone into heat.",
            OnStart = (Entity _entity) =>{
                //In heat for 1-3 turns
                _entity.VolitileStatusTime = Random.Range(1,4);
            },
            OnBeforeAttack = (Entity _entity) =>
                {

                    if (_entity.VolitileStatusTime <= 0)
                    {
                        _entity.CureVolitileStatusCondition();
                        _entity.StatusChanges.Enqueue($"{_entity.Base.EntityName} has calmed down.");
                        return true;
                    }
                    _entity.VolitileStatusTime--;
                    _entity.StatusChanges.Enqueue($"{_entity.Base.EntityName} is still in heat..");

                    //50% chance to do a move
                    if (Random.Range(1,3) == 1) return true;
                    
                    //50% chance to inflict lust damage to self.
                    _entity.StatusChanges.Enqueue($"{_entity.Base.EntityName} is mastrubating.");
                    _entity.InflictLust(_entity.MaxLust/5);
                    _entity.StatusChanges.Enqueue("They have become more lustfull.");
                    return false;
                }
            }
        }

    };
}

public enum ConditionID
{
    // Poison, Burning, Sleeping, Paralyzed, Cold, Aroused
    none, psn, brn, slp, par, cld, ars,

    //Bound, InHeat
    bnd, iht

}
