using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
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
    public GameObject coin;
    public int coinDropAmount;
    public GameObject hitParticles;
    public LayerMask layerMask;

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
    private Vector3 normalScale = new Vector3(0.1f, 0.1f, 0.1f);
    private Vector3 flippedScale = new Vector3(-0.1f, 0.1f, 0.1f);

    void Awake() {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }
    void Start () {
        currentHealth = maxHealth;
        flipReady = true;
        rayOffsetX = 0.8f;
        rayOffsetY = -0.1f;
	}
	
	void FixedUpdate () {

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
            {
                transform.localScale  = normalScale;
                rb.AddForce(transform.right.normalized * moveSpeed);
            }
                
            else if (!movingRight)
            {
                transform.localScale = flippedScale;
                rb.AddForce(-transform.right.normalized * moveSpeed);
            }
                

            hitRight = Physics2D.Raycast(new Vector2(transform.position.x + (rayOffsetX / 6), transform.position.y), transform.right, .6f, layerMask);
            hitLeft = Physics2D.Raycast(new Vector2(transform.position.x - (rayOffsetX / 6), transform.position.y), -transform.right, .6f, layerMask);
            hitRightDown = Physics2D.Raycast(new Vector2(transform.position.x + rayOffsetX, transform.position.y + rayOffsetY), -transform.up, .4f, layerMask);
            hitLeftDown = Physics2D.Raycast(new Vector2(transform.position.x - rayOffsetX, transform.position.y + rayOffsetY), -transform.up, .4f, layerMask);

            Debug.DrawLine(new Vector2(transform.position.x + (rayOffsetX / 6), transform.position.y), new Vector2(transform.position.x + rayOffsetX + 0.6f, transform.position.y), Color.red);
            Debug.DrawLine(new Vector2(transform.position.x - (rayOffsetX / 6), transform.position.y), new Vector2(transform.position.x - rayOffsetX - 0.6f, transform.position.y), Color.red);
            Debug.DrawLine(new Vector2(transform.position.x + rayOffsetX, transform.position.y + rayOffsetY), new Vector2(transform.position.x + rayOffsetX, transform.position.y + rayOffsetY - 0.4f), Color.red);
            Debug.DrawLine(new Vector2(transform.position.x - rayOffsetX, transform.position.y + rayOffsetY), new Vector2(transform.position.x - rayOffsetX, transform.position.y + rayOffsetY - 0.4f), Color.red);

            if (flipReady && !followingPlayer)
            {   if (hitRight.collider != null)
                {
                    //if (hitRight.collider.tag == "Wall" || hitRight.collider.tag == "Floor")
                        StartCoroutine(FlipDirection());
                }
                else if (hitLeft.collider != null)
                {
                    //if (hitLeft.collider.tag == "Wall" || hitLeft.collider.tag == "Floor")
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
            movingRight = true;
        }           
        else if (PlayerBase.Instance.transform.position.x < transform.position.x)
        {
            movingRight = false;
        }
    }

    IEnumerator FlipDirection()
    {       
        flipReady = false;
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
                currentFlickerColour = "Red";

                yield return new WaitForSeconds(flickerTime);
            }
            else if (currentFlickerColour == "Red")
            {
                currentFlickerColour = "White";

                yield return new WaitForSeconds(flickerTime);
            }
        }      
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
            StartCoroutine(Die());
        StartCoroutine(FlickerWhenHit());
    }

    public void SpawnParticles(Vector3 hitPos)
    {
        Instantiate(hitParticles, hitPos, Quaternion.identity);
    }

    IEnumerator Die()
    {
        for (int i = 0; i < coinDropAmount; i++)
        {
            Instantiate(coin, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
            yield return new WaitForSeconds(0.001f);
        }
        Destroy(gameObject);
    }

    /*void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 5f);
    }*/
}
