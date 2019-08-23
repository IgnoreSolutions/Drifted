/// Copyright 2019 Mike Santiago (axiom@ignoresolutions.xyz)

using System.Collections;
using System.Collections.Generic;
using Rewired;
using UnityEngine;

namespace Drifted.Input
{
    public static class DriftedInputManager
    {
        private static Rewired.Player playerInstance;

        public static int rewiredPlayerID = 0;

        public static bool mIsController = false;

        private static int ControllerCount = 0;

        public static bool IsController
        {
            get
            {
                if (playerInstance != null) return ControllerCount > 0;
                return false;
            }
        }

        public static ControllerType LastInputType()
        {
            return ReInput.controllers.GetLastActiveController().type;
        }

        private static List<string> UniqueInputsAskedFor = new List<string>();
        private static bool debug = false;
        public static void SetupInputs(bool debug = false)
        {
            if (playerInstance == null && Application.isPlaying)
            {
                //playerInstance = ReInput.players.GetPlayer(rewiredPlayerID);
                playerInstance = ReInput.players.GetPlayer(rewiredPlayerID);
                ControllerCount = playerInstance.controllers.joystickCount;
            }
            if(debug)
            {
                Debug.Log("Debugging DriftedInputManager");
                DriftedInputManager.debug = true;
            }
        }

        private static void CheckDebug(string action)
        {
            if(debug)
            {
                if(!UniqueInputsAskedFor.Contains(action))
                {
                    UniqueInputsAskedFor.Add(action);
                    Debug.Log("Unique Action: " + action);
                }
            }
        }

        public static void DebugControllers()
        {
            string[] names = UnityEngine.Input.GetJoystickNames();
            Debug.Log("Controllers Connected: " + names.Length);
            if (names.Length > 0)
            {
                for (int i = 0; i < names.Length; i++)
                {
                    if (!string.IsNullOrEmpty(names[i])) Debug.Log(names[i]);
                }
            }
        }

        public static string KeyNameFromEnum(DriftedButtons button)
        {
            return "no";
            /*
            switch (button)
            {
                case DriftedButtons.MovementX: return Inputs;
                case DriftedButtons.MovementY: return MovementAxisY;
                case DriftedButtons.CameraPanX: return CameraPanX;
                case DriftedButtons.CameraPanY: return CameraPanY;
                case DriftedButtons.Jump: return Jump;
                case DriftedButtons.PanCameraLeft: return PanCameraLeft;
                case DriftedButtons.PanCameraRight: return PanCameraRight;
                case DriftedButtons.ResetCamera: return ResetCamera;
                case DriftedButtons.ToggleInventory: return ToggleInventory;
                case DriftedButtons.ToggleMoods: return ToggleMoods;
                case DriftedButtons.ToggleSkills: return ToggleSkills;
                case DriftedButtons.DpadUp: return DpadUp;
                case DriftedButtons.DpadLeft: return DpadLeft;
                case DriftedButtons.DpadRight: return DpadRight;
                case DriftedButtons.DpadDown: return DpadDown;
                case DriftedButtons.OK: return OK;
                case DriftedButtons.Cancel: return Cancel;
            }

            return OK;
            */           
        }

        public static bool KeyUp(string input)
        {
            if(debug) CheckDebug(input);
            if (playerInstance == null) return false;
            return playerInstance.GetButtonUp(input);
        }

        public static bool KeyDown(string input)
        {
            if(debug) CheckDebug(input);
            if (playerInstance == null) return false;
            return playerInstance.GetButtonDown(input);
        }
        public static bool GetKey(string input)
        {
            if(debug) CheckDebug(input);
            if (playerInstance == null) return false;
            return playerInstance.GetButton(input);
        }
        public static float GetAxis(string axis)
        {
            if(debug) CheckDebug(axis);
            if (playerInstance == null) return 0f;
            return playerInstance.GetAxis(axis);
        }

        public static void SetUIActive()
        {
            if(playerInstance == null) {Debug.Log("No instance"); return; }
            playerInstance.controllers.maps.SetAllMapsEnabled(false); // Disable all maps

            // Enable UI maps
            playerInstance.controllers.maps.SetMapsEnabled(true, "UI");
        }

        public static void SetPlayActive()
        {
            Debug.Log("Set play active");
            if(playerInstance == null) {Debug.Log("No instance"); return; }
            playerInstance.controllers.maps.SetAllMapsEnabled(false); // Disable all maps

            // Enable default maps
            playerInstance.controllers.maps.SetMapsEnabled(true, "Default");
        }
    }
}
