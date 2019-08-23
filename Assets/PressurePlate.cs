using System.Collections;
using System.Collections.Generic;
using Drifted;
using UnityEngine;
using UnityEngine.Events;

public class PressurePlate : MonoBehaviour
{
    [SerializeField]
    UnityEvent OnPress;

    [SerializeField]
    UnityEvent OnDepress;

    [SerializeField]
    float DepressAmount = 0.02f;

    private void OnCollisionEnter(Collision collision)
    {
        PlayerMovement script = collision.collider.GetComponent<PlayerMovement>();

        if(script != null && script.GetInstanceID() > GetInstanceID())
        {
            Debug.Log("Player pressed the pressure plate!!!");
            transform.position = new Vector3(transform.position.x, transform.position.y - DepressAmount, transform.position.z);
            OnPress.Invoke();
        }

        // If it's the player
        if(collision.gameObject == DriftedConstants.Instance.Player().Movement.gameObject)
        {

        }
    }

    private void OnCollisionExit(Collision collision)
    {
        // If it's the player
        if (collision.gameObject == DriftedConstants.Instance.Player().Movement.gameObject)
        {
            Debug.Log("Player left the pressure plate!!!");
            transform.position = new Vector3(transform.position.x, transform.position.y + DepressAmount, transform.position.z);
            OnDepress.Invoke();
        }
    }
}
