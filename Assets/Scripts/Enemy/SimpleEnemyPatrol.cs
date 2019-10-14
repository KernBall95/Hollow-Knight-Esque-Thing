using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleEnemyPatrol : MonoBehaviour {

    public float moveSpeed;
    public bool movingRight = true;

    Rigidbody2D rb;
    SpriteRenderer sr;
    float distanceToTarget;
    float directionX;
    Vector3 direction;
    bool followingPlayer;
    EnemyManager EM;
    public bool flipReady;
    RaycastHit2D hit;

    void Start () {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        EM = GetComponent<EnemyManager>();
        flipReady = true;
	}
	
	void FixedUpdate () {
        distanceToTarget = Vector2.Distance(PlayerBase.Instance.transform.position, transform.position);

        if (!EM.isRagdoll){
            if (distanceToTarget < 4f)
            {
                followingPlayer = true;
                MoveToPlayer();
            }
            else
                followingPlayer = false;

            if (!followingPlayer)
            {
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
            }
                     
            if (hit.collider != null && flipReady)
            {
                if (hit.collider.tag == "Wall" || hit.collider.tag == "Floor")
                {
                    StartCoroutine(FlipDirection());
                }
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
  
        rb.AddForce(direction.normalized * moveSpeed);
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
}
