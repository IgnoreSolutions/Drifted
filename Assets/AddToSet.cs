using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddToSet : MonoBehaviour
{
    public TransformSet Set;

    // Start is called before the first frame update
    void Start()
    {
        if (Set != null) Set.Add(transform);
    }
}
