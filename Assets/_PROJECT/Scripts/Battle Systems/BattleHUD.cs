using TMPro;
using UnityEngine;


public class BattleHUD : MonoBehaviour
{
    [SerializeField] TMP_Text nameText, levelText, statusText;
    [SerializeField] UnitVitals vitals;

    Entity currentEntity;

    public void SetData(Entity _incomingEntity)
    {

        currentEntity = _incomingEntity;
        nameText.text = _incomingEntity.Base.Name;
        SetLevel();
        
        vitals.SetMaximums(_incomingEntity);

        vitals.SetHP(_incomingEntity.currentHP);

        vitals.SetMana(_incomingEntity.currentMana);
        vitals.SetLust(_incomingEntity.currentLust);

        SetStatusText();

        currentEntity.OnStatusConditionChanged += SetStatusText;
    }

    public void SetLevel()
    {
        levelText.text = "Lvl " + currentEntity.Level;

    }
    void SetStatusText()
    {
        if (currentEntity.Status == null)
        {
            statusText.text = "";
        }
        else
        {
            statusText.text = $"{currentEntity.Status.ID.ToString().ToUpper()}";
        }
    }
    public void UpdateAll()
    {
        UpdateMana();
        UpdateHP();
        UpdateLust();
    }
    public void UpdateMana()
    {
        if (currentEntity.manaChanged)
        {

            currentEntity.manaChanged = false;
            vitals.SetMana(currentEntity.currentMana);
        }
    }
    public void UpdateHP()
    {
        if (currentEntity.hpChanged)
        {
            vitals.SetHP(currentEntity.currentHP);
            currentEntity.hpChanged = false;
        }
    }
    public void UpdateLust()
    {
        if (currentEntity.lustChanged)
        {
            vitals.SetLust(currentEntity.currentLust);
            currentEntity.lustChanged = false;
        }
    }
}
