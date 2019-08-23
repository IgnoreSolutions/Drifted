
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

namespace MikeSantiago.Extensions
{
    public static class MiscExtensions
    {
        public static Texture2D ToTexture(this Sprite sprite)
        {
            if(sprite == null) return null;

            if (sprite.rect.width != sprite.texture.width)
            {
                Texture2D newText = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
                Color[] newColors = sprite.texture.GetPixels((int)sprite.rect.x,
                                                             (int)sprite.rect.y,
                                                             (int)sprite.rect.width,
                                                             (int)sprite.rect.height);
                newText.SetPixels(newColors, 0);
                newText.Apply();
                return newText;
            }
            else
                return sprite.texture;
        }

        public static T CreateInstance<T>() where T : ScriptableObject
        {
            return (T)ScriptableObject.CreateInstance(typeof(T));
        }

        public static bool IsTrueNull(this UnityEngine.Object obj)
        {
            return ((object)obj) == null;
        }

        public static T RecursiveFindComponent<T>(this GameObject startingPoint) where T : MonoBehaviour
        {
            if (startingPoint == null) return default;

            for(int i = 0; i < startingPoint.transform.childCount; i++)
            {
                GameObject nextSearch = startingPoint.transform.GetChild(i).gameObject;
                if(nextSearch.transform.childCount > 0) return RecursiveFindComponent<T>(nextSearch);
                else
                {
                    T value = nextSearch.GetComponent<T>();
                    if (value != null) return value;
                }
            }

            return default;
        }

        public static string TruncatedName(this GameObject go, int length)
        {
            if (length > go.name.Length) return go.name;
            else return go.name.Substring(0, length);
        }
    }
}