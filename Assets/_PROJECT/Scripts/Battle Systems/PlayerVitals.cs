using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerVitals : MonoBehaviour
{
    [SerializeField] Slider healthBar, manaBar, lustBar;

    [SerializeField] TMP_Text healthText, manaText, lustText;
    void Start()
    {
        healthBar.value = healthBar.maxValue;
        manaBar.value = manaBar.maxValue;
        lustBar.value = 0;
    }

    public void SetHP(float _incomingHP)
    {
        Debug.Log(_incomingHP);

        healthText.text = $"HP: {_incomingHP}/100";
        healthBar.value = _incomingHP;
    }
    public void SetMana(float _incomingMana)
    {
        healthText.text = $"MP: {_incomingMana}/100";
        manaBar.value = _incomingMana;

    }
    public void SetLust(float _incomingLust)
    {

        lustText.text = $"Lust: {_incomingLust}/100";
        lustBar.value = _incomingLust;

    }
}
