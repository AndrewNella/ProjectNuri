using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System;
public class OverworldUI : MonoBehaviour
{
    [SerializeField] Slider healthBar, manaBar, lustBar;

    [SerializeField] TMP_Text healthText, manaText, lustText;
    [SerializeField] PlayerController playerController;
    [SerializeField] Slider expBar;
    [SerializeField] TMP_Text expText, levelText, nameText;

    private void Start()
    {


        UpdateHUDPlayerStats();

    }

    public void UpdateExpBar()
    {
        expText.text = $"{playerController.PlayerEntity.exp} / {playerController.PlayerEntity.Base.GetExpForLevel(playerController.PlayerEntity.Level + 1)}";
        expBar.maxValue = playerController.PlayerEntity.Base.GetExpForLevel(playerController.PlayerEntity.Level + 1);
        expBar.value = playerController.PlayerEntity.exp;

    }
    public void UpdateHUDPlayerStats()
    {
        nameText.text = playerController.PlayerEntity.Base.Name;
        levelText.text = $"LVL {playerController.PlayerEntity.Level}";

        healthText.text = $"{playerController.PlayerEntity.currentHP}/{playerController.PlayerEntity.MaxHp}";
        manaText.text = $"{playerController.PlayerEntity.currentMana}/{playerController.PlayerEntity.MaxMana}";
        lustText.text = $"{playerController.PlayerEntity.currentLust}/{playerController.PlayerEntity.MaxLust}";

        healthBar.maxValue = playerController.PlayerEntity.Base.MaxHp;
        manaBar.maxValue = playerController.PlayerEntity.Base.MaxMana;
        lustBar.maxValue = playerController.PlayerEntity.Base.MaxLust;

        healthBar.value = playerController.PlayerEntity.currentHP;
        manaBar.value = playerController.PlayerEntity.currentMana;
        lustBar.value = playerController.PlayerEntity.currentLust;



        UpdateExpBar();
    }

}
