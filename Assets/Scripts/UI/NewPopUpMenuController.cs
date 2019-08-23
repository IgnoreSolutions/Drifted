using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Drifted;
using System.Collections;

namespace Drifted.UI
{
    /// TODO: rename it to MenuManager
    public class NewPopUpMenuController : MonoBehaviour
    {
        [ReadOnly]
        public string Hello = "This works so far i think :)";
        Dictionary<int, PopUpMenuView> MenuControllers = new Dictionary<int, PopUpMenuView>();

        private bool DelayComplete = true;

        public bool MenusActive
        {
            get
            {
                if (!DelayComplete) return true;
                return MenuControllers.Count > 0;
            }
        }

        //List<NewPopUpMenuDisplay> MenuControllers = new List<NewPopUpMenuDisplay>();

        public void Awake()
        {}

        private void SetDelayComplete() => DelayComplete = true;

        private void MakeAllInactiveExcept(int id)
        {
            lock (((IDictionary)MenuControllers).SyncRoot)
            {
                var controllersToMakeInactive = MenuControllers.Where(x => x.Key != id).ToArray();
                foreach (var ctrl in controllersToMakeInactive) ctrl.Value.IsActive = false;
            }
        }

        public void HideMenuByID(int id)
        {
            lock(((IDictionary)MenuControllers).SyncRoot)
            {
                // TODO: destroy
                if (MenuControllers.ContainsKey(id))
                {
                    MenuControllers[id].CallMenuClosing();
                    MenuControllers[id].SetActive(true);
                }
            }
        }

        public void CloseMenuByID(int id)
        {
            lock (((IDictionary)MenuControllers).SyncRoot)
            {
                // TODO: destroy
                if (MenuControllers.ContainsKey(id))
                {
                    //Debug.Log($"Destroying menu with ID {id}");
                    MenuControllers[id].CallMenuClosing();
                    DestroyImmediate(MenuControllers[id].gameObject);
                    MenuControllers.Remove(id);
                    DelayComplete = false;
                    Invoke("SetDelayComplete", 0.25f);
                }
            }
        }

        private void Update()
        {
            lock(((IDictionary)MenuControllers).SyncRoot){
                //if (MenuControllers.Count <= 0) DriftedConstants.Instance.UIFocused = false;

                if (UnityEngine.Application.isEditor)
                {
                    Hello = "";
                    if (MenuControllers.Count > 0)
                    {
                        foreach (var menu in MenuControllers)
                        {
                            Hello += $"{menu.Key}, ";
                        }
                    }
                    else Hello = "No menu controller! :(";
                }

                if (CustomInputManager.KeyDown(CustomInputManager.GetCurrentMapping().Cancel))
                {
                    // TODO: not close all and instead just close the active and go back to the parent if applicable
                    DriftedConstants.Instance.UI().MenuController.CloseAll();
                    //DriftedConstants.Instance.MenuController.CloseMenuByID(ID);
                    //HidePopup();
                }
            }
        }

        public void CloseAll()
        {
            lock (((IDictionary)MenuControllers).SyncRoot)
            {
                //Debug.Log("Close all " + MenuControllers.Count);
                //DriftedConstants.Instance.UIFocused = false;
                foreach (var popup in MenuControllers.ToList())
                {
                    CloseMenuByID(popup.Key);
                }

                DriftedConstants.Instance.UI().WindowManager.SetEnableBlankDetector(false);
            }
        }

        public PopUpMenuView MakePopUpMenu(Vector2 position, MonoBehaviour parent, params AbstractMenuItem[] items)
        {
            var menu = MakePopUpMenu(parent, items);
            menu.transform.position = position;

            return menu;
        }

        public PopUpMenuView MakePopUpMenu(MonoBehaviour parent, AbstractMenuItem item) => MakePopUpMenu(parent, new AbstractMenuItem[] { item });

        public PopUpMenuView MakePopUpMenu(MonoBehaviour parent, params AbstractMenuItem[] items)
        {
            GameObject newPopup = new GameObject("Pop-Up");
            newPopup.transform.SetParent(transform);

            return null;
        }
    }
}
