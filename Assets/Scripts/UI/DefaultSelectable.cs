using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Drifted.UI
{
    public class DefaultSelectable : MonoBehaviour
    {
        void OnEnable()
        {
            //EventSystem.current.SetSelectedGameObject(gameObject);
        }

        private void Start()
        {
            EventSystem.current.SetSelectedGameObject(gameObject);
        }
    }
}