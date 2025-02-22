using UnityEngine;

public class AttackConstructor
{
    public AttackBase Base { get; set; }

    public float ManaCost { get; set; }

    public AttackConstructor(AttackBase aBase)
    {
        Base = aBase;
        ManaCost = aBase.ManaCost;
    }
}
