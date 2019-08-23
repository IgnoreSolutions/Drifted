using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Drifted.Input;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Drifted.UI
{
    public abstract class AbstractMenuItem
    {
        public bool CloseOnAction { get; set; } = true;

        public MonoBehaviour Parent;

        [SerializeField]
        public abstract string Text { get; set; }
        public virtual Func<bool> ClickAction { get; set; }
        public virtual Action HighlightAction { get; set; }
        public virtual Action DehighlightAction { get; set; }
        public virtual bool ActionMovesPlayer { get; set; } = false;

        private List<AbstractMenuItem> m_SubItems = null;
        public List<AbstractMenuItem> SubItems { get
            {
                return m_SubItems;
            }
            set
            {
                if(value != null)
                {
                    ClickAction = null;
                    m_SubItems = value;
                }
            }
        }
    }

    // TODO: rename to menu item. 
    public class MenuItem : AbstractMenuItem
    {
        public override string Text { get; set; } = "<No Text>";
        public bool IsContainer => ClickAction == null || SubItems != null;

        public MenuItem(string text, Func<bool> action, bool movePlayer)
        {
            Text = text;
            ClickAction = action;
            ActionMovesPlayer = movePlayer;
        }

        public MenuItem() { }

        public MenuItem(string text, AbstractMenuItem[] children, bool movePlayer)
        {
            Text = text;
            SubItems = new List<AbstractMenuItem>(children);
            ActionMovesPlayer = movePlayer;
        }
    }

    public class PopUpMenu
    {
        List<AbstractMenuItem> MenuItems = new List<AbstractMenuItem>();

        public event MenuItemsClearedDelegate MenuItemsCleared;
        public event MenuItemsChangedDelegate MenuItemsChanged;

        public delegate void MenuItemsClearedDelegate();
        public delegate void MenuItemsChangedDelegate();

        public PopUpMenu()
        {}

        public List<AbstractMenuItem> GetMenuItems() => MenuItems;

        public PopUpMenu(params AbstractMenuItem[] menuItems)
        {
            MenuItems.AddRange(menuItems);
        }

        public void AddMenu(AbstractMenuItem menuItem)
        {
            MenuItems.Add(menuItem);
            if (MenuItemsChanged != null) MenuItemsChanged();
        }
        public void ClearMenus()
        {
            MenuItems.Clear();
            if (MenuItemsCleared != null) MenuItemsCleared();
        }

        public static AbstractMenuItem MakeMenuItem(string text, Func<bool> clickAction, bool movePlayer = false)
        {
            return new MenuItem(text, clickAction, movePlayer);
        }

        public static AbstractMenuItem MakeMenuItem(string text, AbstractMenuItem[] children)
        {
            return new MenuItem(text, children, false);
        }

        public static AbstractMenuItem MakeMenuItem()
        {
            return new MenuItem();
        }
    }

    [RequireComponent(typeof(RectTransform))]
    [DisallowMultipleComponent]
    public class PopUpMenuView : MonoBehaviour
    {
        public MenuManager menuManager;

        [ReadOnly]
        public bool IsActive = false;

        [ReadOnly]
        public bool IsVisible = false;

        [ReadOnly]
        public bool AllowInput = true;

        public Font DefaultFont;

        public bool CurrentlyInUse => (!IsActive && !IsVisible);

        private PopUpMenu PopUpMenu;
        private bool updateThisFrame = true, updateInProgress = false;

        private RectTransform parentRectTransform;


        public event MenuDisplayedDelegate MenuDisplayed;
        public event MenuClosedDelegate MenuClosed;
        public delegate void MenuDisplayedDelegate();
        public delegate void MenuClosedDelegate();

        public int ID;

        private MenuItemView[] ThisMenusItems;

        void Awake()
        {
            ID = UnityEngine.Random.Range(0, 100);

            PopUpMenu = new PopUpMenu();
            PopUpMenu.MenuItemsChanged += () => updateThisFrame = true;

            parentRectTransform = GetComponent<RectTransform>();

            if(DriftedInputManager.IsController)
            {
                DriftedInputManager.SetUIActive();
            }

            //BuildPopUp();
        }

        private void OnDestroy()
        {
            if(DriftedInputManager.IsController)
            {
                //DriftedInputManager.SetPlayActive();
            }
        }

        private void Start()
        {
            BuildPopUp();
            //if(target != null) transform.position = Camera.main.WorldToScreenPoint(target.transform.position);
        }

        private void BuildPopUp()
        {
            if (UnityEngine.Application.isEditor && !UnityEngine.Application.isPlaying) return;

            updateInProgress = true;

            List<MenuItemView> newMenuItems = new List<MenuItemView>();
            int internal_count = 0;
            PopUpMenu.AddMenu(PopUpMenu.MakeMenuItem("Close", () => { HidePopup(); return true; }));
            foreach (var menuItem in PopUpMenu.GetMenuItems())
            {
                if (menuItem != null)
                {
                    GameObject menuItemGo = new GameObject($"{menuItem.Text} Menu");
                    menuItemGo.transform.SetParent(transform);
                    menuItemGo.AddComponent<LayoutElement>();
                    StayOnObject();


                    Text textComponent = menuItemGo.AddComponent<Text>();
                    textComponent.font = DefaultFont;
                    textComponent.color = Color.white;
                    textComponent.horizontalOverflow = HorizontalWrapMode.Overflow;
                    textComponent.verticalOverflow = VerticalWrapMode.Overflow;
                    textComponent.resizeTextMinSize = 22;
                    textComponent.fontSize = 22;
                    textComponent.resizeTextForBestFit = true;

                    if (menuItem.SubItems != null) textComponent.text = $"{menuItem.Text}  >>";
                    else textComponent.text = $"{menuItem.Text}";

                    MenuItemView newItemDisplay = menuItemGo.AddComponent<MenuItemView>();
                    newItemDisplay.AssociatedMenuItem = menuItem;

                    newMenuItems.Add(newItemDisplay);

                    Selectable sel = menuItemGo.AddComponent<Selectable>();
                    sel.targetGraphic = textComponent;
                    sel.colors = new ColorBlock
                    {
                        normalColor = Color.white,
                        highlightedColor = Color.yellow,
                        pressedColor = Color.gray,
                        disabledColor = new Color(.14f, .14f, .14f, .75f),
                        colorMultiplier = 1.0f,
                        fadeDuration = 0.17f
                    };

                    UIClickController clickHandler = menuItemGo.AddComponent<UIClickController>();
                    clickHandler.OnLeft = new UnityEvent();
                    clickHandler.OnLeft.AddListener(() =>
                    {
                        menuItem.ClickAction();
                        if (menuManager != null) menuManager.CloseMenu(ID);
                        MenuClosed?.Invoke();
                        //DriftedConstants.Instance.UI().MenuController.CloseMenuByID(ID);
                    });

                    if (internal_count == 0 && DriftedInputManager.IsController)
                    {
                        EventSystem.current.SetSelectedGameObject(menuItemGo);
                       //sel.Select();
                       //newItemDisplay.Highlight();
                    }

                    internal_count++;
                }

            }
            ThisMenusItems = newMenuItems.ToArray();
            updateInProgress = false;
        }

        private int CurrentMenuIndex = 0;

        void Update()
        {
            if (target != null) StayOnObject();
        }

        public void ShowPopup(float x, float y)
        {
            //parentRectTransform.anchorMin = Vector2.zero;
            //parentRectTransform.anchorMax = Vector2.zero;
            //parentRectTransform.position = new Vector2(x, y);
            parentRectTransform.position = new Vector3(x, y);
            IsVisible = true;
            parentRectTransform.gameObject.SetActive(true); 
            IsActive = true;
            DriftedConstants.Instance.UI().WindowManager.SetEnableBlankDetector(true);
            if (MenuDisplayed != null) MenuDisplayed();
        }

        public GameObject target;
        [SerializeField]
        private Vector3 Offset = new Vector3(0, -100);
        public void StayOnObject()
        {
            if(target != null)
            {
                parentRectTransform.position = Camera.main.WorldToScreenPoint(target.transform.position) + Offset;
            }
        }

        /// <summary>
        /// Autos the show popup. (Ok vs)
        /// </summary>
        /// <param name="target">The GameObject that you want the Camera to target when clicking. Or, leave it null if you don't want it to do that.</param>
        public void AutoShowPopup()
        {
            if(target != null)
            {
                DriftedConstants.Instance.Camera.FollowTarget = target.transform;
                this.target = target;

                var highlighter = target.GetComponent<Highlighter>();
                if(highlighter != null)
                {
                    MenuClosed += () => highlighter.Dehightlight();

                    highlighter.Highlight();
                }
            }

            if (CustomInputManager.IsController)
            {
                ShowPopup(new Vector2(Screen.width / 2, Screen.height / 2));
            }
            else
            {
                Vector3 showPos = new Vector3(UnityEngine.Input.mousePosition.x + (parentRectTransform.sizeDelta.x ),
                    UnityEngine.Input.mousePosition.y - (parentRectTransform.sizeDelta.y ));
                ShowPopup(showPos);
                SnapOnScreen();
            }
        }

        public void SnapOnScreen()
        {
            Vector2 aPos = parentRectTransform.anchoredPosition;
            float xpos = aPos.x;
            xpos = Mathf.Clamp(xpos, 0, Screen.width - parentRectTransform.sizeDelta.x);
            aPos.x = xpos;
            parentRectTransform.anchoredPosition = aPos;
        }

        //public void ShowPopupAtMouse() => ShowPopup(UnityEngine.Input.mousePosition);
        public void ShowPopup(Vector2 pos) => ShowPopup(pos.x, pos.y);


        public void HidePopup()
        {
            IsVisible = false;
            MenuClosed?.Invoke();
            ThisMenusItems = null;
            SetActive(false);
            //Destroy(this);
        }

        public void SetInactive()
        {
            IsActive = false;
            MenuClosed?.Invoke();
        }

        public void SetActive(bool destroyOnClose = false)
        {
            IsActive = false;
            MenuClosed?.Invoke();
            if(destroyOnClose)
            {
                ThisMenusItems = null;
                Destroy(this);
            }
        }

        public void AddItem(AbstractMenuItem item)
        {
            if(PopUpMenu == null)
            {
                Debug.LogWarning($"Internal menu null ({item.Text})");
                return;
            }
            PopUpMenu.AddMenu(item);
        }
         
        public void CallMenuClosing() => MenuClosed?.Invoke();
    }
}
