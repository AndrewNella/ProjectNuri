using UnityEngine;

public class Attack
{
    public AttackBase Base { get; set; }

    public float ManaCost { get; set; }

    public float LustCost { get; set; }

    public Attack(AttackBase aBase)
    {
        Base = aBase;
        ManaCost = aBase.ManaCost;
        LustCost = aBase.LustCost;
    }

    public Attack(AttackSaveData _saveData)
    {
        Base = AttackDataBase.GetAttackbyName(_saveData.attackName);
    }

    public AttackSaveData GetAttackSaveData()
    {
        var _saveData = new AttackSaveData()
        {
            attackName = Base.Attackname,

        };
        return _saveData;
    }
}

[System.Serializable]
public class AttackSaveData
{
    public string attackName;

}
