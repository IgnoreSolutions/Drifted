using Drifted.Interactivity;
using Drifted.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Drifted.UI
{
    [RequireComponent(typeof(Text))]
    public class MenuItemView : MonoBehaviour, ISceneHighlightable
    {
        public event EventHandler ActionClickEvent;

        private Color HighlightColor = Color.yellow;
        private Color TextColor = Color.white;

        private AbstractMenuItem m_MenuItem = null;

        public AbstractMenuItem AssociatedMenuItem
        {
            get
            {
                return m_MenuItem;
            }
            set
            {
                m_MenuItem = value;
                if(value != null) update = true;
            }
        }

        public Text TextComponent { get; set; }
        public PopUpMenuView Parent { get; set; }

        private bool update = true;

        public void Awake()
        {
            TextComponent = GetComponent<Text>();
            Parent = GetComponentInParent<PopUpMenuView>();
        }

        private void Update()
        {
            if(update)
            {
                if(m_MenuItem.SubItems != null && m_MenuItem.SubItems.Count > 0)
                {
                    TextComponent.text = $"{m_MenuItem.Text}  >>";
                }
                else TextComponent.text = $"{m_MenuItem.Text}";
                update = false;
            }
        }

        public void Interact()
        {
            if (m_MenuItem != null)
            {
                if(AssociatedMenuItem.ActionMovesPlayer)
                {
                    DriftedConstants.Instance.Player().EnqueueAction(
                        DriftedConstants.Instance.Player().MovePlayerTo(AssociatedMenuItem.Parent.transform)
                    );
                    DriftedConstants.Instance.Player().EnqueueWait(0.5f);
                    DriftedConstants.Instance.Player().EnqueueAction(m_MenuItem.ClickAction);
                }
                else
                {
                    DriftedConstants.Instance.Player().EnqueueAction(m_MenuItem.ClickAction);
                }
            }
            if(AssociatedMenuItem.CloseOnAction)
            {
                DriftedConstants.Instance.UI().MenuController.CloseMenuByID(Parent.ID);
            }
        }

        public void Highlight()
        {
            TextComponent.color = HighlightColor;
            AssociatedMenuItem.HighlightAction?.Invoke();
        }

        public void Unhighlight()
        {
            TextComponent.color = TextColor;
            AssociatedMenuItem.DehighlightAction?.Invoke();
        }

        /*
        public void OnPointerEnter(PointerEventData eventData)
        {
            Highlight();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Interact();
            if(AssociatedMenuItem.CloseOnAction)
            {
                //Debug.Log("Hide " + Parent.ID);
                DriftedConstants.Instance.MenuController.CloseMenuByID(Parent.ID);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Unhighlight();
        }
        */
    }
}
