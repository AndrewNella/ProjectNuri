using System.Collections.Generic;
using UnityEngine;


public class ConditionDataBase
{


    public static Dictionary<ConditionID, Condition> Conditions { get; set; } = new Dictionary<ConditionID, Condition>(){
        {
        ConditionID.psn, new Condition(){
            Name = "Poison",
            StartMessage = "has been poisoned",
            OnAfterTurn = (Entity _entity) =>
                {
                    _entity.DamageHP( _entity.MaxHp/5);
                    _entity.StatusChanges.Enqueue($"{_entity.Base.Name} was hurt due to poison");
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
                    _entity.StatusChanges.Enqueue($"{_entity.Base.Name} was hurt due to burning");
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
                    _entity.StatusChanges.Enqueue($"{_entity.Base.Name}'s lust increases due to arousal.");
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
                    _entity.StatusChanges.Enqueue($"{_entity.Base.Name}'s paralasys prevented it from moving.");

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
                    _entity.StatusChanges.Enqueue($"{_entity.Base.Name} has been unfrozen.");
                    return true;

                    } else
                    {
                    _entity.StatusChanges.Enqueue($"{_entity.Base.Name} is forzen and cant move.");
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
                        _entity.StatusChanges.Enqueue($"{_entity.Base.Name} has woken up.");
                        return true;
                    }
                    _entity.StatusTime--;
                    _entity.StatusChanges.Enqueue($"{_entity.Base.Name} is sleeping.");

                    return false;

                }
            }
        }
    };
}

public enum ConditionID
{
    // Poison, Burning, Sleeping, Paralyzed, Cold, Aroused
    none, psn, brn, slp, par, cld, ars
}
