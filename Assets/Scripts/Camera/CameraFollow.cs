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
    private float minPosX;   
    private float maxPosX;
    private float minPosY;
    private float maxPosY;
    private float newPosX;
    private float newPosY;
    private float halfHeight;
    private float halfWidth;

    void Start()
    {
        if(target != null)
            transform.position = target.transform.position;

        halfHeight = Camera.main.orthographicSize;
        halfWidth = Camera.main.aspect * halfHeight;

        minPosX = boundingBox.bounds.min.x + halfWidth;
        maxPosX = boundingBox.bounds.max.x - halfWidth;
        minPosY = boundingBox.bounds.min.y + halfHeight;
        maxPosY = boundingBox.bounds.max.y - halfHeight;
    }

	void Update () {
        if(PlayerBase.Instance != null)
        {
            newPos = new Vector3(PlayerBase.Instance.transform.position.x, PlayerBase.Instance.transform.position.y + yOffset, distFromPlayer);
            newPosX = newPos.x;
            newPosY = newPos.y;

            if (newPos.x < minPosX)
                newPosX = minPosX;

            if (newPos.x > maxPosX)
                newPosX = maxPosX;

            if (newPos.y < minPosY)
                newPosY = minPosY;

            if (newPos.y > maxPosY)
                newPosY = maxPosY;

            newPos = new Vector3(newPosX, newPosY, distFromPlayer);

            transform.position = Vector3.Lerp(transform.position, newPos, speed * Time.deltaTime);
        }
    }
}
