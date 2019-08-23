/**

ActivityConsole.cs

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
using Drifted.UI.WindowManager;
using UnityEngine;
using UnityEngine.UI;

namespace Drifted.UI
{
    [DisallowMultipleComponent]
    public class ActivityConsole : MonoBehaviour
    {
        [HideInInspector]
        public GameObject ActivityConsoleWindow;

        public MikeWindowManager WM;

        private Color ConsoleBGColor = new Color(.45f, .45f, .45f, .90f);
        private Color ConsoleBGColorHidden = new Color(.45f, .45f, .45f, .00f);
        private Color TextColor = Color.white;
        private Color TextColorHidden = new Color(1f, 1f, 1f, .00f);

        private EzTimer DisappearTimer = new EzTimer(3.4f, () => { }, false);
        private EzTimer LerpTimer = null;

        private Text ActivityLogTextBox;

        public string LastLineAdded { get; private set; }

        private string FullLogText = "";

        [SerializeField]
        private float longestCharHeight = 50f;

        private void Awake()
        {
            if (WM == null) GetComponent<MikeWindowManager>();


            /*
            if (ActivityConsoleWindow == null) ActivityConsoleWindow = BuildActivityConsole();
            else
            {
                ActivityLogTextBox = GetComponent<Text>();
            }
            */

            //DisappearTimer.ChangeAction(FadeOutConsole);
            //LerpTimer = new EzTimer(3.0f, null, true);

            //AddLine("Welcome to Drifted!");
        }

        private bool CheckTextWidth()
        {
            var parent = ActivityConsoleWindow.transform.parent.gameObject;
            var text = ActivityConsoleWindow.GetComponent<Text>();

            float textHeight = LayoutUtility.GetPreferredHeight(text.rectTransform);
            float parentHeight = parent.GetComponent<RectTransform>().rect.height;

            return (textHeight > (parentHeight - longestCharHeight));
        }

        private GameObject BuildActivityConsole()
        {
            GameObject activityConsoleWindow = WM.CreateWindow(new Vector2(300, 200), "", ConsoleBGColor, TextColor,
                                                                destroyOnClose: false, needsCloseButton: false, moveable: false, hasShadow: false); //these named arguments are redundant but i like having them here.
            activityConsoleWindow.name = "Activity Console";

            GameObject activityLogText = new GameObject("Activity Log Text");
            Text textComponent = activityLogText.AddComponent<Text>();
            textComponent.font = Resources.Load<Font>("Fonts/game");
            textComponent.text = "Welcome to Drifted!\n";
            textComponent.alignment = TextAnchor.LowerLeft;
            textComponent.verticalOverflow = VerticalWrapMode.Truncate; // Looks like shit but i don't care.
            activityLogText.AddComponent<LayoutElement>();

            activityLogText.transform.SetParent(activityConsoleWindow.transform);

            ActivityLogTextBox = textComponent;

            RectTransform rectTransform = activityConsoleWindow.GetComponent<RectTransform>();
            /*
            rectTransform.anchorMin = new Vector2(0, 1);
            rectTransform.anchorMax = new Vector2(0, 1); // Top left
            */

            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(0, 0); // Bottom left
                       
            rectTransform.pivot = rectTransform.anchorMax;

            return activityConsoleWindow;
        }

        public void AddLine(string text)
        {
            //FullLogText = $"{text}\n{FullLogText}";
            FullLogText += $"{text}\n";
            LastLineAdded = text;
            //ActivityLogTextBox.text += text;
            //if (!ActivityLogTextBox.text.EndsWith("\n")) ActivityLogTextBox.text += "\n";
            if(ActivityLogTextBox != null) ShowConsole();
        }

        public void ShowConsole()
        {
            LerpTimer.Stop();

            Image bg = ActivityConsoleWindow.GetComponent<Image>();
            bg.color = ConsoleBGColor;

            ActivityLogTextBox.gameObject.SetActive(true);
            ActivityLogTextBox.color = TextColor;

            DisappearTimer.Start();
        }

        public void FadeOutConsole()
        {
            /*
            var img = ActivityConsoleWindow.GetComponent<Image>();
            img.color = Color.Lerp(ConsoleBGColor, ConsoleBGColorHidden, DisappearTimer.Time);
            */

            LerpTimer.ResetTime();
            LerpTimer.Start();

            ActivityLogTextBox.gameObject.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            /*
            if(ActivityLogTextBox != null) ActivityLogTextBox.text = $"{FullLogText}";
            if (DisappearTimer.enabled) DisappearTimer.Tick(Time.deltaTime);
            if (LerpTimer != null && LerpTimer.enabled)
            {
                LerpTimer.Tick(Time.deltaTime);
                Image bg = ActivityConsoleWindow.GetComponent<Image>();
                bg.color = Color.Lerp(ConsoleBGColorHidden, ConsoleBGColor, LerpTimer.Time);
                //ActivityLogTextBox.color = Color.Lerp(TextColorHidden, TextColor, LerpTimer.Time);
            }
            */
        }

    }
}