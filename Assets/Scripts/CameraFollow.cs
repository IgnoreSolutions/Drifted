/*
    Copyright (C) 2019 Mike Santiago - All Rights Reserved
    axiom@ignoresolutions.xyz

    Permission to use, copy, modify, and/or distribute this software for any
    purpose with or without fee is hereby granted, provided that the above
    copyright notice and this permission notice appear in all copies.

    THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES
    WITH REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES OF
    MERCHANTABILITY AND FITNESS. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR
    ANY SPECIAL, DIRECT, INDIRECT, OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES
    WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN
    ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF
    OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.
*/

using UnityEngine;
using System;
using UnityEngine.UI;
using Drifted;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using Drifted.UI.WindowManager;
using System.Collections;
using Drifted.Input;

enum UnityMouseButtons
{
    Left = 0,
    Right = 1,
    Middle = 2
}

// Camera Transform notes
// Y affects height of camera/distance from player
// X and Z are your movement axis.
//  X = l/r, Z = u/d. General rule of thumb not to be 110% obeyed.

[DisallowMultipleComponent]
public class CameraFollow : MonoBehaviour
{
    [Header("New Stuff")]
    public DriftedSceneManager SceneVariables;
    #region Public variables
    public Transform FollowTarget, CameraAnchor;
    public Vector3 MinDistance;
    public Vector3 MaxDistance;
    public Vector3 CameraOffset;
    public float CameraSmooth = 0.4f;

    public float CameraSpeed = 3.1f;
    public float ScrollSpeed = 60.0f;
    public float mouseX, mouseY;

    public float CameraDefaultHeight = 20f;

    public Vector2 CameraHeightBounds = new Vector2(-46.0f, 25f);
    public Vector2 CameraFOVBounds = new Vector2(25f, 100f);
    #endregion

    private float OriginFieldOfView = 60.0f;
    private bool LerpTimerEnabled = false;
    private float ResetLerpTimer = -1.0f;
    private const float LerpMaxTime = 1.5f;
    private bool AllowReset = false;
    private Vector3 camVelocity = Vector3.zero;

    private Camera gameCamera;

    private void Start()
    {
        gameCamera = GetComponent<Camera>();

        var newCamPos = transform.position;
        newCamPos.y = CameraDefaultHeight; // Maintain default camera Y value so we're not zoomed all the way in for no reason.
        transform.position = newCamPos;
        transform.LookAt(CameraAnchor);

        OriginFieldOfView = gameCamera.fieldOfView;

        if (FollowTarget != null) CameraAnchor.transform.position = FollowTarget.position;

    }

    private void Awake()
    {
    }

    public void SetCameraFollow(GameObject newFollowTarget)
    {
        FollowTarget = newFollowTarget.transform;
    }

    private void CenterCameraOnPlayer()
    {
        var newPos = FollowTarget.position;
        newPos.y = CameraAnchor.position.y; // Make sure distance doesn't change.
        newPos.x += CameraOffset.x; // Add offset to ensure camera is centered.
        newPos.z += CameraOffset.z;
        CameraAnchor.position = newPos;
    }

    private void RecalculateOffset()
    {
        CameraOffset = CameraAnchor.position - FollowTarget.transform.position;
    }

    /// <summary>
    /// Maintains a minimum distance from the player.
    /// </summary>
    private void CalculateDistance()
    {
        var distance = (CameraAnchor.position - FollowTarget.transform.position);
        if (distance.y < MinDistance.y)
        {
            var newPos = CameraAnchor.position;
            float diff = MinDistance.y - distance.y;
            float newY = CameraAnchor.position.y + diff;
            newPos.y = newY;
            CameraAnchor.position = newPos;
        }
        else if (distance.y > MaxDistance.y)
        {
            var newPos = CameraAnchor.position;
            float diff = distance.y - MaxDistance.y;
            newPos.y = (CameraAnchor.position.y - diff);
            CameraAnchor.position = newPos;
        }
    }

    private static bool IsPointerOverUIElement()
    {
        var eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        return results.Count > 0;
    }

    private bool MouseHoldingItem()
    {
        //return DriftedConstants.Instance.UI().CursorHandler.GetHeldItem() != null;
        return false;
    }

