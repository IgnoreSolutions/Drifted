// /**
// NewClass
// Created 3/27/2019 4:43 PM
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
using System;
using System.IO;
using UnityEngine;

namespace Drifted.CustomInput
{

    public interface IButton
    {
        KeyCode KeyCode { get; set; }
        string AxisName { get; set; }
        KeyType InputStyle { get; set; }
        bool InvertAxis { get; set; }
    }

    [Serializable]
    public abstract class AbstractInput : IButton
    {
        public KeyType InputStyle { get; set; } = KeyType.Button;
        public KeyCode KeyCode { get; set; }
        public string AxisName { get; set; }
        public bool InvertAxis { get; set; } = false;
        internal float Direction { get; set; } = 0.0f;

        public virtual dynamic GetValue()
        {
            switch(InputStyle)
            {
                case KeyType.Axis: return UnityEngine.Input.GetAxis(AxisName);
                case KeyType.Button: return UnityEngine.Input.GetKey(KeyCode);
                default: return 0;
            }
        }
    }

    [Serializable]
    public class ButtonOrAxis : AbstractInput
    {
        public ButtonOrAxis(KeyCode kc)
        {
            InputStyle = KeyType.Button;
            KeyCode = kc;
            //value = float.MinValue;
        }

        public ButtonOrAxis(string AxisName, bool invertAxis = false, bool actAsButton = false, float direction = 0.0f)
        {
            if (actAsButton)
            {
                InputStyle = KeyType.AxisEmulatingButton;
                Direction = direction;
            }
            else InputStyle = KeyType.Axis;

            base.AxisName = AxisName;
            KeyCode = KeyCode.None;
            InvertAxis = invertAxis;
        }
    }

    public enum KeyType
    {
        Axis,
        Button,
        Other,
        AxisEmulatingButton
    }
}