/**

SnapToGround.cs

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
using System.Collections;

[DisallowMultipleComponent]
public class SnapToGround : MonoBehaviour
{
    private Vector3 OriginalForward;
    private RaycastHit hit;

    bool done = false;

    private void Awake()
    {
        OriginalForward = Quaternion.Euler(transform.localEulerAngles) * Vector3.forward;
        Debug.DrawRay(transform.position, OriginalForward * 10, Color.red);
        Debug.DrawRay(transform.position, -OriginalForward * 10, Color.blue);

        if (Physics.Raycast(transform.position, -OriginalForward, out hit, 10f))
        {
            Debug.DrawLine(hit.point, hit.point + new Vector3(1.0f, 0.0f, 1.0f));
            if (done) return;
            transform.position = hit.point;
            //transform.up = hit.normal;
            transform.rotation = Quaternion.Euler(-(transform.rotation.x + 90 + hit.normal.x), transform.rotation.y, transform.rotation.z);
            //transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.y);
            done = true;
        }

        //StartCoroutine(RaycastRoutine());
    }

    private void OnDrawGizmos()
    {
        if(this.isActiveAndEnabled)
        {
            Debug.DrawRay(transform.position, transform.up * 30, Color.red);
            Debug.DrawRay(transform.position, -transform.up * 30, Color.blue);
        }
    }

    private IEnumerator RaycastRoutine()
    {
        while (true)
        {
            if (Physics.Raycast(transform.position, -transform.up, out hit, 20f))
            {
                transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
            }
            yield return new WaitForSeconds(0.25f);
        }
    }

    [SerializeField]
    float RaycastOffset = 10.0f;

    void Update()
    {
        if (Physics.Raycast(transform.position, -transform.up, out hit, 20f))
        {
            transform.position = new Vector3(transform.position.x, hit.point.y + RaycastOffset, transform.position.z);
        }
        /*

        // TODO: Is this change ok?
        if (!done)
        {


            Physics.Raycast(transform.position, -OriginalForward, out hit);
            transform.position = hit.point;
            //transform.up -= (transform.up - hit.normal) * 0.1f;
            transform.rotation = Quaternion.Euler(-(transform.up.x + 90), transform.up.y, transform.up.z);
            done = true;
        }
        */
    } 
}
