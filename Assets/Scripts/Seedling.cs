using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[DisallowMultipleComponent]
public class Seedling : MonoBehaviour
{
    public Transform coconutPrefab;
    public GameObject palmTreeStage1Prefab;
    public GameObject palmTreeStage2Prefab;
    public GameObject palmTreeStage3Prefab;
    public GameObject palmTreeStage4Prefab;
    public GameObject palmTreeStage5Prefab;

    public float growthTime = 60.0f;

    // Start is called before the first frame update
    void Start()
    {
        palmTreeStage1Prefab = Instantiate(palmTreeStage1Prefab, coconutPrefab.position, coconutPrefab.rotation) as GameObject;
        palmTreeStage1Prefab.tag = coconutPrefab.gameObject.tag;
        palmTreeStage1Prefab.layer = coconutPrefab.gameObject.layer;
        palmTreeStage1Prefab.transform.SetParent(coconutPrefab);

        Invoke("FirstCycle", 60.0f);
    }

    void FirstCycle()
    {
        palmTreeStage2Prefab = Instantiate(palmTreeStage2Prefab, coconutPrefab.position, coconutPrefab.rotation) as GameObject;
        palmTreeStage2Prefab.tag = coconutPrefab.gameObject.tag;
        palmTreeStage2Prefab.layer = coconutPrefab.gameObject.layer;
        palmTreeStage2Prefab.transform.SetParent(coconutPrefab);
        Destroy(palmTreeStage1Prefab);

        Invoke("SecondCycle", 60.0f);
    }

    void SecondCycle()
    {
        palmTreeStage3Prefab = Instantiate(palmTreeStage3Prefab, coconutPrefab.position, coconutPrefab.rotation) as GameObject;
        palmTreeStage3Prefab.tag = coconutPrefab.gameObject.tag;
        palmTreeStage3Prefab.layer = coconutPrefab.gameObject.layer;
        palmTreeStage3Prefab.transform.SetParent(coconutPrefab);
        palmTreeStage3Prefab.transform.SetParent(coconutPrefab);

        Destroy(palmTreeStage2Prefab);

        Invoke("ThirdCycle", 60.0f);
    }

    void ThirdCycle()
    {
        palmTreeStage4Prefab = Instantiate(palmTreeStage4Prefab, coconutPrefab.position, coconutPrefab.rotation) as GameObject;
        palmTreeStage4Prefab.tag = coconutPrefab.gameObject.tag;
        palmTreeStage4Prefab.layer = coconutPrefab.gameObject.layer;
        palmTreeStage4Prefab.transform.SetParent(coconutPrefab);

        Destroy(palmTreeStage3Prefab);

        Invoke("FourthCycle", 60.0f);
    }

    void FourthCycle()
    {
        palmTreeStage5Prefab = Instantiate(palmTreeStage5Prefab, coconutPrefab.position, coconutPrefab.rotation) as GameObject;
        palmTreeStage5Prefab.tag = coconutPrefab.gameObject.tag;
        palmTreeStage5Prefab.layer = coconutPrefab.gameObject.layer;
        palmTreeStage5Prefab.transform.SetParent(coconutPrefab);
        Destroy(palmTreeStage4Prefab);
    }
}
