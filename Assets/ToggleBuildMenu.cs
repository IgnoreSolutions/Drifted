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

using System.Collections;
using System.Collections.Generic;
using Drifted;
using UnityEngine;
using Drifted.Input;

public class ToggleBuildMenu : MonoBehaviour
{
    [SerializeField]
    Transform target;

    [SerializeField]
    Transform buildUI;

    [SerializeField]
    bool Display = true;

    bool allowedToToggle = true;

    [SerializeField]
    GameEvent ToggleBuildEvent;

    private PlayerMovement player;
    private CameraFollow mainCam;

    [SerializeField]
    BoolReference UIActive;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();

        mainCam = Camera.main.GetComponent<CameraFollow>();
    }

    private void OnValidate()
    {
        if (target != null) target.gameObject.SetActive(Display);
        if (buildUI != null) buildUI.gameObject.SetActive(Display);
    }

    void BeginBuildMode()
    {
        if(UIActive != null) UIActive.Value = true;
        target.gameObject.SetActive(Display);
        buildUI.gameObject.SetActive(Display);

        //player.gameObject.SetActive(false);
        player.enabled = false;
        mainCam.FollowTarget = target.transform;

        if(ToggleBuildEvent != null) ToggleBuildEvent.Raise();
    }

    void EndBuildMode()
    {
        if(UIActive != null) UIActive.Value = false;

        target.gameObject.SetActive(Display);
        buildUI.gameObject.SetActive(Display);

        // TODO: find a better way to do this; disabling breaks FindObjectWithTag
        //player.gameObject.SetActive(true);
        player.enabled = true;
        if(ToggleBuildEvent != null) ToggleBuildEvent.Raise();
    }

    void ResetAllowed() => allowedToToggle = true;

    private void LateUpdate()
    {
        if (!allowedToToggle) return;
        if(DriftedInputManager.GetKey("ToggleBuild"))
        {
            allowedToToggle = false;

            Display = !Display;

            if (Display) BeginBuildMode();
            else EndBuildMode();

            Invoke("ResetAllowed", 3.0f);
        }
    }
}
