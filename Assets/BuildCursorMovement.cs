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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Drifted;
using UnityEngine;
using Drifted.Input;
using Drifted.Interactivity;

public class BuildCursorMovement : MonoBehaviour
{
    [SerializeField]
    GameObject currentBuildingPrefab;

    [SerializeField]
    GameObject physicalCursor;

    [SerializeField]
    Transform PlacementParent;

    [ReadOnly]
    [SerializeField]
    bool AbleToPlace = false;

    private Camera CameraReference;
    [SerializeField]
    float moveSpeed = 2.2f;
    // Start is called before the first frame update

    private MeshRenderer physicalCursorMR;

    private BoxCollider cursorTriggerCollider;

    void Awake()
    {
        cursorTriggerCollider = GetComponent<BoxCollider>();

        if(physicalCursor != null)
        {
            physicalCursorMR = physicalCursor.GetComponent<MeshRenderer>();
        }
    }

    void Start()
    {
        CameraReference = Camera.main;
        SetMatColor(Color.green);

        if(currentBuildingPrefab != null)
        {
            var cursorPreview = currentBuildingPrefab;

        }
        //var colliderSwap = gameObject.AddComponent<Collider>();
        //colliderSwap = transform.GetChild(0).GetComponent<Collider>().Copy();
    }

    void SetupPreview()
    {
        DestroyChildren();

        var cursorPreview = Instantiate(currentBuildingPrefab, transform.position, transform.rotation);
        cursorPreview.transform.SetParent(transform);

        DriftedSceneInteractable cullingProps = cursorPreview.GetComponent<DriftedSceneInteractable>();
        if(cullingProps != null)
        {
            cullingProps.d_SetCullingAllowed(false);
        } else Debug.Log("Nope, can't do it like this.");
        BoxCollider col = cursorPreview.GetComponent<BoxCollider>();
        if(col != null && cursorTriggerCollider != null) 
        {
            col.enabled = false;

            cursorTriggerCollider.size = col.size;
            cursorTriggerCollider.center = col.center;

            if(physicalCursor != null)
            {
                physicalCursor.transform.localScale =  new Vector3(col.size.x, col.size.y * 2, col.size.z);
            }
        }

        if (AbleToPlace) SetMatColor(Color.green);
        else SetMatColor(Color.red);
        //cursorPreview.transform.position = Vector3.zero;
    }

    [SerializeField]
    Vector3 raycastOffset = new Vector3(0, 3, 0);

    [SerializeField]
    DriftedLayers RaycastLayerMask = (DriftedLayers.DriftedLevel);

    void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, -transform.up * 30f, Color.green);
    }

    void HoldToGround()
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, -transform.up * 30f, out hit))
        {
            float distanceToGround = hit.distance - raycastOffset.y;
            if(hit.transform.gameObject.layer == (int)DriftedLayers.DriftedEnvironment) AbleToPlace = false;

            float x, y, z;
            x = transform.position.x;
            y = hit.point.y - raycastOffset.y;
            z = transform.position.z;

            transform.position = new Vector3(x, y, z);
        }
    }

    void DestroyChildren()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }

    Vector3 moveDir = Vector3.zero;

    void CameraStep()
    {
        // Ensures our player's forward direction is the same as the camera.
        Vector3 controlRight = Vector3.Cross(CameraReference.transform.up, CameraReference.transform.forward);
        Vector3 controlForward = Vector3.Cross(CameraReference.transform.right, Vector3.up);

        //moveDir = Vector3.zero;
        moveDir = Vector3.ClampMagnitude((controlRight * DriftedInputManager.GetAxis("MoveHorizontal")) + (controlForward * DriftedInputManager.GetAxis("MoveForward")), 1.0f);

    }

    void SetMatColor(Color theColor)
    {
        if(physicalCursorMR != null)
        {
            theColor.a = .24f;
            physicalCursorMR.material.color = theColor;
        }
    }

    Renderer thisRenderer;
    void SetColor(Color theColor)
    {
        var renderers = transform.GetChild(0).GetComponentsInChildren<Renderer>();

        thisRenderer = renderers.FirstOrDefault(x => x.gameObject.layer != LayerDefinitions.DriftedMinimap);
        if (thisRenderer == null)
        {
            Debug.LogWarning("Renderer was null on " + gameObject.name + ". No changes made.", gameObject);
            return;
        }

        foreach (var mat in thisRenderer.materials)
        {
            mat.color = theColor;
            //mat.shader = shader;

            /*
            if (mat.shader.name.Contains("Hologram"))
            {
                //Debug.Log("Shader props set");
                mat.SetFloat("_Amplitude", Amplitude);
                mat.SetFloat("_Speed", Speed);
                mat.SetFloat("_Amount", Amount);
            }
            else if (mat.shader.name.Contains("Outline"))
            {
                // Boujee outline
                mat.SetColor("_FirstOutlineColor", Color.yellow);
                mat.SetFloat("_FirstOutlineWidth", 0.15f);

                mat.SetColor("_SecondOutlineColor", Color.yellow);
                mat.SetFloat("_SecondOutlineWidth", 0.15f);
            }
            */
            //mat.SetColor("Tint Color", preservedColor);
        }
    }

    public void OnColliderEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name);
        AbleToPlace = false;
        SetColor(Color.red);
    }

    private void OnColliderExit(Collision collision)
    {
        Debug.Log(collision.gameObject.name);
        AbleToPlace = true;
        SetColor(Color.green);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger enter");
        AbleToPlace = false;
        SetMatColor(Color.red);
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Trigger exit");
        AbleToPlace = true;
        SetMatColor(Color.green);
    }

    public void SetBuildPrefab(GameObject newBuildPrefab)
    {
        currentBuildingPrefab = newBuildPrefab;
        SetupPreview();
    }

    private void FixedUpdate()
    {
        CameraStep();

        if(Input.GetKeyDown(KeyCode.Space) && AbleToPlace)
        {
            var newGo = Instantiate(currentBuildingPrefab, transform.position, transform.rotation);
            if(PlacementParent != null) 
            {
                newGo.transform.SetParent(PlacementParent);
                //newGo.layer = PlacementParent.gameObject.layer;
            }
            AbleToPlace = false;

            SnapToGround snap;
            if((snap = newGo.GetComponent<SnapToGround>()) != null)
            {
                snap.enabled = true;
            }
        }

        //GetComponent<Rigidbody>().MovePosition(transform.position + (moveDir * moveSpeed * Time.fixedDeltaTime));
        HoldToGround();
        transform.position = transform.position + (moveDir * moveSpeed * Time.fixedDeltaTime);
        if(physicalCursor != null) physicalCursor.transform.position = transform.position;
    }
}
