using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class SetCameraFollowTarget : MonoBehaviour
{
    private CinemachineVirtualCamera vCam;

    // Start is called before the first frame update
    void Start()
    {
        vCam = GetComponent<CinemachineVirtualCamera>();
        vCam.Follow = PlayerBase.Instance.transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
