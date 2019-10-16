using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackHit : MonoBehaviour
{
    [HideInInspector] public bool hitEnemy = false;
    [HideInInspector] public EnemyBase eBase;
    [HideInInspector] public Rigidbody2D eRB;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            hitEnemy = true;
            eBase = other.GetComponent<EnemyBase>();
            eRB = other.GetComponent<Rigidbody2D>();
        }           
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.tag == "Enemy")
        {
            hitEnemy = false;
        }
    }
}
