using UnityEngine;
using System.Collections;
using Drifted.UI.WindowManager;
using System;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class WMStayAtMouse : MonoBehaviour
{
    public Vector3 OffsetFromMouse = Vector3.zero;

    public Vector3 CurrentMousePosition;

    public GameObject FollowTarget = null;

    [ReadOnly]
    public MikeWindowManager WM;
    private Camera mainCam;

    void Awake()
    {
        mainCam = Camera.main;
    }

    void Start()
    {
        if (FollowTarget != null) transform.position = FollowTarget.transform.position + OffsetFromMouse;
        
    }

    void Update()
    {
        if(FollowTarget != null)
        {
            transform.position = FollowTarget.transform.position + OffsetFromMouse;
        }
        else
        {
            transform.position = Input.mousePosition + OffsetFromMouse;
        }
    }
}
