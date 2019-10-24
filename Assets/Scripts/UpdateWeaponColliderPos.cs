using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateWeaponColliderPos : MonoBehaviour
{
    [HideInInspector] public bool downwardAttack;

    private Vector2 rightPos = new Vector2(5f, 5.4f);
    private Vector2 upPos = new Vector2(0f, 13.2f);
    private Vector2 downPos = new Vector2(0f, -6.5f);

    void Start()
    {
        transform.localPosition = rightPos;
        downwardAttack = false;
    }

    void Update()
    {
        if (PlayerBase.Instance.v < -0.7f && PlayerBase.Instance.h == 0f)
        {
            transform.localPosition = upPos;
            transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
            downwardAttack = false;
        }
        else if (PlayerBase.Instance.v > 0.7f && PlayerBase.Instance.isGrounded == false)
        {
            transform.localPosition = downPos;
            transform.localRotation = Quaternion.Euler(0f, 0f, -90f);
            downwardAttack = true;
        }
        else
        {
            transform.localPosition = rightPos;
            transform.rotation = Quaternion.identity;
            downwardAttack = false;
        }     
        
        
        
        
    }
}

