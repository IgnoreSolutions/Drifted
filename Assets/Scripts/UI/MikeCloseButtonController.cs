using System;
using Drifted.UI.WindowManager;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Drifted.UI
{

    public class MikeCloseButtonController : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        private Color oldHighlightColor;
        public Color HighlightColor = Color.red;

        public Text CloseButtonText;
        public MikeWindowManager WindowManager;
        public string WindowName;

        public bool DestroyOnClose = false;

        public void OnPointerClick(PointerEventData eventData)
        {
            transform.parent.parent.gameObject.SetActive(false);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            oldHighlightColor = CloseButtonText.color;

            if (CloseButtonText != null) CloseButtonText.color = HighlightColor;
        }

        public void OnPointerExit(PointerEventData eventData)
        {

            if (CloseButtonText != null)
            {
                CloseButtonText.color = Color.white;
            }
        }
    }
}