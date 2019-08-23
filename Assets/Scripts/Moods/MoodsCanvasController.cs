// /**
// MoodsController.cs
// Created 3/24/2019 7:39 AM
//
// Copyright (C) 2019 Mike Santiago - All Rights Reserved
// axiom@ignoresolutions.xyz
//
// Permission to use, copy, modify, and/or distribute this software for any
// purpose with or without fee is hereby granted, provided that the above
// copyright notice and this permission notice appear in all copies.
//
// THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES
// WITH REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES OF
// MERCHANTABILITY AND FITNESS. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR
// ANY SPECIAL, DIRECT, INDIRECT, OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES
// WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN
// ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF
// OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.
//
// */

using Drifted;
using Drifted.Input;
using UnityEngine;

/// <summary>
/// MonoBehaviour that attaches to the UICanvas and manages showing/hiding the moods display.
/// </summary>
public class MoodsCanvasController : MonoBehaviour
{
    public GameObject Target;

    private EzTimer keyDelayTimer = new EzTimer(0.22f, null, false);
    public bool Display = false, AllowedToToggle = true;

    // Use this for initialization
    void Start()
    {
        if (Target != null) Target.SetActive(false);
        keyDelayTimer.ChangeAction(() => AllowedToToggle = true);
    }

    // Update is called once per frame
    void Update()
    {
        if (keyDelayTimer.enabled) keyDelayTimer.Tick(Time.deltaTime);
        if (Target == null) return;

        if(DriftedInputManager.KeyDown(CustomInputManager.GetCurrentMapping().ToggleMoods))
        {
//            if (CustomInputManager.IsController && DriftedConstants.Instance.UIFocused) return;
            AllowedToToggle = false;
            Display = !Display;
            keyDelayTimer.Start();
            Target.SetActive(Display);
        }
    }
}
