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
        nameText.text = _incomingEntity.Base.EntityName;
        SetLevel();
        SetListenersForEntity();

        vitals.SetMaximums(_incomingEntity);

        vitals.SetHP(_incomingEntity.currentHP);

        vitals.SetMana(_incomingEntity.currentMana);
        vitals.SetLust(_incomingEntity.currentLust);

        SetStatusText();


        currentEntity.OnStatusConditionChanged += SetStatusText;
    }

    void SetListenersForEntity()
    {
        currentEntity.OnHPChanged += UpdateHP;
        currentEntity.OnManaChanged += UpdateMana;
        currentEntity.OnLustChanged += UpdateLust;
    }
    void RemoveListenersForEntity()
    {
        currentEntity.OnHPChanged -= UpdateHP;
        currentEntity.OnManaChanged -= UpdateMana;
        currentEntity.OnLustChanged -= UpdateLust;
    }

    private void OnDisable()
    {
        RemoveListenersForEntity();
    }

    private void OnDestroy()
    {
        RemoveListenersForEntity();
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

        vitals.SetMana(currentEntity.currentMana);

    }
    public void UpdateHP()
    {

        vitals.SetHP(currentEntity.currentHP);

    }
    public void UpdateLust()
    {

        vitals.SetLust(currentEntity.currentLust);

    }
}
