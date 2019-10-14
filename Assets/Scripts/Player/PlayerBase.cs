using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBase : MonoBehaviour {

    public float speed;
    public float jumpSpeed;
    public float dashSpeed;
    public float maxDashLength = 0.4f;
    public float maxDashCooldown = 0.75f;
    public bool isGrounded;
    //public Collider2D weaponCollider;
    // public GameObject attackEffect;

    Rigidbody2D rb;
    Animator anim;
    float lessGravityTime;
    float maxLessGravityTime = 0.4f;
    float h;
    public float currentDashCooldown = 0f;
    float dashTimer;
    bool isJumping;
    bool hasDoubleJump = true;
    bool doubleJumpUsed;
    bool doubleJumpReady;
    public bool jumpReady;
    [HideInInspector]public bool isDashing = false;
    bool dashComplete;
    public bool groundedSinceLastDash;
    public bool isRagdoll;
    bool movingRight = true;
    Vector3 normalScale = new Vector3(0.1f, 0.1f, 1f);
    Vector3 flippedScaleX = new Vector3(-0.1f, 0.1f, 1f);
    RaycastHit2D hit;

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

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        //hasDoubleJump = true;
        //isDashing = false;
        jumpReady = true;      
        dashTimer = maxDashLength;
    }

    void Update()
    {
        isRagdoll = GetComponent<PlayerTakeDamage>().isRagdoll;
        h = Input.GetAxis("Horizontal");

        currentDashCooldown -= Time.deltaTime;
        dashTimer += Time.deltaTime;
        lessGravityTime += Time.deltaTime;

        hit = Physics2D.Raycast(transform.position, -transform.up, .9f);
        Debug.DrawLine(transform.position, transform.position - (transform.up * .4f), Color.red);
        if (hit.collider != null)
        {
            if (hit.collider.tag == "Floor")
                isGrounded = true;
        }
        else
            isGrounded = false;
             
        if (dashTimer >= maxDashLength)
        {
            isDashing = false;
            anim.SetBool("DashingRight", false);
            anim.SetBool("DashingLeft", false);
        }           
        else
        {
            isDashing = true;
            if (movingRight)
                anim.SetBool("DashingRight", true);
            else if (!movingRight)
                anim.SetBool("DashingLeft", true);
        }

        if (dashTimer >= maxDashLength && !dashComplete)
            EndDash();

        if (isGrounded)
        {
            groundedSinceLastDash = true;
            anim.SetBool("Ground", true);
        }
        else if (!isGrounded)
            anim.SetBool("Ground", false);
                 
        if (movingRight)
        {
            transform.localScale = normalScale;
        }           
        else if (!movingRight)
        {          
            transform.localScale = flippedScaleX;
        }


        if (!isRagdoll)
        {
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

            if (Input.GetButtonUp("Jump"))
            {
                jumpReady = true;
                doubleJumpReady = true;
                rb.gravityScale = 1;

                if (rb.velocity.y > 0)
                    rb.velocity = new Vector2(rb.velocity.x, 0f);
            }

            if (isDashing)
                rb.gravityScale = 0;

            if (Input.GetAxis("RightTrigger") > 0.1 && currentDashCooldown < 0f && groundedSinceLastDash)
            {
                Dash();
                currentDashCooldown = maxDashCooldown;
            }
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

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Floor")
        {
            isJumping = false;
            doubleJumpUsed = false;
            //anim.SetBool("doubleJump", false);
        }
    }

    void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.tag == "Floor")
            isJumping = true;
                   
    }
}
