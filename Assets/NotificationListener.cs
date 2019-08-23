﻿/*
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
using Drifted.UI;
using Drifted.UI.Notifications;
using UnityEngine;

namespace Drifted.UI.Notifications
{
    public class NotificationListener : MonoBehaviour
    {
        [SerializeField]
        NotificationManager notificationManager;

        [SerializeField]
        GameObject NotificationTemplate;

        public void HandleNotification(GameObject target /*where the notification came from*/)
        {
            if (notificationManager == null) return;

            Debug.Log("Handling notification.");
            DriftedNotification topMostNotification = notificationManager.GetNotification();

            var newNotification = Instantiate(NotificationTemplate, transform);

            NotificationBinder binder = newNotification.GetComponent<NotificationBinder>();
            if (binder == null) return;

            binder.SetNotification(topMostNotification);
            //binder.DestroyMeIn(3.0f);
        }
    }
}