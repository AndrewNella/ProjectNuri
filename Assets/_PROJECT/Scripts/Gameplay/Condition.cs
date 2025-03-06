using System;
using UnityEngine;

public class Condition
{

    public string Name { get; set; }
    public string ConditionDescription { get; set; }
    public string StartMessage { get; set; }

    public Action<Entity> OnAfterTurn { get; set; }
    public Action<Entity> OnStart { get; set; }
    public Func<Entity, bool> OnBeforeAttack { get; set; }


}
