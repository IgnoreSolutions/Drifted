using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleCullingSystem : MonoBehaviour
{
    CullingGroup cullingGroup;
    [SerializeField]
    float BoundRadius = 3.0f;
    [SerializeField]
    Vector3 Center = Vector3.zero;

    [SerializeField]
    ParticleSystem disableTarget;

    private void Start()
    {
        SetupCullingGroup();
    }

    void SetupCullingGroup()
    {
        if(cullingGroup == null)
        {
            cullingGroup = new CullingGroup();
        }

        cullingGroup.targetCamera = Camera.main;

        BoundingSphere[] spheres = new BoundingSphere[1];
        spheres[0] = new BoundingSphere(Center + transform.position, BoundRadius);
        cullingGroup.SetBoundingSpheres(spheres);
        cullingGroup.SetBoundingSphereCount(1);

        cullingGroup.onStateChanged += CullingGroup_OnStateChanged;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(Center + transform.position, BoundRadius);
    }

    void CullingGroup_OnStateChanged(CullingGroupEvent sphere)
    {
        if (sphere.hasBecomeVisible)
        {
            if (disableTarget != null) disableTarget.Play();
        }
        else if (sphere.hasBecomeInvisible)
        {
            if (disableTarget != null) disableTarget.Stop();
        }
        else Debug.Log("Limbo?");
    }

    private void OnEnable()
    {
        SetupCullingGroup();
    }

    private void OnDisable()
    {
        if(cullingGroup != null)
        {   
            cullingGroup.Dispose();
            cullingGroup = null;
        }
    }


}
