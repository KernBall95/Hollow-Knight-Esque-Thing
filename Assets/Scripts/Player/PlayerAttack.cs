using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Experimental.Rendering.LWRP;

[RequireComponent(typeof(Animator))]
public class PlayerAttack : MonoBehaviour {

    public int damage;
    public float knockbackStrength;

    private float attackCooldown = 0.35f;
    private float attackTime = 0.1f;
    private bool attackReady;
    public bool attackActive;
    private Animator anim;
    private SpriteRenderer attackEffect;
    private AttackHit aHit;
    private Transform weapon;
    private CameraEffects camEffects;
    
    Light2D light2D;

    void Awake()
    {
        anim = GetComponent<Animator>();
        aHit = GetComponentInChildren<AttackHit>();
        attackEffect = aHit.GetComponent<SpriteRenderer>();
        weapon = transform.Find("Player Attack Effect");        
        light2D = attackEffect.GetComponentInChildren<Light2D>();
    }
    void Start () {
        camEffects = GameManager.Instance.camEffects;

        attackEffect.enabled = false;
        light2D.enabled = false;
        attackReady = true;
        attackActive = false;
        weapon.gameObject.SetActive(false);
	}
	
	void Update () {
        if (Input.GetButtonDown("Attack") && attackReady && PlayerBase.Instance.isDashing == false)
        {
            StartCoroutine(Attack());
            anim.SetBool("Attacking", true);
        }

        if (aHit.hitEnemy && attackActive)
        {
            if (!aHit.eBase.ignoresKnockback)
            {
                Rigidbody2D enemyRB = aHit.eRB;
                StartCoroutine(ApplyKnockbackToEnemy(enemyRB));
            }
            CauseDamage(aHit.eBase);
            GameManager.Instance.audio.Play();
            attackActive = false;
        }           
    }

    IEnumerator Attack()
    {
        attackActive = true;
        weapon.gameObject.SetActive(true);
        attackReady = false;
        attackEffect.enabled = true;
        light2D.enabled = true;
        
        yield return new WaitForSeconds(attackTime);

        attackActive = false;
        weapon.gameObject.SetActive(false);
        anim.SetBool("Attacking", false);
        attackEffect.enabled = false;
        light2D.enabled = false;

        yield return new WaitForSeconds(attackCooldown - attackTime);

        attackReady = true;
    }

    void CauseDamage(EnemyBase target)
    {
        target.GetComponent<EnemyBase>().TakeDamage(damage);
        ApplyKnockbackToPlayer();
        StartCoroutine(camEffects.CameraShake(camEffects.enemyHitAmpGain, camEffects.enemyHitShakeIntensity, camEffects.enemyHitShakeLength));      
    }

    IEnumerator ApplyKnockbackToEnemy(Rigidbody2D eRB)
    {
        eRB.GetComponent<EnemyBase>().isRagdoll = true;
        Vector2 knockbackDirection = new Vector2(eRB.transform.position.x - transform.position.x, 0).normalized;
        knockbackDirection.y = 0.25f;
        eRB.AddRelativeForce(knockbackDirection * knockbackStrength, ForceMode2D.Impulse);

        yield return new WaitForSeconds(0.5f);
        
        if(eRB != null)
            eRB.GetComponent<EnemyBase>().isRagdoll = false;
    }

    void ApplyKnockbackToPlayer()
    {
        if(GetComponentInChildren<UpdateWeaponColliderPos>().downwardAttack == true)
        {
            PlayerBase.Instance.rb.velocity = Vector2.zero;
            PlayerBase.Instance.rb.AddForce(knockbackStrength/2 * transform.up, ForceMode2D.Impulse);
        }
    }
}
