using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D),typeof(Animator), typeof(PlayerTakeDamage))]
public class PlayerBase : MonoBehaviour
{
    [HideInInspector] public int currentHealth;
    public int maxHealth;   
    public float speed;
    public float jumpSpeed;
    public float dashSpeed;
    public float maxDashLength = 0.4f;
    public float maxDashCooldown = 0.75f;
    public bool isGrounded;  
    public int currentCash;
    public Animator anim;
    [HideInInspector] public bool isDashing = true;
    [HideInInspector] public float h;
    [HideInInspector] public float v;
    [HideInInspector] public Rigidbody2D rb;
  
    private float lessGravityTime;
    private float maxLessGravityTime = 0.4f;  
    private float currentDashCooldown = 0f;
    private float dashTimer;
    private bool isJumping = true;
    private bool hasDoubleJump = true;
    private bool doubleJumpUsed;
    private bool doubleJumpReady;
    private bool jumpReady;   
    private bool dashComplete;
    private bool groundedSinceLastDash;
    private bool isRagdoll;
    public bool movingRight = true;
    private Vector3 normalScale = new Vector3(0.1f, 0.1f, 1f);
    private Vector3 flippedScaleX = new Vector3(-0.1f, 0.1f, 1f);
    [SerializeField] private GameObject startUp;
    private RaycastHit2D hitBack;
    private RaycastHit2D hitFront;
    private float groundedRayXOffset = 0.29f;
    private CapsuleCollider2D col;
    private bool atLever = false;
    private Lever lever;

    private static PlayerBase instance;
    public static PlayerBase Instance
    {
        get
        {
            if (instance == null)
                instance = GameObject.FindObjectOfType<PlayerBase>();
            return instance;
        }
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        col = GetComponent<CapsuleCollider2D>();
    }

    void Start()
    {
        currentHealth = maxHealth;
        jumpReady = true;
        dashTimer = maxDashLength;
        currentCash = 0;
    }

