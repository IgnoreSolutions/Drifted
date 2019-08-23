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

using Drifted;
using Drifted.Input;
using Drifted.Interactivity;
using Drifted.UI;
using Drifted.UI.WindowManager;
using MikeSantiago.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Drifted.Player
{
    [DisallowMultipleComponent]
    public class PlayerRaycaster : MonoBehaviour
    {
        public MenuManager menuManager;
        public BoolReference menusActiveReference;

        [ReadOnly]
        public string RaycastResult;
        public float Radius = 2.0f;
        public MikeWindowManager WM;

        [SerializeField]
        [ReadOnly]
        private int RaycastLayerMask = (1 << LayerDefinitions.DriftedEntities) | (1 << LayerDefinitions.DriftedEnvironment);
        private bool inSubMenu = false;
        private EzTimer menuClearedActivation;
        private bool allowedToShowAgain = true;

        // Start is called before the first frame update
        void Awake()
        {
            menusActiveReference = new BoolReference();
        }
        void Start()
        {
            //DriftedConstants.Instance.OnSceneLoaded += () => WM = DriftedConstants.Instance.UI().WindowManager;
            menuClearedActivation = new EzTimer(1.0f, () => allowedToShowAgain = true, false);

        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, Radius);
        }

        private MonoBehaviour FindClickHandler(GameObject obj)
        {
            foreach (var behav in obj.GetComponents<MonoBehaviour>())
            {
                if (behav is ISceneInteractable) return behav;
            }
            return null;
        }

        public bool CanCook()
        {
            var firePits = GetBehavioursInRadius<FirePitScript>();

            var pit = firePits.FirstOrDefault();

            if (pit.CurrentPitState == (FirePitStates.Charcoal | FirePitStates.LogsBurning | FirePitStates.LogsHalfBurnt)) return true;
            return false;
        }

        private ISceneInteractable[] GetObjectsInRadius()
        {
            Collider[] hit = Physics.OverlapSphere(transform.position, Radius, RaycastLayerMask);

            List<ISceneInteractable> castItems = new List<ISceneInteractable>();
            if (hit.Length > 0)
            {
                string colliders = "";
                for (int i = 0; i < hit.Length; i++)
                {
                    ISceneInteractable handler = FindClickHandler(hit[i].gameObject) as ISceneInteractable;
                    if (handler != null)
                    {
                        if (i == hit.Length - 1) colliders += hit[i].gameObject.name;
                        else colliders += $"{hit[i].gameObject.name}, ";
                        castItems.Add(handler);
                    }
                }

                RaycastResult = colliders;
                return castItems.ToArray();
            }
            return null;
        }

        private T[] GetBehavioursInRadius<T>() where T : MonoBehaviour
        {
            Collider[] hit = Physics.OverlapSphere(transform.position, Radius * 2, RaycastLayerMask);

            T[] castItems = new T[10];
            if (hit.Length > 0)
            {
                for (int i = 0; i < hit.Length; i++)
                {
                    if (i > castItems.Length) break;

                    T behaviour = hit[i].gameObject.GetComponent<T>();
                    if (behaviour != null && behaviour.gameObject.name != gameObject.name) castItems[i] = behaviour;
                }
                return castItems;
            }
            return null;
        }

        private PopUpMenuView ThisCursorMenu;
        private void MakeObjectChoiceMenu(ISceneInteractable[] objects)
        {
            List<AbstractMenuItem> items = new List<AbstractMenuItem>();

            Vector2 centerScreen = new Vector2(Screen.width / 2, Screen.height / 2);
            foreach (var obj in objects)
            {
                DriftedSceneInteractable monoBehaviour = obj as DriftedSceneInteractable;
                if (monoBehaviour == null) continue;

                string menuName = monoBehaviour.gameObject.TruncatedName(15);
                if (!string.IsNullOrEmpty(monoBehaviour.GetFriendlyName())) menuName = monoBehaviour.GetFriendlyName();

                var menu = PopUpMenu.MakeMenuItem($"{menuName}    >>>", () => { obj.Interact(this); return true; });
                menu.HighlightAction = () =>
                {
                    Debug.Log("Highlight!");
                    Highlighter highlighter = monoBehaviour.gameObject.GetComponent<Highlighter>();
                    if (highlighter != null)
                        highlighter.Highlight();
                };
                menu.DehighlightAction = () =>
                {
                    Highlighter highlighter = monoBehaviour.gameObject.GetComponent<Highlighter>();
                    if (highlighter != null)
                        highlighter.Dehightlight();
                };
                items.Add(menu);
            }

            var finalPopUp = menuManager.MakePopUpMenu(this.gameObject, items.ToArray());
        }

        private void ActuallyAllowedToShow() => allowedToShowAgain = true;

        private void HandleInteraction()
        {

            //            if (ThisCursorMenu == null && allowedToShowAgain && !DriftedConstants.Instance.FullScreenUIActive && !DriftedConstants.Instance.UIFocused && !DriftedConstants.Instance.MenusActive)
            if (menusActiveReference.Value) // Menus Active so we're ignoring. 
            {
                return;
            }

            if (DriftedInputManager.KeyDown("Interact") /*&& (WM.GetMenuController().IsPopUpActivated == false)*/)
            {
                var clickHandlers = GetObjectsInRadius();
                if (clickHandlers == null) return;
                // TODO: Is any pop up active flag

                if (clickHandlers.Length == 1)
                {
                    Debug.Log("Interacting with 1 object.");
                    allowedToShowAgain = false;
                    Invoke("ActuallyAllowedToShow", 1.0f);
                    //clickHandlers[0].Interact(new Vector3(Screen.width / 2, Screen.height / 2));
                    clickHandlers[0].Interact(this);
                }
                else if (clickHandlers.Length > 1)
                {
                    MakeObjectChoiceMenu(clickHandlers);
                }
                else { Debug.Log("Shrug"); }
            }
        }

        void Update()
        {
            HandleInteraction();
        }
    }
    //
}


