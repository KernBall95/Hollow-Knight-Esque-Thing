using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public int coinValue;
    public float forceY;
    public float minX,maxX;

    private Rigidbody2D rb;
    private float forceX;

    void Start()
    {
        forceX = Random.Range(minX, maxX);
        rb = GetComponent<Rigidbody2D>();

        rb.AddForce(new Vector2(forceX, forceY), ForceMode2D.Impulse);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player")
        {
            PlayerBase.Instance.currentCash += coinValue;
            Destroy(gameObject);
        }
    }
}
