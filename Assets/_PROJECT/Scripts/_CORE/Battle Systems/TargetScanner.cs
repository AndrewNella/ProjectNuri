using UnityEngine;

public class TargetScanner : MonoBehaviour
{
    public float scanRange;
    public LayerMask targetLayer;
    public RaycastHit2D[] targets;
    public Transform nearestTarget;

    void FixedUpdate()
    {
        targets = Physics2D.CircleCastAll(transform.position, scanRange, Vector2.zero, 0, targetLayer);
        nearestTarget = GetNearestTarget();
    }

    Transform GetNearestTarget()
    {
        Transform _result = null;
        float _diff = 100f;

        foreach (RaycastHit2D _target in targets)
        {
            Vector3 myPos = transform.position;
            Vector3 targetPos = _target.transform.position;

            float currentDiff = Vector3.Distance(myPos, targetPos);

            if(currentDiff < _diff)
            {
                _diff = currentDiff;
                _result = _target.transform;
            }
        }

        return _result;
    }
}
