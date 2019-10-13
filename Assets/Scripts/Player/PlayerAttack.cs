using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour {

    public int damage;
    public float knockbackStrength;
    public GameObject attackEffect;

    float effectTime = 0.2f;
    float attackCooldown = 0.4f;
    float attackTime = 0.1f;
    bool attackReady;
    GameObject targetHit;
    bool attackActive;

	void Start () {
        attackReady = true;
        attackActive = false;
	}
	
	void Update () {

        if (Input.GetButtonDown("Attack") && attackReady)
            StartCoroutine(Attack());
    }

    IEnumerator Attack()
    {
        attackActive = true;
        attackReady = false;
        
        attackEffect.SetActive(true);

        yield return new WaitForSeconds(attackTime);
        attackActive = false;
        yield return new WaitForSeconds(effectTime - attackTime);
        attackEffect.SetActive(false);
        yield return new WaitForSeconds(attackCooldown - attackTime - effectTime);
        attackReady = true;
    }

    void CauseDamage(GameObject target)
    {
        target.GetComponent<EnemyManager>().TakeDamage(damage);
        
    }

    IEnumerator ApplyKnockback(Rigidbody2D eRB)
    {
        eRB.GetComponent<EnemyManager>().isRagdoll = true;
        Vector2 knockbackDirection = new Vector2(eRB.transform.position.x - transform.position.x, 0).normalized;
        knockbackDirection.y = 0.25f;
        eRB.AddRelativeForce(knockbackDirection * knockbackStrength, ForceMode2D.Impulse);

        yield return new WaitForSeconds(0.25f);
        eRB.GetComponent<EnemyManager>().isRagdoll = false;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Enemy" && attackActive)
        {
            CauseDamage(other.gameObject);

            if (other.GetComponent<EnemyManager>().ignoresKnockback == false)
            {
                Rigidbody2D enemyRB = other.GetComponent<Rigidbody2D>();
                StartCoroutine(ApplyKnockback(enemyRB));                
            }
                

            attackActive = false;
        }
    }
}
