// /**
// NewClass
// Created 5/3/2019 5:17 PM
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Drifted.Player
{
    public class CoroutineQueue
    {
        private bool m_Active = false;
        public bool Active { get => m_Active; }

        MonoBehaviour m_Owner = null;

        Coroutine m_InternalCoroutine = null;
        Queue<IEnumerator> actions = new Queue<IEnumerator>();

        public int ActionCount() => actions.Count;

        public CoroutineQueue(MonoBehaviour owner)
        {
            m_Owner = owner;
        }

        public void StartLoop()
        {
            m_InternalCoroutine = m_Owner.StartCoroutine(Process());
        }

        public void StopLoop()
        {
            m_Owner.StopCoroutine(m_InternalCoroutine);
            m_InternalCoroutine = null;
        }

        public void EnqueueWait(float waitTime)
        {
            actions.Enqueue(Wait(waitTime));
        }

        public void EnqueueAction(IEnumerator action)
        {
            actions.Enqueue(action);
        }

        public void EnqueueAction(Func<bool> func) => actions.Enqueue(DoFunc(func));
        public void EnqueueAction(Action action) => actions.Enqueue(DoAction(action));

        private IEnumerator DoFunc(Func<bool> func)
        {
            func();
            yield return null;
        }

        private IEnumerator DoAction(Action action)
        {
            action();
            yield return null;
        }

        private IEnumerator Wait(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
        }

        private bool Paused = false;
        public void Pause() => Paused = true;
        public void Unpause() => Paused = false;

        private IEnumerator Process()
        {
            while(true)
            {
                if(Paused)
                {
                    m_Active = false;
                    yield return null;
                }
                else
                {
                    if (actions.Count > 0)
                    {
                        m_Active = true;
                        yield return m_Owner.StartCoroutine(actions.Dequeue());
                    }
                    else
                    {
                        m_Active = false;
                        yield return null;
                    }
                }
                yield return new WaitForSeconds(0.5f);
            }
        }
    }
}