    private void UpdateHandleMovement()
    {
        //if (DriftedConstants.Instance.FullScreenUIActive) return;

        {
            float mx = 0.0f, my = 0.0f;

            if(DriftedInputManager.IsController && DriftedInputManager.LastInputType() != Rewired.ControllerType.Mouse)
            {
                mx = DriftedInputManager.GetAxis("CameraHorizontal") * CameraSpeed * Time.deltaTime;
                my = DriftedInputManager.GetAxis("CameraVertical") * CameraSpeed * Time.deltaTime;
            }
            else 
            {
                if (Input.GetMouseButton((int)UnityMouseButtons.Right) && !IsPointerOverUIElement() && !MouseHoldingItem()) // Only handle camera movement by mouse if the right button is held down.
                {
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                    mx = DriftedInputManager.GetAxis("CameraHorizontal") * CameraSpeed * Time.deltaTime;
                    my = DriftedInputManager.GetAxis("CameraVertical") * CameraSpeed * Time.deltaTime;
                }
                else
                {
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                }
            }

            mouseX += mx;
            mouseY -= my;
            mouseY = Mathf.Clamp(mouseY, CameraHeightBounds.x, CameraHeightBounds.y);
            //transform.LookAt(CameraAnchor);
            CameraAnchor.rotation = Quaternion.Euler(mouseY, mouseX, 0); // Rotate camera
            AllowReset = true;
        }

        if (DriftedInputManager.GetKey("PanCameraLeft"))
        {
            mouseX -= CameraSpeed * Time.deltaTime;

            transform.LookAt(CameraAnchor);
            CameraAnchor.rotation = Quaternion.Euler(mouseY, mouseX, 0); // Rotate camera
            AllowReset = true;
        }
        else if (DriftedInputManager.GetKey("PanCameraRight"))
        {
            mouseX += CameraSpeed * Time.deltaTime;

            transform.LookAt(CameraAnchor);
            CameraAnchor.rotation = Quaternion.Euler(mouseY, mouseX, 0); // Rotate camera
            AllowReset = true;
        }
        else if (DriftedInputManager.KeyDown("ResetCamera") && AllowReset)
        {
            //Debug.Log($"Resetting Camera Rotation to {StartTransform.rotation.x},{StartTransform.rotation.y},{StartTransform.rotation.z}");
            FollowTarget = GameObject.FindWithTag("Player").transform;
            ResetLerpTimer = 0.0f; // Initiate the timer to do a nice smooth reset transition.
            LerpTimerEnabled = true;
            AllowReset = false;
        }

        if (LerpTimerEnabled) // if timer is active
        {
            mouseX = Mathf.Lerp(mouseX, 0, ResetLerpTimer);
            mouseY = Mathf.Lerp(mouseY, 0, ResetLerpTimer);
            gameCamera.fieldOfView = Mathf.Lerp(gameCamera.fieldOfView, OriginFieldOfView, ResetLerpTimer);

            transform.LookAt(CameraAnchor);
            CameraAnchor.rotation = Quaternion.Euler(mouseY, mouseX, 0);
        }
    }

    private Vector3 TargetScale;

    private void UpdateHandleZoom()
    {
        //if (DriftedConstants.Instance.FullScreenUIActive) return;

        // TODO: generically handle zoom in/out
        var wheel = DriftedInputManager.GetAxis("Zoom");
        var newPos = transform.position;

        if (gameCamera != null)
        {
            if(wheel != 0.0f)
            {
                float zoomAmount = wheel * ScrollSpeed;
                Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView + zoomAmount, 20, 100);
                /*
                float radius = wheel * ScrollSpeed; // Radius from camera
                float posX = Camera.main.transform.eulerAngles.x + 90; // Up and down
                float posY = -1 * (Camera.main.transform.eulerAngles.y - 90); // Left and right

                posX = posX / 180 * Mathf.PI; // Degrees to radians
                posY = posY / 180 * Mathf.PI;

                float x, y, z;
                x = radius * Mathf.Sin(posX) * Mathf.Cos(posY); // Calculate new coordinates
                z = radius * Mathf.Sin(posX) * Mathf.Sin(posY);
                y = radius * Mathf.Cos(posX);

                float camX, camY, camZ;
                camX = Camera.main.transform.position.x; // Current cam position
                camY = Camera.main.transform.position.y;
                camZ = Camera.main.transform.position.z;

                float newY, newZ;
                newY = Mathf.Clamp(camY + y, cameraYMin, cameraYMax);
                newZ = Mathf.Clamp(camZ + z, cameraZMin, cameraZMax);
                Debug.Log($"({y}), ({z})");

                Camera.main.transform.position = new Vector3(camX + x, newY, newZ);
                */



            }
            /*
            float increaseFactor = ScrollSpeed * Time.deltaTime;
            if (wheel > 0f) // in
            {
                
                AllowReset = true;
            }
            else if (wheel < 0f)
            {

                AllowReset = true;
            }
            */
        }
    }

    private IEnumerator SmoothCameraScale(Vector3 targetScale)
    {
        if (targetScale == Vector3.zero) yield return null;

        for(float f = 0; f <= 1.0f; f += (.01f * Time.deltaTime))
        {
            transform.position = Vector3.SmoothDamp(CameraAnchor.localScale, targetScale, ref camVelocity, f);
            yield return null;
        }

        yield return null;
    }

    private float transitionLength;
    //public void SetFollowTarget(Transform newTarget)
    //{
        //FollowTarget = newTarget;
    //}

    public void SetFollowTarget(GameObject gameObject)
    {
        if (gameObject == null) return;
        if (FollowTarget != gameObject.transform) FollowTarget = gameObject.transform;
    }

    public void ResetFollowTarget()
    {
        var playerTransform = GameObject.FindWithTag("Player").transform;
        if (FollowTarget != playerTransform) FollowTarget = playerTransform;
    }

    [Header("No really this time it's not a lie")]
    [Range(-10f, 100f)]
    [SerializeField]
    private float CameraDistance = 2;

    public void Update()
    {
        if (FollowTarget == null) ResetFollowTarget();

        //Vector3 targetPosition = FollowTarget.transform.TransformPoint(new Vector3(0, 5, -10));
        //Vector3 followTarget = new Vector3(FollowTarget.transform.position.x, FollowTarget.transform.position.y, CameraDistance);


        //var toCam = -Vector3.forward * CameraDistance;
        //transform.position = CameraAnchor.position + toCam;

        CameraAnchor.transform.position = Vector3.SmoothDamp(CameraAnchor.transform.position, FollowTarget.transform.position, ref camVelocity, CameraSmooth);


        //transform.position = Vector3.SmoothDamp(transform.position, distanceAdjust, ref camVelocity, CameraSmooth);



        UpdateHandleMovement();
        UpdateHandleZoom();

        // Reset Timer logic, if applicable.
        if (LerpTimerEnabled)
        {
            ResetLerpTimer += Time.deltaTime;
            if (ResetLerpTimer >= LerpMaxTime) LerpTimerEnabled = false; // Reset the timer to disabled state.
        }
    }

}