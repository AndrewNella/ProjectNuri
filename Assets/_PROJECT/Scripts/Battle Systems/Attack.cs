using UnityEngine;

public class Attack
{
    public AttackBase Base { get; set; }

    public float ManaCost { get; set; }

    public Attack(AttackBase aBase)
    {
        Base = aBase;
        ManaCost = aBase.ManaCost;
    }
}
