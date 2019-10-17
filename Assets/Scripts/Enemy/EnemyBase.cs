using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class EnemyBase : MonoBehaviour {

    [SerializeField] enum EnemyType { Patrol, Follow };
    [SerializeField] EnemyType enemyType;

    public int maxHealth;
    public int attackDamage;
    public bool ignoresKnockback;    
    public float moveSpeed;
    public float jumpForce;
    public bool movingRight = true;
    public bool isRagdoll;

    private int currentHealth;   
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private float distanceToTarget;
    private bool flipReady;
    private RaycastHit2D hitLeft, hitRight, hitLeftDown, hitRightDown;
    private float rayOffsetX, rayOffsetY;
    private bool followingPlayer;
    private float flickerTime = 0.1f;
    private string currentFlickerColour = "Red";

    void Awake() {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }
    void Start () {
        currentHealth = maxHealth;
        flipReady = true;
        rayOffsetX = 0.8f;
        rayOffsetY = -0.4f;
	}
	
	void Update () {
        distanceToTarget = Vector2.Distance(PlayerBase.Instance.transform.position, transform.position);
        
        if (!isRagdoll){
            if(enemyType == EnemyType.Follow)
            {
                if (distanceToTarget < 5f)
                    MoveToPlayer();
                else
                    followingPlayer = false;
            }

            if (movingRight)
                rb.AddForce(transform.right.normalized * moveSpeed);
            else if (!movingRight)
                rb.AddForce(-transform.right.normalized * moveSpeed);

            hitRight = Physics2D.Raycast(new Vector2(transform.position.x + (rayOffsetX / 2), transform.position.y), transform.right, .6f);
            hitLeft = Physics2D.Raycast(new Vector2(transform.position.x - (rayOffsetX / 2), transform.position.y), -transform.right, .6f);
            hitRightDown = Physics2D.Raycast(new Vector2(transform.position.x + rayOffsetX, transform.position.y + rayOffsetY), -transform.up, .4f);
            hitLeftDown = Physics2D.Raycast(new Vector2(transform.position.x - rayOffsetX, transform.position.y + rayOffsetY), -transform.up, .4f);

            Debug.DrawLine(new Vector2(transform.position.x + (rayOffsetX / 2), transform.position.y), new Vector2(transform.position.x + rayOffsetX + 0.6f, transform.position.y), Color.red);
            Debug.DrawLine(new Vector2(transform.position.x - (rayOffsetX / 2), transform.position.y), new Vector2(transform.position.x - rayOffsetX - 0.6f, transform.position.y), Color.red);
            Debug.DrawLine(new Vector2(transform.position.x + rayOffsetX, transform.position.y + rayOffsetY), new Vector2(transform.position.x + rayOffsetX, transform.position.y + rayOffsetY - 0.4f), Color.red);
            Debug.DrawLine(new Vector2(transform.position.x - rayOffsetX, transform.position.y + rayOffsetY), new Vector2(transform.position.x - rayOffsetX, transform.position.y + rayOffsetY - 0.4f), Color.red);

            if (flipReady && !followingPlayer)
            {   if (hitRight.collider != null)
                {
                    if (hitRight.collider.tag == "Wall" || hitRight.collider.tag == "Floor")
                        StartCoroutine(FlipDirection());
                }
                else if (hitLeft.collider != null)
                {
                    if (hitLeft.collider.tag == "Wall" || hitLeft.collider.tag == "Floor")
                        StartCoroutine(FlipDirection());
                }
                else if (hitRightDown.collider == null)
                    StartCoroutine(FlipDirection());

                else if (hitLeftDown.collider == null)
                    StartCoroutine(FlipDirection());
            }

            if (followingPlayer)
            {
                if (hitRight.collider != null)
                {
                    if (hitRight.collider.tag == "Wall" || hitRight.collider.tag == "Floor")
                        rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
                }
                else if (hitLeft.collider != null)
                {
                    if (hitLeft.collider.tag == "Wall" || hitLeft.collider.tag == "Floor")
                        rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
                }
            }
        }               
	}

    void MoveToPlayer()
    {
        followingPlayer = true;
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
    }

    IEnumerator FlipDirection()
    {       
        flipReady = false;

        if (sr.flipX == true)
            sr.flipX = false;
        else if (sr.flipX == false)
            sr.flipX = true;

        movingRight = !movingRight;
        yield return new WaitForSeconds(0.5f);
        flipReady = true;
    }
    IEnumerator FlickerWhenHit()
    {
        while (isRagdoll == true)
        {
            if (currentFlickerColour == "White")
            {
                sr.color = new Color(255f, 0f, 0f, 255f);
                currentFlickerColour = "Red";

                yield return new WaitForSeconds(flickerTime);
            }
            else if (currentFlickerColour == "Red")
            {
                sr.color = new Color(255f, 255f, 255f, 255f);
                currentFlickerColour = "White";

                yield return new WaitForSeconds(flickerTime);
            }
        }      
        sr.color = new Color(255f, 0f, 0f, 255f);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
            Die();
        StartCoroutine(FlickerWhenHit());
    }

    void Die()
    {
        Destroy(gameObject);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 5f);
    }
}
