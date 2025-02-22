using TMPro;
using UnityEngine;

public class BattleHUD : MonoBehaviour
{
    [SerializeField] TMP_Text nameText, levelText;
    [SerializeField] PlayerVitals vitals;

    public void SetData(Entity _incomingEntity)
    {
        nameText.text = _incomingEntity.Base.Name;
        levelText.text = "Lvl " + _incomingEntity.Level;

        vitals.SetHP(_incomingEntity.currentHP);

        vitals.SetMana(_incomingEntity.currentMana);
        vitals.SetLust(_incomingEntity.currentLust);

    }
}
