using System.Collections.Generic;
using UnityEngine;

public class MapArea : MonoBehaviour
{
    [SerializeField] List<Entity> areaEnemies;

    public Entity GetRandomAreaEnemy()
    {
        var _entityHolder = areaEnemies[Random.Range(0, areaEnemies.Count)];
        _entityHolder.Init();
        return _entityHolder;
    }
}
