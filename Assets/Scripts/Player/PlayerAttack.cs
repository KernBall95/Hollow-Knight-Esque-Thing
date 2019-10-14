using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour {

    public int damage;
    public float knockbackStrength;
   

    //private float effectTime = 0.2f;
    private float attackCooldown = 0.4f;
    private float attackTime = 0.2f;
    private bool attackReady;
    //private GameObject targetHit;
    private bool attackActive;
    private Animator anim;
    private SpriteRenderer attackEffect;

    [SerializeField] private AttackHit aHit;

	void Start () {
        anim = GetComponent<Animator>();
        aHit = GetComponentInChildren<AttackHit>();
        attackEffect = aHit.GetComponent<SpriteRenderer>();

        attackReady = true;
        attackActive = false;
	}
	
	void Update () {
        if (Input.GetButtonDown("Attack") && attackReady && PlayerBase.Instance.isDashing == false)
        {
            StartCoroutine(Attack());
            anim.SetBool("Attacking", true);
        }

        if (aHit.hitEnemy && attackActive)
        {
            CauseDamage(aHit.eManager);
            if (!aHit.eManager.ignoresKnockback)
            {
                Rigidbody2D enemyRB = aHit.eRB;
                StartCoroutine(ApplyKnockback(enemyRB));
            }
            attackActive = false;
        }           
    }

    IEnumerator Attack()
    {
        attackActive = true;
        attackReady = false;
        attackEffect.enabled = true;

        //yield return new WaitForSeconds(attackTime);
        
        yield return new WaitForSeconds(attackTime);
        attackActive = false;
        anim.SetBool("Attacking", false);
        attackEffect.enabled = false;
        yield return new WaitForSeconds(attackCooldown - attackTime);
        attackReady = true;
    }

    void CauseDamage(EnemyManager target)
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
}
