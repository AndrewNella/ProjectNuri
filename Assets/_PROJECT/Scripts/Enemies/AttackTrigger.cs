using UnityEngine;

public class AttackTrigger : MonoBehaviour
{
    bool isTrapTriggered = false;
    [SerializeField] FieldMonsterBase fieldBase;




    private void Awake()
    {
        ResetTrap();
        fieldBase = GetComponent<FieldMonsterBase>();
    }

    public void ResetTrap()
    {
        isTrapTriggered = false;
    }
    void OnTriggerEnter2D(Collider2D other)
    {

        if (((1 << other.gameObject.layer) & GameLayers.Instance.PlayerLayer) != 0)
        {
            isTrapTriggered = true;
            fieldBase.TriggerAttackFromThisEntity();
        }
    }
}
