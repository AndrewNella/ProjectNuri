using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerVitals : MonoBehaviour
{
    [SerializeField] Slider healthBar, manaBar, lustBar;

    [SerializeField] TMP_Text healthText, manaText, lustText;

    [SerializeField] float maxHP, maxMana, maxLust;
    void Start()
    {

        // healthBar.value = healthBar.maxValue;
        // manaBar.value = manaBar.maxValue;
        // lustBar.value = 0;
    }
    public void SetMaximums(Entity _incomingEntity)
    {
        maxHP = _incomingEntity.currentHP;
        healthBar.maxValue = maxHP;

        maxMana = _incomingEntity.currentMana;
        manaBar.maxValue = maxMana;

        maxLust = _incomingEntity.Base.MaxLust;
        lustBar.maxValue = maxLust;

    }
    public void SetHP(float _incomingHP)
    {

        healthText.text = $"HP: {_incomingHP}/{maxHP}";
        healthBar.value = _incomingHP;
    }
    public void SetMana(float _incomingMana)
    {
        manaText.text = $"MP: {_incomingMana}/{maxMana}";
        manaBar.value = _incomingMana;

    }
    public void SetLust(float _incomingLust)
    {

        lustText.text = $"Lust: {_incomingLust}/{maxLust}";
        lustBar.value = _incomingLust;

    }
    public void SetHP(float _incomingHP, float _incomingMax)
    {

        healthText.text = $"HP: {_incomingHP}/{_incomingMax}";
        healthBar.value = _incomingHP;
    }
    public void SetMana(float _incomingMana, float _incomingMax)
    {
        healthText.text = $"MP: {_incomingMana}/{_incomingMax}";
        manaBar.value = _incomingMana;

    }
    public void SetLust(float _incomingLust, float _incomingMax)
    {

        lustText.text = $"Lust: {_incomingLust}/{_incomingMax}";
        lustBar.value = _incomingLust;

    }
}
