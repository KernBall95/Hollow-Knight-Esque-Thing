using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverDoor : MonoBehaviour
{
    public Lever lever;

    private bool doorOpen = false;

    void Update()
    {
        if (lever.isActivated && doorOpen == false)
            OpenDoor();
    }

    void OpenDoor()
    {
        transform.Translate(transform.up * 2.5f);
        doorOpen = true;
    }
}
