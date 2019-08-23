// /**
// NavMeshBoar.cs
// Created 3/24/2019 7:39 AM
//
// Copyright (C) 2019 Mike Santiago - All Rights Reserved
// axiom@ignoresolutions.xyz
//
// Permission to use, copy, modify, and/or distribute this software for any
// purpose with or without fee is hereby granted, provided that the above
// copyright notice and this permission notice appear in all copies.
//
// THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES
// WITH REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES OF
// MERCHANTABILITY AND FITNESS. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR
// ANY SPECIAL, DIRECT, INDIRECT, OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES
// WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN
// ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF
// OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.
//
// */
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


/**
 * Let's talk about boars
 * 
 * Boars need to do the following.
 * 
 * 1. Walk anywhere
 * 2. Find food
 * 3. Stand still
 * 
 * 
 * 
 **/
public class NavMeshBoar : MonoBehaviour
{
    public GameObject CurrentTarget;
    public Vector3 CurrentVelocity;
    public float ActualDistanceFromTarget;
    public float MaxDistanceFromTarget = 3.0f;

    public float FeedTime = 6.0f;

    private NavMeshAgent navMesh;
    private Vector3 PositionLastFrame;

    List<GameObject> FoodSourceList = new List<GameObject>();

    public bool IsFeeding = false;
    private EzTimer FeedTimer = null;

    void BuildFoodList(string foodName)
    {
        GameObject[] temp = (GameObject[])FindObjectsOfType<GameObject>();

        foreach(var go in temp)
        {
            if(go.name == foodName) FoodSourceList.Add(go);
        }
    }

    private void Awake()
    {
        navMesh = GetComponent<NavMeshAgent>();
        BuildFoodList("fern");

        if (FoodSourceList.Count > 0 && CurrentTarget == null)
        {
            CurrentTarget = FoodSourceList[Mathf.FloorToInt(UnityEngine.Random.Range(0, (FoodSourceList.Count - 1)))];
        }

        FeedTimer = new EzTimer(FeedTime, () =>
        {
            if (FoodSourceList.Count > 0 && CurrentTarget != null)
            {
                Feed(false);
                FoodSourceList.Remove(CurrentTarget);
                Destroy(CurrentTarget);
                CurrentTarget = null;
                SetNewTarget();
            }
        }, false);
    }

    private void SetNewTarget()
    {
        if (FoodSourceList.Count > 0 && CurrentTarget == null)
        {
            CurrentTarget = FoodSourceList[Mathf.FloorToInt(UnityEngine.Random.Range(0, (FoodSourceList.Count - 1)))];
        }
        else
        {
            CurrentTarget = GameObject.FindWithTag("Player");
        }
    }




    // Update is called once per frame
    void Update()
    {
        if (FeedTimer != null && FeedTimer.enabled) FeedTimer.Tick(Time.deltaTime);
        //FeedTimer.Tick(Time.deltaTime);

        if(CurrentTarget == null) // set current target to the player instead! bc there's no food and i'm hungry
        {
            CurrentTarget = GameObject.FindWithTag("Player");
            return;
        }

        if (!navMesh.isStopped)
            navMesh.destination = CurrentTarget.transform.position;

        CurrentVelocity = navMesh.velocity;
        GetComponent<Animator>().SetFloat("MoveSpeed", Mathf.Abs(CurrentVelocity.magnitude));

        //RemainingDistanceFromTarget = navMesh.remainingDistance;
        ActualDistanceFromTarget = Vector3.Distance(transform.position, CurrentTarget.transform.position);

        if (ActualDistanceFromTarget <= MaxDistanceFromTarget && !navMesh.isStopped)
        {
            navMesh.isStopped = true;
            Feed(navMesh.isStopped);
        }
        else if (ActualDistanceFromTarget >= MaxDistanceFromTarget && navMesh.isStopped)
        {
            navMesh.isStopped = false;
            Feed(navMesh.isStopped);
        }
    }

    void Feed(bool begin)
    {
        IsFeeding = begin;
        if (begin && (FoodSourceList.Count > 0)) FeedTimer.Start();
    }
}
