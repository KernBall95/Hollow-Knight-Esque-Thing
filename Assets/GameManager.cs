using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
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
        parallaxing = GetComponent<Parallaxing>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator ReloadScene()
    {
        //SceneManager.LoadScene("Main", LoadSceneMode.Single);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Main", LoadSceneMode.Single);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        parallaxing.Reset();
    }
}
