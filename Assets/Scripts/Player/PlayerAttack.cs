using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Experimental.Rendering.LWRP;

[RequireComponent(typeof(Animator))]
public class PlayerAttack : MonoBehaviour {

    public int damage;
    public float knockbackStrength;
    public float shakeAmount;
    public float shakeLength;

    private float attackCooldown = 0.35f;
    private float attackTime = 0.19f;
    private bool attackReady;
    private bool attackActive;
    private Animator anim;
    private SpriteRenderer attackEffect;
    private AttackHit aHit;
    private CinemachineVirtualCamera vCam;
    private CinemachineBasicMultiChannelPerlin noise;
    Light2D light2D;

    void Awake()
    {
        anim = GetComponent<Animator>();
        aHit = GetComponentInChildren<AttackHit>();
        attackEffect = aHit.GetComponent<SpriteRenderer>();
        vCam = GameObject.Find("CM vcam1").GetComponent<CinemachineVirtualCamera>();
        noise = vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        light2D = attackEffect.GetComponentInChildren<Light2D>();
    }
    void Start () {
        attackEffect.enabled = false;
        light2D.enabled = false;
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
            if (!aHit.eBase.ignoresKnockback)
            {
                Rigidbody2D enemyRB = aHit.eRB;
                StartCoroutine(ApplyKnockback(enemyRB));
            }
            CauseDamage(aHit.eBase);           
            attackActive = false;
        }           
    }

    IEnumerator Attack()
    {
        attackActive = true;
        attackReady = false;
        attackEffect.enabled = true;
        light2D.enabled = true;
        
        yield return new WaitForSeconds(attackTime);

        attackActive = false;
        anim.SetBool("Attacking", false);
        attackEffect.enabled = false;
        light2D.enabled = false;

        yield return new WaitForSeconds(attackCooldown - attackTime);

        attackReady = true;
    }

    void CauseDamage(EnemyBase target)
    {
        target.GetComponent<EnemyBase>().TakeDamage(damage);
        StartCoroutine(CameraShake(shakeAmount, shakeLength));
    }

    IEnumerator ApplyKnockback(Rigidbody2D eRB)
    {
        eRB.GetComponent<EnemyBase>().isRagdoll = true;
        Vector2 knockbackDirection = new Vector2(eRB.transform.position.x - transform.position.x, 0).normalized;
        knockbackDirection.y = 0.25f;
        eRB.AddRelativeForce(knockbackDirection * knockbackStrength, ForceMode2D.Impulse);

        yield return new WaitForSeconds(0.5f);
        
        if(eRB != null)
            eRB.GetComponent<EnemyBase>().isRagdoll = false;
    }

    IEnumerator CameraShake(float shakeIntensity, float shakeTiming)
    {
        Noise(1, shakeIntensity);
        yield return new WaitForSeconds(shakeTiming);
        Noise(0, 0);
    }

    void Noise(float amplitudeGain, float frequencyGain)
    {
        noise.m_AmplitudeGain = amplitudeGain;
        noise.m_FrequencyGain = frequencyGain;
    }
}
