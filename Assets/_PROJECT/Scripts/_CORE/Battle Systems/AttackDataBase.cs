using UnityEngine;
using System.Collections.Generic;

public class AttackDataBase
{
    static Dictionary<string, AttackBase> _attackDictionary;

    public static void Init()
    {
        _attackDictionary = new Dictionary<string, AttackBase>();

        var _attackArray = Resources.LoadAll<AttackBase>("");
        foreach (var _incomingAttack in _attackArray)
        {
            if (_attackDictionary.ContainsKey(_incomingAttack.Attackname))
            {
                Debug.LogError($"There are two attacks with the name of {_incomingAttack.Attackname}");
                continue;
            }
            _attackDictionary[_incomingAttack.Attackname] = _incomingAttack;
        }
    }

    public static AttackBase GetAttackbyName(string _attackname)
    {
        if (!_attackDictionary.ContainsKey(_attackname))
        {
            Debug.LogError($"The name of {_attackname} was not found.");
            return null;
        }
        return _attackDictionary[_attackname];
    }
}
