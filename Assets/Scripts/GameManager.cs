using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [HideInInspector] public CameraEffects camEffects;
    [HideInInspector] public AudioSource audio;

    private Parallaxing parallaxing;

    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
                instance = GameObject.FindObjectOfType<GameManager>();
            return instance;
        }
    }

    void Awake()
    {
        camEffects = GetComponent<CameraEffects>();
        parallaxing = GetComponent<Parallaxing>();
        audio = GetComponent<AudioSource>();
    }

    public IEnumerator ReloadScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Main", LoadSceneMode.Single);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        
        parallaxing.Reset();
    }
}
