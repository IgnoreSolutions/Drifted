using System.Collections;
using System.Collections.Generic;
using Drifted;
using UnityEngine;
using UnityEngine.UI;

public class TempControllerIndicator : MonoBehaviour
{
    [SerializeField]
    new Animation animation;

    [ReadOnly]
    [SerializeField]
    bool IsController = CustomInputManager.IsController;

    [SerializeField]
    bool doItAnyway = false;

    void Start()
    {
        if ((IsController | doItAnyway) && animation != null) animation.Play();
        else
        {
            animation.enabled = false;
            animation.gameObject.GetComponent<CanvasGroup>().alpha = 0f;
        }
    }

    private void OnValidate()
    {
        animation = GetComponent<Animation>();
    }
}
