using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

// TODO: optimize the FUCK out of this......

public class BoarTest : MonoBehaviour
{
    /**
        Unity Configurable fields.
     */
    public float MovementSpeed = 5;
    public float DirectionChangeSpeed = 3.0f;
    public float MaxLookVariation = 30;
    public float SlowDownSpeed = 2.5f;
    public float StartMovingSpeed = 2.7f;
    public Vector3 currentMoveSpeed = Vector3.zero;
    public Vector3 CurrentVelocity = Vector3.zero;
    public float AnimationMoveSpeed = 0.0f;
    public UnityEngine.UI.Text DebugText;
    /**
        End Unity fields.
     */

    /**
        Internal variables.
     */
    private float heading;
    private Vector3 targetRotation;

    private Rigidbody boarRb;
    private Animator animalAnimator;
    private EzTimer BehaviourChangeTimer;

    private MikeAI AITestV2;
    /**
        End Internal variables
     */

    private void Start()
    {
        // Pre-defining the lookAndMove action because I'm too damn lazy to make another method for it!
        Action lookAndMove =  () =>
        {
            /*He look, and he MOVE*/
            LookAround();
            Move();
        };

        // This is awesome! Just pass it in all the behaviours you want it to go through.
        AITestV2 = new MikeAI(LookAround, Move, DontMove, lookAndMove);

        // Any time the behaviour is changed, this event is called.
        // If the new behaviour is the same as the old behaviour, this WONT be called.
        AITestV2.BehaviourChange += (e) =>
        {
            // Every time the behaviour is changed, let's just update all the internal timers.

            this.LookStartTime = Time.time;
            this.DontMoveStart = Time.time;
            this.MoveStart = Time.time;
        };
    }

    void OnCollisionEnter(Collision col)
    {
        // Debug-draw all contact points and normals
        foreach (ContactPoint contact in col.contacts)
        {
            Debug.DrawRay(contact.point, contact.normal, Color.white);
        }

        if(col.gameObject.name == "Water")
        {
            Debug.Log("Water colliding, making him go the other way!");
            heading = (((targetRotation.y + 180) + UnityEngine.Random.Range(0, 30)) % 360);
            targetRotation = new Vector3(0, heading, 0);
            transform.eulerAngles = targetRotation;
            AITestV2.SetCurrentBehaviourIndex(1);
        }
    }

    void Awake()
    {
        boarRb = GetComponent<Rigidbody>();
        animalAnimator = GetComponent<Animator>(); 
        //controller = GetComponent<CharacterController>();

        // Set random initial rotation
        heading = UnityEngine.Random.Range(0, 360);
        transform.eulerAngles = new Vector3(0, heading, 0);

        //StartCoroutine(NewHeading());
    }

    void Update()
    {
        if (AITestV2 == null) throw new Exception("You dun fucked up.");

        if (DebugText != null)
            DebugText.text = AITestV2.CurrentBehaviourName();

        AITestV2.UpdateAI(Time.deltaTime);


    }

    private float LookStartTime = 1.0f;
    /// <summary>
    /// Calculates a new direction to move towards.
    /// </summary>
    void LookAround()
    {
        var floor = Mathf.Clamp(heading - MaxLookVariation, 0, 360);
        var ceil = Mathf.Clamp(heading + MaxLookVariation, 0, 360);
        heading = UnityEngine.Random.Range(floor, ceil);
        targetRotation = new Vector3(0, heading, 0);

        currentMoveSpeed = Vector3.zero;
        transform.eulerAngles = Vector3.Slerp(transform.eulerAngles, targetRotation, (Time.time - LookStartTime) * DirectionChangeSpeed);

        //animalAnimator.SetBool("Feeding", false);
    }

    private float MoveStart = 1.0f;
    void Move()
    {
        transform.eulerAngles = Vector3.Slerp(transform.eulerAngles, targetRotation, Time.deltaTime * DirectionChangeSpeed);

        Vector3 newVelocity = boarRb.velocity;
        var forward = transform.forward;
        var targetDirection = forward * MovementSpeed;

        newVelocity.x = Mathf.Lerp(boarRb.velocity.x, (targetDirection.x), (Time.time - MoveStart) * StartMovingSpeed);
        newVelocity.z = Mathf.Lerp(boarRb.velocity.z, (targetDirection.z), (Time.time - MoveStart) * StartMovingSpeed);

        //currentMoveSpeed = Vector3.Lerp(currentMoveSpeed, targetSpeed, (Time.time - MoveStart) * StartMovingSpeed);
        //currentMoveSpeed = forward * MovementSpeed;
        boarRb.velocity = newVelocity;
        CurrentVelocity = boarRb.velocity;
        AnimationMoveSpeed = Mathf.Abs(CurrentVelocity.magnitude);
        animalAnimator.SetFloat("MoveSpeed", AnimationMoveSpeed);
    }

    private float DontMoveStart = 1.0f;
    void DontMove()
    {
        Vector3 targetVelocity = new Vector3(0, boarRb.velocity.y, 0); // Still allow the natural Y velocity so he falls.

        currentMoveSpeed = Vector3.Lerp(boarRb.velocity, Vector3.zero, (Time.time - DontMoveStart) * SlowDownSpeed);
        boarRb.velocity = currentMoveSpeed;

        CurrentVelocity = boarRb.velocity;
        AnimationMoveSpeed = Mathf.Abs(CurrentVelocity.magnitude);
        animalAnimator.SetFloat("MoveSpeed", AnimationMoveSpeed);
    }
}






