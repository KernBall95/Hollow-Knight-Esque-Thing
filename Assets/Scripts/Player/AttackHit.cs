using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackHit : MonoBehaviour
{
    [HideInInspector] public bool hitEnemy = false;
    [HideInInspector] public EnemyManager eManager;
    [HideInInspector] public Rigidbody2D eRB;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       // hitEnemy = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            hitEnemy = true;
            eManager = other.GetComponent<EnemyManager>();
            eRB = other.GetComponent<Rigidbody2D>();
        }
            
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.tag == "Enemy")
        {
            hitEnemy = false;
        }
    }
}
