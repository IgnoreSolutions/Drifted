/// Copyright (C) 2019 Mike Santiago (admin@ignoresolutions.xyz)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MikeSantiago.Extensions;

namespace Drifted
{
    public class ThreeDify : MonoBehaviour
    {
        [SerializeField]
        Sprite targetSprite;

        [SerializeField]
        Transform targetParent;

        [SerializeField]
        Material materialToApply;

        void Start()
        {
        }

        void DestroyChildren()
        {
            if(targetParent == null) return ;

            for(int i = targetParent.childCount - 1; i >= 0; i--)
            {
                StartCoroutine(Destroy(targetParent.GetChild(i).gameObject));
            }
        }

        IEnumerator Destroy(GameObject go)
        {
             yield return new WaitForEndOfFrame();
            DestroyImmediate(go);
        }

        void Generate(Texture2D spriteTexture)
        {
            if(spriteTexture == null) return;

            for(int y = 0; y < spriteTexture.width; y++)
                {
                    for(int x = 0; x < spriteTexture.height; x++)
                    {
                        Color pixelColor = spriteTexture.GetPixel(x, y);

                        if(pixelColor.a <= 0.2f) continue;

                        var pixel = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        var mr = pixel.GetComponent<MeshRenderer>();

                        mr.material.color = pixelColor;
                        pixel.transform.SetParent(targetParent);
                        pixel.name = $"Pixel {x},{y}";
                        pixel.transform.localScale = targetParent.localScale;
                        pixel.transform.localPosition = new Vector3(targetParent.transform.localPosition.x + x, 
                                                                targetParent.transform.localPosition.y + y, 
                                                                targetParent.transform.localPosition.z /* +y */);
                    }
                }

            var combiner = targetParent.gameObject.AddComponent<MeshCombiner>();
            combiner.CombineMeshes(materialToApply, spriteTexture);
        }

        bool generated = false;

        void Update()
        {
            if(!generated)
            {
                generated = true;
                DestroyChildren();
                Generate(targetSprite.ToTexture());
            }
        }

        void OnValidate()
        {
            if (targetSprite != null && targetParent != null)
            {
                //DestroyChildren();
                //Generate(targetSprite.ToTexture());
            }
        }

    }
}