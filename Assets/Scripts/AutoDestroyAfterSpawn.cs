using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroyAfterSpawn : MonoBehaviour
{
    public float timeBeforeDestruct;

    void Start()
    {
        StartCoroutine(Destruct());
    }

    IEnumerator Destruct()
    {
        yield return new WaitForSeconds(timeBeforeDestruct);
        Destroy(gameObject);
    }
}
