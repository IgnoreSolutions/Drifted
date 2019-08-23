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

using Drifted.Moods;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[System.Serializable]
// TODO: public class MoodData

[System.Serializable]
public class SerializableVector3
{
    public float x, y, z;

    public SerializableVector3(Vector3 unityVec)
    {
        x = unityVec.x;
        y = unityVec.y;
        z = unityVec.z;
    }

    public Vector3 ToUnityVector()
    {
        return new Vector3(x, y, z);
    }

    public override string ToString()
    {
        return $"({x}, {y}, {z})";
    }
}

[System.Serializable]
public class PlayerData
{
    public SerializableVector3 PlayerPosition = new SerializableVector3(Vector3.zero);

    public PlayerData()
    {

    }

    public PlayerData(GameObject player)
    {
        if(player.tag == "Player")
        {
            PlayerPosition = new SerializableVector3(player.transform.position);
            //MoodsController moodController = player.GetComponent<MoodsController>();
        }
    }
}