    void Update()
    {
        if(isRagdoll != GetComponent<PlayerTakeDamage>().isRagdoll)
            isRagdoll = GetComponent<PlayerTakeDamage>().isRagdoll;

        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        currentDashCooldown -= Time.deltaTime;
        dashTimer += Time.deltaTime;
        lessGravityTime += Time.deltaTime;

        hitBack = Physics2D.Raycast(new Vector3(transform.position.x - groundedRayXOffset, transform.position.y, transform.position.z), -transform.up, .9f);
        hitFront = Physics2D.Raycast(new Vector3(transform.position.x + groundedRayXOffset, transform.position.y, transform.position.z), -transform.up, .9f);
        //Debug.DrawLine(new Vector2(transform.position.x - groundedRayXOffset, transform.position.y), new Vector3(transform.position.x - groundedRayXOffset, transform.position.y, transform.position.z) - (transform.up * .4f), Color.red);
        //Debug.DrawLine(new Vector2(transform.position.x + groundedRayXOffset, transform.position.y), new Vector3(transform.position.x + groundedRayXOffset, transform.position.y, transform.position.z) - (transform.up * .4f), Color.red);

        if (hitBack.collider != null)
        {
            if (hitBack.collider.tag == "Floor" && isGrounded == false) 
                isGrounded = true;
        }
        else if(hitFront.collider != null)
        {
            if (hitFront.collider.tag == "Floor" && isGrounded == false)
                isGrounded = true;
        }
        else
            isGrounded = false;
        
        if (isGrounded && isJumping == true)
        {
            groundedSinceLastDash = true;
            anim.SetBool("Ground", true);
            isJumping = false;
            doubleJumpUsed = false;
            col.sharedMaterial.friction = 0.4f;
        }
        else if (!isGrounded && isJumping == false)
        {
            isJumping = true;
            anim.SetBool("Ground", false);
            col.sharedMaterial.friction = 0f;
        }
            
        if (movingRight && transform.localScale != normalScale)
        {
            transform.localScale = normalScale;
            groundedRayXOffset = 0.29f;
        }
        else if (!movingRight && transform.localScale != flippedScaleX)
        {
            transform.localScale = flippedScaleX;
            groundedRayXOffset = -0.29f;
        }      

        if (dashTimer >= maxDashLength && isDashing == true)
        {
            isDashing = false;
            anim.SetBool("DashingRight", false);
            anim.SetBool("DashingLeft", false);
        }
        else
        {
            if (isDashing == false && dashTimer < maxDashLength)
            {
                isDashing = true;
                if (movingRight)
                    anim.SetBool("DashingRight", true);
                else if (!movingRight)
                    anim.SetBool("DashingLeft", true);
            }
        }

        if (dashTimer >= maxDashLength && !dashComplete)
            EndDash();

        if (!isRagdoll)
        {
            anim.SetBool("Ragdoll", false);
            if (h != 0 && !isDashing)
            {
                anim.SetBool("isWalking", true);
                if (h > 0)
                {
                    h = 1;
                    movingRight = true;
                }
                else if (h < 0)
                {
                    h = -1;
                    movingRight = false;
                }
                rb.velocity = new Vector2(h * speed, rb.velocity.y);
            }
            else
            {
                if (!isDashing)
                    rb.velocity = new Vector2(0, rb.velocity.y);

                anim.SetBool("isWalking", false);
            }

            if (Input.GetButton("Jump") && !isDashing)
            {
                if (jumpReady)
                {
                    Jump();
                    jumpReady = false;
                }

                doubleJumpReady = false;

                if (!doubleJumpUsed)
                    rb.gravityScale = 0;

                if (lessGravityTime >= maxLessGravityTime)
                {
                    rb.gravityScale = 1;
                }
            }
            if (isDashing)
                rb.gravityScale = 0;

            if (anim.GetBool("Attacking") == false)
            {
                if (Input.GetAxis("RightTrigger") > 0.1 && currentDashCooldown < 0f && groundedSinceLastDash)
                {
                    Dash();
                    currentDashCooldown = maxDashCooldown;
                }
            }

            if (atLever && Input.GetButtonDown("Action"))
            {
                lever.isActivated = true;
            }
        }
        else
            anim.SetBool("Ragdoll", true);

        if (Input.GetButtonUp("Jump"))
        {
            jumpReady = true;
            doubleJumpReady = true;
            rb.gravityScale = 1;

            if (rb.velocity.y > 0)
                rb.velocity = new Vector2(rb.velocity.x, 0f);
        }
    }

    void Jump()
    {

        if (isJumping && hasDoubleJump && !doubleJumpUsed && doubleJumpReady && !isDashing)
        {
            //anim.SetBool("doubleJump", true);
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed * 1.5f);
            rb.gravityScale = 1;
            doubleJumpUsed = true;
        }

        if (isGrounded)
        {
            lessGravityTime = 0f;
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
        }
    }

    void Dash()
    {
        dashTimer = 0;
        dashComplete = false;
        groundedSinceLastDash = false;

        if (movingRight == true)
            rb.AddForce(transform.right * dashSpeed, ForceMode2D.Impulse);
        else if (movingRight == false)
            rb.AddForce(-transform.right * dashSpeed, ForceMode2D.Impulse);

        rb.velocity = new Vector2(rb.velocity.x, 0f);
    }

    void EndDash()
    {
        rb.velocity = new Vector2(0, rb.velocity.y);
        dashComplete = true;
        rb.gravityScale = 1;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
            DieAndRespawn();
    }

    void DieAndRespawn()
    {
        isRagdoll = false;
        rb.velocity = Vector2.zero;
        startUp.gameObject.tag = "Startup";
        StartCoroutine(GameManager.Instance.ReloadScene());       
        transform.position = GameObject.Find("PlayerStartPosition").transform.position;              
        currentHealth = maxHealth;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == "Lever")
        {
            atLever = true;
            lever = other.GetComponent<Lever>();
        }
        else
            atLever = false;
        
    }
}
