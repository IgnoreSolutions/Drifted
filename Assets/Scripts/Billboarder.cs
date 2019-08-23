using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboarder : MonoBehaviour
{
    public Camera mainCamera;
    [SerializeField]
    float constant = 3.0f;
    // Update is called once per frame
    private void Awake()
    {
        mainCamera = Camera.main;
    }
    void Update()
    {
        if (mainCamera != null)
        {
            transform.LookAt(
                transform.position + mainCamera.transform.parent.rotation * Vector3.back * constant, 
                mainCamera.transform.parent.rotation * Vector3.up * constant
            );
        }
    }
}
