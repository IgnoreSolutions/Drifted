// <auto-generated>
// Rewired Constants
// This list was generated on 6/3/2019 12:17:46 AM
// The list applies to only the Rewired Input Manager from which it was generated.
// If you use a different Rewired Input Manager, you will have to generate a new list.
// If you make changes to the exported items in the Rewired Input Manager, you will
// need to regenerate this list.
// </auto-generated>

namespace RewiredConsts {
    public static partial class Action {
        // Default
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Default", friendlyName = "Horizontal Movement")]
        public const int MoveHorizontal = 0;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Default", friendlyName = "Move Forward Backward")]
        public const int MoveForward = 1;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Default", friendlyName = "Reset Camera")]
        public const int ResetCamera = 2;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Default", friendlyName = "Pan Camera Left")]
        public const int PanCameraLeft = 3;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Default", friendlyName = "Pan Camera Right")]
        public const int PanCameraRight = 4;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Default", friendlyName = "Show Inventory")]
        public const int ToggleInventory = 5;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Default", friendlyName = "Show the Skills Interface")]
        public const int ToggleSkills = 6;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Default", friendlyName = "Show the Moods Panel")]
        public const int ToggleMoods = 7;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Default", friendlyName = "Pause the game")]
        public const int Pause = 8;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Default", friendlyName = "Enter Build Mode")]
        public const int ToggleBuildMode = 9;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Default", friendlyName = "Horizontal camera control")]
        public const int CameraHorizontal = 10;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Default", friendlyName = "Move the camera vertically.")]
        public const int CameraVertical = 11;
        // UI
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Interface Controls", friendlyName = "UI Horizontal")]
        public const int UIHorizontal = 12;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Interface Controls", friendlyName = "UI Vertical")]
        public const int UIVertical = 13;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Interface Controls", friendlyName = "UISubmit")]
        public const int UISubmit = 14;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Interface Controls", friendlyName = "UICancel")]
        public const int UICancel = 15;
        [Rewired.Dev.ActionIdFieldInfo(categoryName = "Interface Controls", friendlyName = "Secondary Click")]
        public const int UISecondaryAction = 16;
    }
    public static partial class Category {
        public const int Default = 0;
        public const int UI = 2;
    }
    public static partial class Layout {
        public static partial class Joystick {
            public const int Default = 0;
            public const int UI = 1;
        }
        public static partial class Keyboard {
            public const int Default = 0;
        }
        public static partial class Mouse {
            public const int Default = 0;
        }
        public static partial class CustomController {
            public const int Default = 0;
        }
    }
    public static partial class Player {
        [Rewired.Dev.PlayerIdFieldInfo(friendlyName = "System")]
        public const int System = 9999999;
        [Rewired.Dev.PlayerIdFieldInfo(friendlyName = "Player0")]
        public const int Player0 = 0;
    }
    public static partial class CustomController {
    }
    public static partial class LayoutManagerRuleSet {
    }
    public static partial class MapEnablerRuleSet {
    }
}
