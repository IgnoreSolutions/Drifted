using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Drifted.Entities
{
    public class EntityHealthBar : MonoBehaviour
    {
        public Image Meter;
        public Text Name;

        [ReadOnly]
        public float currentValue = .5f;

        public void SetFill(float value)
        {
            currentValue = value;
            UpdateView();
        }

        void UpdateView()
        {
            if(Meter == null)
            {
                Debug.LogWarning("Meter is null");
                return;
            }
            Meter.fillMethod = Image.FillMethod.Horizontal;
            Meter.fillAmount = currentValue;

            if(currentValue > 1.45f) Meter.color = Color.white;
            if(currentValue > 1.0f) Meter.color = new Color(0f, 00f, 0f, 1.0f);
            if(currentValue > .65f) Meter.color = Color.green;
            if(currentValue > .25f && currentValue < .65f) Meter.color = Color.yellow;
            if(currentValue <= .2f) Meter.color = Color.red;
        }

        void OnValidate()
        {
            UpdateView();
        }

        public void SetVisibility(bool visible)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(visible);
            }
        }
    }
}