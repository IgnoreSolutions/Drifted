using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasAdjuster : MonoBehaviour
{
    Canvas _canvas;
    void Awake()
    {
        _canvas = GetComponent<Canvas>();
        if(_canvas != null)
        {
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }
    }
}
