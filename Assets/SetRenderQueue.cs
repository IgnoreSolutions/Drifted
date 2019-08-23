using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class SetRenderQueue : MonoBehaviour
{
    Material mat;

    public int RenderQueueOrder = 2;
    // Start is called before the first frame update
    void Start()
    {
        mat = GetComponent<MeshRenderer>().material;

        mat.renderQueue = RenderQueueOrder;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
