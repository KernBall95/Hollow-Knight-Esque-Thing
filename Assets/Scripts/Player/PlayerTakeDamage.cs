using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class PlayerTakeDamage : MonoBehaviour {

    public float knockbackStrength;
    public float knockbackTime;
    public float invulnTime;
    public float flickerTime;
    [HideInInspector]public bool isRagdoll;

    private Rigidbody2D rb;
    public List<SpriteRenderer> sr = new List<SpriteRenderer>();
    private Vector2 knockbackDirection;
    private bool isInvuln;
    private float currentAlpha;
    private CameraEffects camEffects;


    void Awake() {
        rb = GetComponent<Rigidbody2D>();
        
    }
    void Start () {
        camEffects = GameManager.Instance.camEffects;

        isRagdoll = false;
        isInvuln = false;
        currentAlpha = 1;

        foreach(Transform child in transform)
        {
            if (child.GetComponent<SpriteRenderer>() != null)
                sr.Add(child.GetComponent<SpriteRenderer>());
        }
	}

    IEnumerator KnockbackPlayer()
    {
        rb.gravityScale = 1;
        rb.velocity = Vector2.zero;
        rb.AddForce(knockbackDirection * knockbackStrength, ForceMode2D.Impulse);
        isRagdoll = true;       

        yield return new WaitForSeconds(knockbackTime);
        isRagdoll = false;
    }

    IEnumerator TempInvuln()
    {
        isInvuln = true;

        yield return new WaitForSeconds(invulnTime);

        isInvuln = false;
    }

    IEnumerator FlashSprite()
    {
        while (isInvuln)
        {
            if (currentAlpha == 1f)
            {
                currentAlpha = 0.5f;
                for(int i = 0; i < sr.Count; i++)
                {
                    sr[i].color = new Color(sr[i].color.r, sr[i].color.g, sr[i].color.b, currentAlpha);
                }
                //sr.color = new Color(1f, 1f, 1f, 0.4f);
                yield return new WaitForSeconds(flickerTime);
            }
            else if (currentAlpha == 0.5f)
            {
                currentAlpha = 1f;
                for(int i = 0; i < sr.Count; i++)
                {
                    sr[i].color = new Color(sr[i].color.r, sr[i].color.g, sr[i].color.b, currentAlpha);
                }              
                yield return new WaitForSeconds(flickerTime);
            }                              
        }
        for(int i = 0; i < sr.Count;i++)
            sr[i].color = new Color(sr[i].color.r, sr[i].color.g, sr[i].color.b, 1);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag == "Enemy")
        {
            if (!isInvuln)
            {
                int damage = other.gameObject.GetComponent<EnemyBase>().attackDamage;

                knockbackDirection = new Vector2(transform.position.x - other.transform.position.x, Vector2.up.y).normalized;                

                if(PlayerBase.Instance.currentHealth > 1)
                {
                    StartCoroutine(KnockbackPlayer());
                    StartCoroutine(TempInvuln());
                    StartCoroutine(camEffects.CameraShake(camEffects.playerHitAmpGain, camEffects.playerHitShakeIntensity, camEffects.playerHitShakeLength));
                    StartCoroutine(FlashSprite());
                }
                PlayerBase.Instance.TakeDamage(damage);                
            }          
        }
    }
}
