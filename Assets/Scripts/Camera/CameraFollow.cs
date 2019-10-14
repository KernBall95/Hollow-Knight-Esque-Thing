using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    public Transform target;
    public float distFromPlayer;
    public float yOffset;
    public float speed;
    public Collider2D boundingBox;

    private Vector3 newPos;
    private float minPos;   
    private float maxPos;
    private float halfHeight;
    private float halfWidth;

    void Start()
    {
        transform.position = target.transform.position;

        halfHeight = Camera.main.orthographicSize;
        halfWidth = Camera.main.aspect * halfHeight;

        minPos = boundingBox.bounds.min.x + halfWidth;
        maxPos = boundingBox.bounds.max.x - halfWidth;
        
    }

	void FixedUpdate () {
        newPos = new Vector3(PlayerBase.Instance.transform.position.x, PlayerBase.Instance.transform.position.y + yOffset, distFromPlayer);

        if (newPos.x < minPos)
            newPos = new Vector3(minPos, PlayerBase.Instance.transform.position.y + yOffset, distFromPlayer);
        else if(newPos.x > maxPos)
            newPos = new Vector3(maxPos, PlayerBase.Instance.transform.position.y + yOffset, distFromPlayer);
            

        transform.position = Vector3.Slerp(transform.position, newPos, speed * Time.deltaTime);
        
	}
}
