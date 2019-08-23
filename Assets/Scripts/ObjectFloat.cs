using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Rigidbody))]
[DisallowMultipleComponent]
public class ObjectFloat : MonoBehaviour
{
    [SerializeField]
    string SwimAnimationName = "Swimming";

    public float waterLevel = 0.0f;
    public float floatThreshold = 2.0f;
    public float waterDensity = 0.125f;
    public float downForce = 1.0f;

    float forceFactor;
    Vector3 floatForce;

    private Animator animator;
    private Rigidbody rb;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    [SerializeField]
    bool floating = false;

    // Update is called once per frame
    void FixedUpdate()
    {
        forceFactor = 1.0f - ((transform.position.y - waterLevel) / floatThreshold);

        if (forceFactor > 0.0f)
        {
            if (floating != true)
            {
                floating = true;
                animator.ResetTrigger(SwimAnimationName);
                animator.SetTrigger(SwimAnimationName);
            }

            floatForce = -Physics.gravity * GetComponent<Rigidbody>().mass * (forceFactor - GetComponent<Rigidbody>().velocity.y * waterDensity);
            floatForce += new Vector3(0.0f, -downForce * GetComponent<Rigidbody>().mass, 0.0f);
            rb.AddForceAtPosition(floatForce, transform.position);
        }
        else
        {
            if (floating)
            {
                floating = false;
                animator.ResetTrigger("Idle");
                animator.SetTrigger("Idle");
            }
        }
    }
}
