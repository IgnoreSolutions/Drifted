/// Copyright (c) 2019 Mike Santiago (admin@ignoresolutions.xyz)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Drifted
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class MeshCombiner : MonoBehaviour
    {
        [SerializeField]
        bool ExecuteOnAwake = false;

        public void Awake()
        {
            if(ExecuteOnAwake && Application.isPlaying) CombineMeshes(null, null);
        }

        public void CombineMeshes(Material materialToApply = null, Texture2D texture = null)
        {
            MeshFilter[] childrenMeshFilters = GetComponentsInChildren<MeshFilter>();
            CombineInstance[] combineInstances = new CombineInstance[childrenMeshFilters.Length];

            int i = 0;
            while(i < childrenMeshFilters.Length)
            {
                combineInstances[i].mesh = childrenMeshFilters[i].sharedMesh;
                combineInstances[i].transform = childrenMeshFilters[i].transform.localToWorldMatrix;
                childrenMeshFilters[i].gameObject.SetActive(false);
                
                i++;
            }
            
            MeshFilter thisMf = transform.GetComponent<MeshFilter>();
            thisMf.mesh = new Mesh();
            thisMf.mesh.CombineMeshes(combineInstances, true);
            if(materialToApply != null)
            {
                MeshRenderer thisMr = transform.GetComponent<MeshRenderer>();
                thisMr.material = materialToApply;

                if(texture != null)
                {
                    thisMr.material.mainTexture = texture;
                }
            }
            thisMf.gameObject.SetActive(true);
        }    
    }
}