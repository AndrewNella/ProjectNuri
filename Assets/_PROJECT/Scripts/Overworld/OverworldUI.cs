using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System;
using System.Collections;
public class OverworldUI : MonoBehaviour
{
    [SerializeField] Slider healthBar, manaBar, lustBar;

    [SerializeField] TMP_Text healthText, manaText, lustText;
    [SerializeField] Slider expBar;
    [SerializeField] TMP_Text expText, levelText, nameText;

    private void Awake()
    {

        StartCoroutine(LoadData());
    }

    IEnumerator LoadData()
    {
        yield return new WaitForEndOfFrame();
        UpdateHUDPlayerStats();
    }

    public void UpdateExpBar()
    {
        expText.text = $"{PlayerController.instance.PlayerEntity.exp} / {PlayerController.instance.PlayerEntity.Base.GetExpForLevel(PlayerController.instance.PlayerEntity.Level + 1)}";
        expBar.maxValue = PlayerController.instance.PlayerEntity.Base.GetExpForLevel(PlayerController.instance.PlayerEntity.Level + 1);
        expBar.value = PlayerController.instance.PlayerEntity.exp;

    }
    public void UpdateHUDPlayerStats()
    {
        nameText.text = PlayerController.instance.PlayerEntity.Base.Name;
        levelText.text = $"LVL {PlayerController.instance.PlayerEntity.Level}";

        healthText.text = $"{PlayerController.instance.PlayerEntity.currentHP}/{PlayerController.instance.PlayerEntity.MaxHp}";
        manaText.text = $"{PlayerController.instance.PlayerEntity.currentMana}/{PlayerController.instance.PlayerEntity.MaxMana}";
        lustText.text = $"{PlayerController.instance.PlayerEntity.currentLust}/{PlayerController.instance.PlayerEntity.MaxLust}";

        healthBar.maxValue = PlayerController.instance.PlayerEntity.Base.MaxHp;
        manaBar.maxValue = PlayerController.instance.PlayerEntity.Base.MaxMana;
        lustBar.maxValue = PlayerController.instance.PlayerEntity.Base.MaxLust;

        healthBar.value = PlayerController.instance.PlayerEntity.currentHP;
        manaBar.value = PlayerController.instance.PlayerEntity.currentMana;
        lustBar.value = PlayerController.instance.PlayerEntity.currentLust;



        UpdateExpBar();
    }

}
