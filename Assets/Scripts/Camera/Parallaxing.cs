using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallaxing : MonoBehaviour {

    public Transform[] scenery;
    public Transform sky;
    public float smoothing = 1f;

    private float[] parallaxScales;
    private Transform cam;
    private Vector3 previousCamPos;

    void Awake()
    {
        cam = Camera.main.transform;
    }

	void Start () {
        previousCamPos = cam.position;
        parallaxScales = new float[scenery.Length];

        for(int i = 0; i < scenery.Length; i++)
        {
            parallaxScales[i] = scenery[i].position.z * -1;
        }
	}
	
	void Update () {
        if (cam != null)
            UpdateParallax();
        else
            Debug.LogError("Parallax script is missing its camera!");
    }

    void UpdateParallax()
    {
        for (int i = 0; i < scenery.Length; i++)
        {
            float parallax = (previousCamPos.x - cam.position.x) * parallaxScales[i];
            float sceneryTargetPosX = scenery[i].position.x + parallax;
            Vector3 sceneryTargetPos = new Vector3(sceneryTargetPosX, scenery[i].position.y, scenery[i].position.z);

            scenery[i].position = Vector3.Lerp(scenery[i].position, sceneryTargetPos, smoothing * Time.deltaTime);
        }
        previousCamPos = cam.position;
        sky.position = new Vector3(cam.position.x, sky.position.y, sky.position.z);
    }

    public void Reset()
    {
        cam = Camera.main.transform;

        for (int i = 0; i < scenery.Length; i++)
        {
            scenery[i] = GameObject.Find("Background Mountains " + (i + 1)).transform;
            sky = GameObject.Find("Sky").transform;
            Debug.Log(cam.GetInstanceID());
        }       
    }
}
