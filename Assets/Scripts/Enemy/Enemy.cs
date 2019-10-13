using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
    
    public int maxHealth;
    public int currentHealth;
    public int attackDamage;
    public bool ignoresKnockback;
    //[HideInInspector]
    public bool isRagdoll;

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
