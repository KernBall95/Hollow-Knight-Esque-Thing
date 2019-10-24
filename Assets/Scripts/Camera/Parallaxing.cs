using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallaxing : MonoBehaviour {
    
    public float smoothing = 1f;

    //[SerializeField] private Transform[] sceneryArray;
    [SerializeField] private List<Transform> sceneryList = new List<Transform>();
    private float[] parallaxScales;
    private Transform cam;
    private Vector3 previousCamPos;  
    private Transform sky;
    private GameObject sceneryParent;

    void Awake()
    {
        cam = Camera.main.transform;
        sceneryParent = GameObject.Find("MovingScenery");
        sky = GameObject.Find("Sky").transform;
    }

	void Start () {
        foreach (Transform child in sceneryParent.transform)
        {
            sceneryList.Add(child);
        }

        previousCamPos = cam.position;
        parallaxScales = new float[sceneryList.Count];

        for(int i = 0; i < sceneryList.Count - 1; i++)
        {
            parallaxScales[i] = sceneryList[i].position.z * -1;
        }      
	}
	
	void Update () {
        if (cam != null)
        {
            if (cam.position != previousCamPos)
                UpdateParallax();
        }
        else
            Debug.LogError("Parallax script is missing its camera!");
            
    }

    void UpdateParallax()
    {
        for (int i = 0; i < sceneryList.Count - 1; i++)
        {
            float parallax = (previousCamPos.x - cam.position.x) * parallaxScales[i];
            float sceneryTargetPosX = sceneryList[i].position.x + parallax;
            Vector3 sceneryTargetPos = new Vector3(sceneryTargetPosX, sceneryList[i].position.y, sceneryList[i].position.z);

            sceneryList[i].position = Vector3.Lerp(sceneryList[i].position, sceneryTargetPos, smoothing * Time.fixedDeltaTime);
        }
        previousCamPos = cam.position;
        sky.position = new Vector3(cam.position.x, sky.position.y, sky.position.z);
    }

    public void Reset()
    {
        cam = Camera.main.transform;
        sceneryParent = GameObject.Find("MovingScenery");
        sky = GameObject.Find("Sky").transform;

        sceneryList.Clear();

        foreach (Transform child in sceneryParent.transform)
        {
            sceneryList.Add(child);
        }       
    }
}
