using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class PlayerTakeDamage : MonoBehaviour {

    public float knockbackStrength;
    public float knockbackTime;
    public float invulnTime;
    public float alphaFlickerTime;
    [HideInInspector]public bool isRagdoll;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Vector2 knockbackDirection;
    private bool isInvuln;


    void Awake() {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }
    void Start () {        
        isRagdoll = false;
        isInvuln = false;
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
        Physics2D.IgnoreLayerCollision(9, 11, true);

        yield return new WaitForSeconds(invulnTime);

        Physics2D.IgnoreLayerCollision(9, 11, false);
        isInvuln = false;
    }

    /*IEnumerator FlashSprite()
    {
        while (isInvuln)
        {
            if (alphaHigh)
            {
                sr.color = new Color(1f, 1f, 1f, 0.4f);
                alphaHigh = false;
                alphaLow = true;
                yield return new WaitForSeconds(alphaFlickerTime);
            }
            else if (alphaLow)
            {
                sr.color = new Color(1f, 1f, 1f, 1f);
                alphaLow = false;
                alphaHigh = true;
                yield return new WaitForSeconds(alphaFlickerTime);
            }                              
        }
        sr.color = new Color(1f, 1f, 1f, 1f);
    }*/

    void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag == "Enemy")
        {
            if (!isInvuln)
            {
                int damage = other.gameObject.GetComponent<EnemyBase>().attackDamage;

                knockbackDirection = new Vector2(transform.position.x - other.transform.position.x, Vector2.up.y).normalized;

                PlayerBase.Instance.TakeDamage(damage);
                StartCoroutine(KnockbackPlayer());
                StartCoroutine(TempInvuln());
                //StartCoroutine(FlashSprite());
            }          
        }
    }
}
