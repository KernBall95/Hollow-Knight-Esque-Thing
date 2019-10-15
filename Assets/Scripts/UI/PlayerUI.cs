using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private GameObject healthSlider;
    private float healthSliderWidth;

    void Start()
    {
        healthSliderWidth = 1;
    }

    void Update()
    {
        healthSliderWidth = (float)PlayerBase.Instance.currentHealth / (float)PlayerBase.Instance.maxHealth;
        healthSlider.transform.localScale = new Vector2(healthSliderWidth, healthSlider.transform.localScale.y);
    }
}
