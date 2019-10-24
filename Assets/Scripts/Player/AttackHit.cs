using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackHit : MonoBehaviour
{
    public bool hitEnemy = false;
    public EnemyBase eBase;
    public Rigidbody2D eRB;

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            hitEnemy = true;
            eBase = other.gameObject.GetComponent<EnemyBase>();
            eRB = other.GetComponent<Rigidbody2D>();

            Vector3 collisionPos = other.bounds.ClosestPoint(transform.position);
            other.GetComponent<EnemyBase>().SpawnParticles(collisionPos);  
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
