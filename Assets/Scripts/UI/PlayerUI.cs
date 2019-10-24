using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private GameObject healthSlider;
    private float healthSliderWidth;
    [SerializeField] private TextMeshProUGUI currencyCount;
    private int currentMoney;

    void Start()
    {
        healthSliderWidth = 1;
        currentMoney = 0;
    }

    void Update()
    {
        if (healthSliderWidth != (float)PlayerBase.Instance.currentHealth / (float)PlayerBase.Instance.maxHealth)
            UpdateHealthBar();

        if (currentMoney != PlayerBase.Instance.currentCash)
            ChangeMoneyAmount();
        
    }

    void ChangeMoneyAmount()
    {
        currentMoney = PlayerBase.Instance.currentCash;
        currencyCount.text = currentMoney.ToString();
    }

    void UpdateHealthBar()
    {
        healthSliderWidth = (float)PlayerBase.Instance.currentHealth / (float)PlayerBase.Instance.maxHealth;
        healthSlider.transform.localScale = new Vector2(healthSliderWidth, healthSlider.transform.localScale.y);
    }
}
