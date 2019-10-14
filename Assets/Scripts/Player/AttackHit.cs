using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackHit : MonoBehaviour
{
    [HideInInspector] public bool hitEnemy = false;
    [HideInInspector] public EnemyManager eManager;
    [HideInInspector] public Rigidbody2D eRB;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            hitEnemy = true;
            eManager = other.GetComponent<EnemyManager>();
            eRB = other.GetComponent<Rigidbody2D>();
        }
            
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.tag == "Enemy")
        {
            hitEnemy = false;
        }
    }
}
