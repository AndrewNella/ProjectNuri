using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class EntityDataBase
{
    static Dictionary<string, EntityBase> entityDictionary;

    public static void Init()
    {
        entityDictionary = new Dictionary<string, EntityBase>();

        var _entityArray = Resources.LoadAll<EntityBase>("");
        foreach (var _incomingEntity in _entityArray)
        {
            if (entityDictionary.ContainsKey(_incomingEntity.EntityName))
            {
                Debug.LogError($"There are two entities with the name of {_incomingEntity.EntityName}");
                continue;
            }
            entityDictionary[_incomingEntity.EntityName] = _incomingEntity;
        }
    }

    public static EntityBase GetEntityByName(string _entityName)
    {
        if (!entityDictionary.ContainsKey(_entityName))
        {
            Debug.LogError($"Thename of {_entityName} was not found.");
            return null;
        }
        return entityDictionary[_entityName];
    }
}
