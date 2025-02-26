using TMPro;
using UnityEngine;


public class BattleHUD : MonoBehaviour
{
    [SerializeField] TMP_Text nameText, levelText;
    [SerializeField] PlayerVitals vitals;

    Entity currentEntity;

    public void SetData(Entity _incomingEntity)
    {

        currentEntity = _incomingEntity;
        nameText.text = _incomingEntity.Base.Name;
        levelText.text = "Lvl " + _incomingEntity.Level;

        vitals.SetMaximums(_incomingEntity);

        vitals.SetHP(_incomingEntity.currentHP);

        vitals.SetMana(_incomingEntity.currentMana);
        vitals.SetLust(_incomingEntity.currentLust);
    }


    public void UpdateMana()
    {
        vitals.AnimateManaBar(currentEntity.currentMana);
    }
    public void UpdateHP()
    {
        vitals.AnimateHPBar(currentEntity.currentHP);
    }
    public void UpdateLust()
    {
        vitals.AnimateLustBar(currentEntity.currentLust);
    }
}
