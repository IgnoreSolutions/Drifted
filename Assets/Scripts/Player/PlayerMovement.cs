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
using Drifted.Input;
using Drifted.Interactivity;
using Drifted.Player;
using Drifted.UI.WindowManager;
using Rewired;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class PlayerMovement : MonoBehaviour
{
    [Header("New Stuff")]
    public DriftedSceneManager SceneVariables;
    public GameEvent ResetFollowTargetEvent;

    [ReadOnly]
    public bool QueueActive = false;
    private CoroutineQueue ActivityQueue;

    public NavMeshAgent NavAgent;

    public float moveSpeed = 2.0f;
    public float animTurnSpeed = 10.0f;
    public Vector3 MaxVelocity = Vector3.zero;
    public float LerpTime = 5.0f;
    public Camera CameraReference;

    private Vector3 moveDir;
    private float yVel;
    private Rigidbody rb;
    public Animator anim;

    private float LerpTimer = 0.0f;

    [ReadOnly]
    public string RaycastResult = "";

    public Vector3 Velocity;

    public bool PlayerIsMoving
    {
        get => (Mathf.Abs(Velocity.x) > 0 || Mathf.Abs(Velocity.z) > 0);
    }

    [Header("Debug Inputs")]
    public bool DebugInputs = false;

    void Awake()
    {
        DriftedInputManager.SetupInputs(DebugInputs);

        if (CameraReference == null)
        {
            var _camera = GameObject.Find("CameraAnchor/Camera");
            if (_camera == null) throw new Exception("No CameraAnchor found!");

            CameraReference = _camera.GetComponent<Camera>();
        }
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

        ActivityQueue = new CoroutineQueue(this);
        ActivityQueue.StartLoop();

        if (NavAgent != null) NavAgent.enabled = false;
    }

    private void Start()
    {
        //DriftedConstants.Instance.Camera.ResetFollowTarget();
        if (SceneVariables != null) SceneVariables.Player = this;
    }

    bool IsNegative(float input) => (input < 0);

    bool currentlyMoving = false;
    void MovePlayer2()
    {
        Vector3 controlRight = Vector3.Cross(CameraReference.transform.up, CameraReference.transform.forward);
        Vector3 controlForward = Vector3.Cross(CameraReference.transform.right, Vector3.up);


        // Apply movement to control input
        //moveDir = (controlRight * Input.GetAxis("Horizontal")) + (controlForward * Input.GetAxis("Vertical"));
        moveDir = new Vector3(DriftedInputManager.GetAxis("MoveHorizontal"), DriftedInputManager.GetAxis("MoveForward"));
        if (anim.gameObject.activeSelf) //Prevents crap from being logged to the console.
        {
            anim.SetFloat("MoveSpeed", Mathf.Abs(rb.velocity.magnitude));
        }

        //aDebug.Log(moveDir.sqrMagnitude);
        if (moveDir.sqrMagnitude >= 0.5) currentlyMoving = true;

        if (currentlyMoving)
        {
            Vector3 movementSide = transform.right * moveDir.x;
            Vector3 movementForward = transform.forward * moveDir.y;

            rb.AddForce(movementSide * moveSpeed, ForceMode.Impulse);
            rb.AddForce(movementForward * moveSpeed, ForceMode.Impulse);
            Debug.Log("Force");
        }
        else
        {
            rb.velocity = Vector3.zero;
        }
    }

    [ReadOnly]
    public bool Moving = false;

    void MovePlayerControlled()
    {
        Vector3 controlRight = Vector3.Cross(CameraReference.transform.up, CameraReference.transform.forward);
        Vector3 controlForward = Vector3.Cross(CameraReference.transform.right, Vector3.up);

        // Apply movement to control input
        moveDir = (controlRight * DriftedInputManager.GetAxis("MoveHorizontal")) + (controlForward * DriftedInputManager.GetAxis("MoveForward"));
        if (anim.gameObject.activeSelf) //Prevents crap from being logged to the console.
        {
            anim.SetFloat("MoveSpeed", Mathf.Abs(rb.velocity.magnitude));
            Moving = true;
        }

        if (moveDir != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation((transform.position + moveDir) - transform.position), animTurnSpeed * Time.deltaTime);

        if (rb != null)
        {
            if (Mathf.Abs(moveDir.x) > float.Epsilon || Mathf.Abs(moveDir.z) > float.Epsilon) //moving 
            {
                Vector3 newVelocity = rb.velocity;
                newVelocity.x = Mathf.Lerp(rb.velocity.x, (MaxVelocity.x * moveDir.x), LerpTimer);
                newVelocity.z = Mathf.Lerp(rb.velocity.z, (MaxVelocity.z * moveDir.z), LerpTimer);
                //newVelocity.y = -5.0f;

                //if(CameraReference != null) rb.AddRelativeForce(newVelocity + CameraReference.transform.right /*is this correct?*/, ForceMode.Acceleration);
                rb.velocity = newVelocity;
                Velocity = newVelocity;

                //transform.forward = CameraReference.transform.right;
                //rb.velocity = (newVelocity * CameraReference.transform.right);

                // When the LerpTimer is maxed out, then the player will have reached their max speed.
                LerpTimer += Time.deltaTime * LerpTime;
                if (LerpTimer > LerpTime) LerpTimer = LerpTime;
            }
            else
            {
                // TODO: are these lines necessary?
                //rb.velocity = new Vector3(0, rb.velocity.y, 0);
                //rb.angularVelocity = new Vector3(0, rb.angularVelocity.y, 0);
                Moving = false;
                anim.SetFloat("MoveSpeed", 0);
                Velocity = rb.velocity;

                LerpTimer = 0;
            }
        }
    }

    public void OnPauseGame()
    {
        CanMove = false;
        ActivityQueue.Pause();
        Moving = false;
        anim.SetFloat("MoveSpeed", 0);
        anim.ResetTrigger("Idle");
        anim.SetTrigger("Idle");
    }

    public void OnResumeGame()
    {
        CanMove = true;
        ActivityQueue.Unpause();
    }

    internal void SetPosition(SerializableVector3 playerPosition, SerializableQuaternion playerRotation)
    {
        transform.position = playerPosition.ToUnityVector();
        transform.rotation = playerRotation.ToUnityQuaternion();
    }

    [ReadOnly]
    public bool AtTarget = false;
    public IEnumerator MovePlayerTo(Transform point)
    {
        if (NavAgent != null)
        {
            NavAgent.enabled = true;

            NavAgent.destination = point.position;
            Moving = true;
            NavAgent.isStopped = false;
            anim.SetFloat("MoveSpeed", NavAgent.speed);

            while ((AtTarget = NavAtTarget()) == false) // Wait until he's close.
            {
                yield return null;
            }

            NavAgent.enabled = false;
            yield return MakePlayerFaceTarget(point);

            Moving = false;
            anim.SetFloat("MoveSpeed", 0);
        }
    }

    private IEnumerator MakePlayerFaceTarget(Transform target)
    {
        float dot = Vector3.Dot(transform.forward, (target.position - transform.position).normalized);
        do
        {
            dot = Vector3.Dot(transform.forward, (target.position - transform.position).normalized);
            yield return null;

            var newRot = Quaternion.LookRotation(target.position - transform.position);
            for (float f = 0; f <= 0.9f; f += (animTurnSpeed * Time.deltaTime))
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, newRot, f);
                yield return null;
            }

        } while (dot < 0.55f);

        yield return null;
    }

    private bool NavAtTarget()
    {
        if (!NavAgent.isActiveAndEnabled) return false;
        if (!NavAgent.pathPending)
        {
            if (NavAgent.remainingDistance <= NavAgent.stoppingDistance)
            {
                if (!NavAgent.hasPath || NavAgent.velocity.sqrMagnitude <= 0.1f)
                {
                    return true;
                }
            }
        }

        return false;
    }

    void CalculatePlayerMovement()
    {
        bool rtrn = false;

        if (!rtrn)
        {
            // Ensures our player's forward direction is the same as the camera.
            Vector3 controlRight = Vector3.Cross(CameraReference.transform.up, CameraReference.transform.forward);
            Vector3 controlForward = Vector3.Cross(CameraReference.transform.right, Vector3.up);

            //moveDir = Vector3.zero;
            moveDir = Vector3.ClampMagnitude((controlRight * DriftedInputManager.GetAxis("MoveHorizontal")) + (controlForward * DriftedInputManager.GetAxis("MoveForward")), 1.0f);
        }

        if (moveDir != Vector3.zero) transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation((transform.position + moveDir) - transform.position), animTurnSpeed * Time.deltaTime);

        if (moveDir != Vector3.zero)
        {
            Moving = true;
            anim.SetFloat("MoveSpeed", Mathf.Abs(moveDir.sqrMagnitude));
        }
        else
        {
            Moving = false;
            anim.SetFloat("MoveSpeed", 0);
        }
    }

    [Header("Player Animation States")]
    public bool IsEating = false;
    public bool IsSwimming = false;
    public bool IsIdle = true;
    public bool IsHolding = false;
    public bool IsBuilding = false;
    public bool IsChopping = false;
    public bool IsFlinting = false;

    public bool CanMove = true;

    public bool IsDoingSomething()
    {
        return (IsEating || IsBuilding || IsChopping || IsFlinting);
    }

    public void PlayFireAnimation(bool play)
    {
        IsFlinting = play;
        StartCoroutine(PlayAnimation("FireStarting", play, true));
        //PlayAnimation("FireStarting", play, true);
    }

    public void PlaySwimAnimation(bool play)
    {
        IsSwimming = play; 
        StartCoroutine(PlayAnimation("Swimming", play, false));
    }

    public void PlayEatAnimation(bool play)
    {
        IsEating = play;
        StartCoroutine(PlayAnimation("Eating", play, true));
    }

    public void PlayChoppingAnimation(bool play)
    {
        if (play)
        {

            anim.SetBool("Holding", true);
            CanMove = false;
            IsHolding = true;
            IsChopping = true;
        }
        else
        {
            anim.SetBool("Holding", false);
            CanMove = true;
            IsHolding = false;
            IsChopping = false;
        }

        StartCoroutine(PlayAnimation("Chopping", play, true));
    }

    public void PlayBuildingAnimation(bool play)
    {
        IsBuilding = play;
        StartCoroutine(PlayAnimation("LowBuild", play, true));
    }


    private IEnumerator PlayAnimation(string animationName, bool play, bool lockMovement)
    {
        if (lockMovement) CanMove = false;

        if(play)
        {
            anim.ResetTrigger(animationName);
            anim.SetTrigger(animationName);
            IsIdle = false;
            yield return null;
        }
        else
        {
            anim.ResetTrigger("Idle");
            anim.SetTrigger("Idle");
            yield return new WaitForSeconds(2.5f);

            if (lockMovement) CanMove = true;
            IsIdle = true;
        }
        yield return null;
    }

    void Update()
    {
        if (ActivityQueue != null) QueueActive = ActivityQueue.Active;
        else QueueActive = false;
    }

    [SerializeField]
    [ReadOnly]
    float rbMagnitude = 0f;

    [SerializeField]
    Vector3 movementVector = Vector3.zero;
    private void FixedUpdate()
    {
        rbMagnitude = rb.velocity.sqrMagnitude;
        movementVector = moveDir;

        if(ActivityQueue != null && !ActivityQueue.Active && CanMove)
        {
            CalculatePlayerMovement();
            if (moveDir.sqrMagnitude > 0.2f)
            {
                // TODO: stop this from executing all the time
                //if(ResetFollowTargetEvent != null) ResetFollowTargetEvent.Raise(gameObject);
            }
            rb.MovePosition(rb.position + moveDir * moveSpeed * Time.fixedDeltaTime);
        }
    }

    // Queue Functions
    public void EnqueueWait(float waitTime) => ActivityQueue.EnqueueWait(waitTime);
    public void EnqueueAction(Action action) => ActivityQueue.EnqueueAction(action);
    public void EnqueueAction(Func<bool> action) => ActivityQueue.EnqueueAction(action);
    public void EnqueueAction(IEnumerator action) => ActivityQueue.EnqueueAction(action);
    //
}
