using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour {

    [SerializeField] enum EnemyType { Patrol, Follow };
    [SerializeField] EnemyType enemyType;

    public int maxHealth;
    public int attackDamage;
    public bool ignoresKnockback;    
    public float moveSpeed;
    public bool movingRight = true;
    [HideInInspector]public bool isRagdoll;

    private int currentHealth;   
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private float distanceToTarget;
    private Vector3 direction;
    private bool flipReady;
    private RaycastHit2D hit;

    void Awake() {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }
    void Start () {
        currentHealth = maxHealth;
        flipReady = true;
	}
	
	void FixedUpdate () {
        distanceToTarget = Vector2.Distance(PlayerBase.Instance.transform.position, transform.position);

        if (!isRagdoll){
            if(enemyType == EnemyType.Follow)
            {
                if (distanceToTarget < 4f)
                    MoveToPlayer();                              
            }

            if (movingRight)
            {
                hit = Physics2D.Raycast(transform.position, transform.right, .9f);
                rb.AddForce(transform.right * moveSpeed);
                Debug.DrawLine(transform.position, new Vector2(transform.position.x + 0.7f, transform.position.y), Color.red);
            }
            else if (!movingRight)
            {
                hit = Physics2D.Raycast(transform.position, -transform.right, .9f);
                rb.AddForce(-transform.right * moveSpeed);
                Debug.DrawLine(transform.position, new Vector2(transform.position.x - 0.7f, transform.position.y), Color.red);
            }
            direction = rb.velocity.normalized;


            if (hit.collider != null && flipReady)
            {
                if (hit.collider.tag == "Wall" || hit.collider.tag == "Floor")
                    StartCoroutine(FlipDirection());              
            }         
        }               
	}

    void MoveToPlayer()
    {
        direction = new Vector3(PlayerBase.Instance.transform.position.x - transform.position.x, transform.position.y, transform.position.z).normalized;
        direction.y = 0;
        if (PlayerBase.Instance.transform.position.x > transform.position.x)
        {
            sr.flipX = false;
            movingRight = true;

        }           
        else if (PlayerBase.Instance.transform.position.x < transform.position.x)
        {
            sr.flipX = true;
            movingRight = false;
        }
  
        rb.AddForce(direction * moveSpeed);
    }

    IEnumerator FlipDirection()
    {       
        flipReady = false;

        if (sr.flipX == true)
            sr.flipX = false;
        else if (sr.flipX == false)
            sr.flipX = true;

        movingRight = !movingRight;
        yield return new WaitForSeconds(0.25f);
        flipReady = true;
    }

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
