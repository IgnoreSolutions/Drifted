using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// TODO: revisit this when I have meshes combined.
public class CameraPoke : MonoBehaviour
{
    [SerializeField]
    float distance = 10.0f;

    [SerializeField]
    bool oneMinus = false;

    [SerializeField]
    Material newMat;

    private Dictionary<int, MeshRenderer> instanceDictionary;

    void Awake()
    {
        instanceDictionary = new Dictionary<int, MeshRenderer>();
    }

    private void OnDrawGizmos() 
    {
        Debug.DrawRay(transform.position, transform.forward * distance, Color.red);    
    }

    private void LateUpdate() 
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.forward, out hit, distance))
        {
            if(hit.collider != null && hit.collider.gameObject != null)
            {
                MeshRenderer mr = null;
                bool alreadyContains = false;
                if(instanceDictionary.ContainsKey(hit.collider.gameObject.GetInstanceID()))
                {
                    MeshRenderer possibleRenderer = instanceDictionary[hit.collider.gameObject.GetInstanceID()];
                    if(possibleRenderer == null) instanceDictionary.Remove(hit.collider.GetInstanceID());
                    else{ alreadyContains = true; mr = possibleRenderer;}
                }
                
                if(mr == null)
                {
                    var renderers = hit.collider.gameObject.GetComponentsInChildren<MeshRenderer>();

                }

                if(mr != null)
                {
                    mr.material = newMat;

                    if(oneMinus) mr.material.SetFloat("_Transparency", 1 - (distance / hit.distance));
                    else mr.material.SetFloat("_Transparency", (distance / hit.distance));
                    
                    if(!alreadyContains) instanceDictionary.Add(hit.collider.gameObject.GetInstanceID(), mr);
                }
            }
        }    
    }
}
