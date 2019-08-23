using UnityEngine;
using System.Collections;

public class WMStayOnScreen : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        RectTransform thisRectTransform = GetComponent<RectTransform>();
        if(thisRectTransform != null)
        {
            Vector2 newPosition = thisRectTransform.position;
            newPosition.x = Mathf.Clamp(newPosition.x, 0, Screen.width - (thisRectTransform.sizeDelta.x / 2));
            newPosition.y = Mathf.Clamp(newPosition.y, 0, Screen.height + (thisRectTransform.sizeDelta.y));
            thisRectTransform.position = newPosition;
        }
    }
}
