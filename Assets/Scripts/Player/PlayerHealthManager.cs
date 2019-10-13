using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthManager : PlayerStats {

	void Start () {
        currentHealth = maxHealth;
	}

    void Update()
    {
        if (currentHealth <= 0)
            Die();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
    }

    void Die()
    {
        Destroy(gameObject);
    }
	
	
}
