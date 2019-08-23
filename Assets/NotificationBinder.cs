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
using UnityEngine;
using UnityEngine.UI;

namespace Drifted.UI.Notifications
{
    public class NotificationBinder : MonoBehaviour
    {
        [SerializeField]
        Text text;

        [SerializeField]
        Image sprite;

        [SerializeField]
        Animation anim;

        private Action UpdateAction;

        public void SetNotification(DriftedNotification notification)
        {
            if (notification != null)
            {
                text.text = notification.Text;
                if (notification.icon != null) sprite.sprite = notification.icon;
            }
        }

        private void Awake()
        {
            if (anim == null)
            {
                anim = GetComponent<Animation>();
            }

            anim.enabled = true;
            anim.Play();
        }

        private void Start()
        {
        }

        private void DestroyMe()
        {
            Debug.Log("Bye bye!");
            Destroy(this.gameObject);
        }

        public void DestroyMeIn(float timeSeconds)
        {
            Invoke("DestroyMe", timeSeconds);
        }

        private void Update()
        {
            if (!anim.isPlaying) DestroyMeIn(3.0f);
        }
    }
}
