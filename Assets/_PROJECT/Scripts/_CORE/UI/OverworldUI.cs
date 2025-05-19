using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System;
using Kisei.Player;
using System.Collections;
using Sirenix.OdinInspector;
public class OverworldUI : MonoBehaviour
{
    [SerializeField] Slider healthBar, manaBar, lustBar;

    [SerializeField] TMP_Text healthText, manaText, lustText;
    [SerializeField] Slider expBar;
    [SerializeField] TMP_Text expText, levelText, nameText;

    private float maxHP, maxMana, maxLust;

    Entity playerEntity;

    public void SetMaximums(Entity _incomingEntity)
    {
        maxHP = _incomingEntity.MaxHp;
        healthBar.maxValue = maxHP;

        maxMana = _incomingEntity.MaxMana;
        manaBar.maxValue = maxMana;

        maxLust = _incomingEntity.Base.MaxLust;
        lustBar.maxValue = maxLust;
    }
    private void Start()
    {
        StartCoroutine(LoadData());

    }
    IEnumerator LoadData()
    {
        yield return new WaitForEndOfFrame();

        playerEntity = PlayerInstanceHUB.Instance.PlayerController.PlayerEntity; 

        SetMaximums(playerEntity);

        UpdateHUDPlayerStats();
        playerEntity.OnHPChanged += UpdateHUDHealth;
        playerEntity.OnManaChanged += UpdateHUDHealth;
        playerEntity.OnLustChanged += UpdateHUDLust;
    }



    void OnDisable()
    {
        playerEntity.OnHPChanged -= UpdateHUDHealth;
        playerEntity.OnManaChanged -= UpdateHUDHealth;
        playerEntity.OnLustChanged -= UpdateHUDLust;
    }

    public void UpdateExpBar()
    {
        expText.text = $"{playerEntity.exp} / {playerEntity.Base.GetExpForLevel(playerEntity.Level + 1)}";
        expBar.maxValue = playerEntity.Base.GetExpForLevel(playerEntity.Level + 1);
        expBar.value = playerEntity.exp;

    }

    [Button("Update Player Data")]
    public void UpdateHUDPlayerStats()
    {
        nameText.text = playerEntity.Base.EntityName;
        levelText.text = $"LVL {playerEntity.Level}";

        UpdateHUDHealth();
        UpdateHUDMana();
        UpdateHUDLust();

        UpdateExpBar();
    }

    public void UpdateHUDHealth()
    {
        healthText.text = $"{playerEntity.currentHP}/{maxLust}";

        healthBar.maxValue = maxHP;
        healthBar.value = playerEntity.currentHP;

    }

    public void UpdateHUDMana()
    {
        manaText.text = $"{playerEntity.currentMana}/{maxMana}";

        manaBar.maxValue = maxMana;
        manaBar.value = playerEntity.currentMana;
    }

    public void UpdateHUDLust()
    {

        lustText.text = $"{playerEntity.currentLust}/{maxLust}";
        lustBar.maxValue = maxLust;
        lustBar.value = playerEntity.currentLust;
    }

}
