using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UnitVitals : MonoBehaviour
{
    [SerializeField] Slider healthBar, manaBar, lustBar;

    [SerializeField] TMP_Text healthText, manaText, lustText;

    [SerializeField] float maxHP, maxMana, maxLust;
    [SerializeField] float animationTime = 1f;
    void Start()
    {

        // healthBar.value = healthBar.maxValue;
        // manaBar.value = manaBar.maxValue;
        // lustBar.value = 0;
    }
    public void SetMaximums(Entity _incomingEntity)
    {
        maxHP = _incomingEntity.MaxHp;
        healthBar.maxValue = maxHP;
        

        maxMana = _incomingEntity.MaxMana;
        manaBar.maxValue = maxMana;

        maxLust = _incomingEntity.Base.MaxLust;
        lustBar.maxValue = maxLust;

    }
    public void SetHP(float _incomingHP)
    {

        healthText.text = $"HP: {_incomingHP}/{maxHP}";
        AnimateHPBar(_incomingHP);
    }
    public void SetMana(float _incomingMana)
    {
        manaText.text = $"MP: {_incomingMana}/{maxMana}";
        AnimateManaBar(_incomingMana);

    }
    public void SetLust(float _incomingLust)
    {

        lustText.text = $"Lust: {_incomingLust}/{maxLust}";
        AnimateLustBar(_incomingLust);

    }
    void AnimateHPBar(float _incomingValue)
    {
        healthBar.DOValue(_incomingValue, animationTime);
    }
    void AnimateManaBar(float _incomingValue)
    {
        manaBar.DOValue(_incomingValue, animationTime);
    }
    void AnimateLustBar(float _incomingValue)
    {
        lustBar.DOValue(_incomingValue, animationTime);
    }
}
