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

namespace Drifted
{
    public class SlopeDetect : MonoBehaviour
    {
        RaycastHit hit;//For Detect Sureface/Base.
        Vector3 surfaceNormal;//The normal of the surface the ray hit.
        Vector3 forwardRelativeToSurfaceNormal;//For Look Rotation

        // Update is called once per frame
        void Update()
        {
            CharacterFaceRelativeToSurface();
        }

        //Method For Correct Character Rotation According to Surface.
        private void CharacterFaceRelativeToSurface()
        {
            //For Detect The Base/Surface.
            if (Physics.Raycast(transform.position, -Vector3.up, out hit, 10))
            {
                surfaceNormal = hit.normal; // Assign the normal of the surface to surfaceNormal
                forwardRelativeToSurfaceNormal = Vector3.Cross(transform.right, surfaceNormal);
                Quaternion targetRotation = Quaternion.LookRotation(forwardRelativeToSurfaceNormal, surfaceNormal); //check For target Rotation.
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 2); //Rotate Character accordingly.
            }
        }
    }
}