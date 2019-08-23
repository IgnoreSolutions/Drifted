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

using System;
using System.Collections;
using System.Collections.Generic;
using Drifted;
using Drifted.Player;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshBoar2 : MonoBehaviour
{
    private CoroutineQueue BoarQueue;
    private Rigidbody rb;

    [Header("Culling Group Properties", order = 50)]
    CullingGroup cullingGroup;
    [SerializeField]
    float CullingBoundRadius = 3.0f;
    [SerializeField]
    float DisappearDelayTime = 1.0f;

    void SetupCullingGroup()
    {
        if (cullingGroup == null)
        {
            cullingGroup = new CullingGroup();
        }

        cullingGroup.targetCamera = Camera.main;

        BoundingSphere[] spheres = new BoundingSphere[1000];
        spheres[0] = new BoundingSphere(transform.position, CullingBoundRadius);
        cullingGroup.SetBoundingSpheres(spheres);
        cullingGroup.SetBoundingSphereCount(1);

        cullingGroup.onStateChanged += CullingGroup_StateChanged;
    }

    private void CullingGroup_StateChanged(CullingGroupEvent sphere)
    {
        if (sphere.hasBecomeVisible)
        {
            if (BoarQueue != null)
            {
                BoarQueue.Unpause();
                rb.WakeUp();
            }
        }
        else if (sphere.hasBecomeInvisible)
        {
            if (BoarQueue != null)
            {
                BoarQueue.Pause();
                rb.WakeUp();
            }
        }
    }

    private void OnDisable()
    {
        if (cullingGroup != null)
        {
            cullingGroup.Dispose();
            cullingGroup = null;
        }
    }

    [SerializeField]
    [ReadOnly]
    bool Paused = false;

    public void OnPauseGame()
    {
        Paused = true;
        BoarQueue.Pause();
    }
    public void OnResumeGame()
    {
        Paused = false;
        BoarQueue.Unpause();
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float distance, int layermask)
    {
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * distance;

        randomDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randomDirection, out navHit, distance, layermask);

        return navHit.position;
    }

    public enum BoarBehaviour
    {
        Feed,
        IdleWander,
        TOTAL
    }

    public float TimeTillNextBehaviour = 30.0f;
    public float FeedTime = 6.0f;
    private NavMeshAgent navMeshAgent;
    private Vector3 positionLastFrame;
    private Animator animator;
    private Dictionary<BoarBehaviour, Action> Behaviours;
    public BoarBehaviour CurrentBehaviour;

    private Renderer thisRenderer;
    public void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        Behaviours = new Dictionary<BoarBehaviour, Action>();
        BuildActions();
        CurrentBehaviour = BoarBehaviour.IdleWander;
        animator = GetComponent<Animator>();

        NextWanderTarget();

        BoarQueue = new CoroutineQueue(this);
        BoarQueue.StartLoop();
        BoarQueue.EnqueueWait(1.0f);
        BoarQueue.EnqueueAction(ChangeBehaviour);

        thisRenderer = GetComponent<Renderer>();

        InvokeRepeating("UpdateSeen", 3.0f, 3.0f);

        SetupCullingGroup();

        rb = GetComponent<Rigidbody>();
    }

    private void UpdateSeen() => Seen = IsSeenByCamera();

    private void ChangeBehaviour()
    {
        if (BoarQueue.ActionCount() >= 10 /*|| !Seen*/) return;

        CurrentBehaviour = (BoarBehaviour)UnityEngine.Random.Range(0, (float)BoarBehaviour.TOTAL);

        BoarQueue.EnqueueAction(Behaviours[CurrentBehaviour]);
        BoarQueue.EnqueueAction(Look());
        BoarQueue.EnqueueAction(ChangeBehaviour);
    }

    public void BuildActions()
    {
        Behaviours.Add(BoarBehaviour.Feed, NextFeedTarget);
        Behaviours.Add(BoarBehaviour.IdleWander, NextWanderTarget);
        //Behaviours.Add(BoarBehaviour.Look, Look);
    }

    private IEnumerator SetFeedAnimation(bool play)
    {
        yield return null;
        animator.SetBool("Feeding", play);
        //animator.Play("Feeding", -1, 30);
    }

    [ReadOnly]
    public bool Seen = false;
    public bool IsSeenByCamera()
    {
        Vector3 screenPoint = Camera.main.WorldToViewportPoint(transform.position);
        bool onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
        Seen = onScreen;
        return onScreen;
    }

    private float lookLerp = 0f;
    private Quaternion originalLook;
    private Quaternion targetLook;
    private bool KeepLooking = false;
    private IEnumerator Look()
    {
        navMeshAgent.isStopped = true;
        int lookCount = UnityEngine.Random.Range(0, 8);

        for(int i = 0; i < lookCount; i++)
        {
            float lookDirection = UnityEngine.Random.Range(0, 360);
            originalLook = transform.localRotation;
            targetLook = Quaternion.Euler(transform.localRotation.x, lookDirection, transform.localRotation.z);
            animator.SetFloat("MoveSpeed", 1.0f);
            while (lookLerp < 1.0f)
            {
                yield return null;
                lookLerp = Mathf.Clamp01(lookLerp + Time.deltaTime);
                transform.localRotation = Quaternion.Slerp(originalLook, targetLook, lookLerp);
            }
            animator.SetFloat("MoveSpeed", 0.0f);
            yield return new WaitForSeconds(0.74f);
        }

        // Do shit after.
        BoarQueue.EnqueueAction(ChangeBehaviour);
    }

    private void NextWanderTarget()
    {
        float nextRandomDistance = UnityEngine.Random.Range(RandomDistanceRange.x, RandomDistanceRange.y);
        //Vector3 wanderPosition = NavMeshBoar2.RandomNavSphere(transform.position, nextRandomDistance, -1);
        Target = NavMeshBoar2.RandomNavSphere(transform.position, nextRandomDistance, -1);
        if (BoarQueue != null)
        {
            BoarQueue.EnqueueAction(MoveBoarTo(Target));
            BoarQueue.EnqueueWait(3.0f);
            BoarQueue.EnqueueAction(ChangeBehaviour);
        }
    }

    [ReadOnly]
    [SerializeField]
    private bool AtTarget = false;
    public IEnumerator MoveBoarTo(Vector3 position)
    {
        if(navMeshAgent != null)
        {
            navMeshAgent.destination = position;
            navMeshAgent.isStopped = false;
            animator.SetFloat("MoveSpeed", navMeshAgent.speed);

            while((AtTarget = NavAtTarget()) == false)
            {
                yield return null;
            }
            navMeshAgent.velocity = Vector3.zero;
            animator.SetFloat("MoveSpeed", 0);
        }
    }

    [ReadOnly]
    [SerializeField]
    private bool PathPending;
    private bool NavAtTarget()
    {
        PathPending = navMeshAgent.pathPending;
        if(!navMeshAgent.pathPending)
        {
            if(navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                //if (!navMeshAgent.hasPath) 
                return true;
            }
        }

        return false;
    }

    GameObject[] BuildFoodList(string foodName)
    {
        GameObject[] temp = (GameObject[])FindObjectsOfType<GameObject>();
        List<GameObject> final = new List<GameObject>();
        foreach (var go in temp)
        {
            if (go.name == foodName) final.Add(go);
        }

        return final.ToArray();
    }

    private void NextFeedTarget()
    {
        GameObject[] objects = BuildFoodList("fern");
        if(objects.Length == 0)
        {
            CurrentBehaviour = BoarBehaviour.IdleWander;
            NextWanderTarget();
        }
        else
        {
            int randomFern = UnityEngine.Random.Range(0, (objects.Length - 1));
            BoarQueue.EnqueueAction(MoveBoarTo(objects[randomFern].transform.position));
            BoarQueue.EnqueueAction(SetFeedAnimation(true));
            BoarQueue.EnqueueWait(20.0f);
            BoarQueue.EnqueueAction(SetFeedAnimation(false));
            BoarQueue.EnqueueAction(ChangeBehaviour);
        }
    }

    public bool IsStopped = false;
    public Vector3 Target = Vector3.zero;
    public float ActualDistanceFromTarget = 0;
    public float MaxDistanceFromTarget = 1.45f;
    public Vector2 RandomDistanceRange = new Vector2(1f, 20f);
    public float BehaviourTimeout = 0.0f;
    public float Velocity;

    [ReadOnly]
    public bool QueueActive = false;
    public void Update()
    {
        if (Paused) return;
        ActualDistanceFromTarget = navMeshAgent.remainingDistance;
        QueueActive = (BoarQueue != null && BoarQueue.Active);
        IsStopped = navMeshAgent.isStopped;
        Velocity = Mathf.Abs(navMeshAgent.velocity.magnitude);
        //if(Velocity > 0) animator.SetFloat("MoveSpeed", Mathf.Abs(navMeshAgent.velocity.magnitude));
        /*
        if (navMeshAgent.isStopped && CurrentBehaviour == BoarBehaviour.IdleWander)
        {
            NextWanderTarget();
            navMeshAgent.isStopped = false;
            animator.SetFloat("MoveSpeed", 2.0f);
        }

        if(navMeshAgent.isStopped && CurrentBehaviour == BoarBehaviour.Feed && !feedingBegun)
        {
            feedingBegun = true;

            Debug.Log("Feed time!");
            animator.SetFloat("MoveSpeed", 0);
            animator.SetBool("Feeding", true);
        }

        if (navMeshAgent.isStopped)
        {  
            animator.SetFloat("MoveSpeed", 0f);
        }
        */
    }

    private bool feedingBegun = false;
}

