/**

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


using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Drifted.CustomInput;
using Drifted.Extras;
using Rewired;
using UnityEngine;

struct AxisDownLastFrame
{
    public string axisName;
    public float direction;

    public AxisDownLastFrame(float dir, string name = "<None>")
    {
        axisName = name;
        direction = dir;
    }
}

namespace Drifted
{
    public enum DriftedButtons
    {
        MovementX,
        MovementY,
        CameraPanX,
        CameraPanY,
        Jump,
        PanCameraLeft,
        PanCameraRight,
        ResetCamera,
        ToggleInventory,
        ToggleMoods,
        ToggleSkills,
        DpadUp,
        DpadDown,
        DpadRight,
        DpadLeft,
        OK,
        Cancel,
        _TOTAL
    }

    public static class InputExtensions
    {
        public static AbstractInputDefinition ReadInputDefinitionFromFile(string path)
        {
            // TODO: format the input files better
            // I think they should be allowed to be modified by humans.a
            IFormatter formatter = new BinaryFormatter();
            using (Stream stream = new FileStream(@"input.binding", FileMode.Open, FileAccess.Read)) return (AbstractInputDefinition)formatter.Deserialize(stream);
        }

        public static void WriteInputDefinitionToFile(this AbstractInputDefinition inputDefinition, string path)
        {
            IFormatter formatter = new BinaryFormatter();
            using (Stream stream = new FileStream(@"input.binding", FileMode.Create, FileAccess.Write)) formatter.Serialize(stream, inputDefinition);
        }
    }

    public static class DriftedInputExtensions
    {
        public static DriftedInput ReadInputFromFile(string path)
        {
            using (StreamReader sr = new StreamReader(path))
            {
                DriftedInput returnValue = new DriftedInput();
                returnValue.Platform = PlatformUtilities.GetCurrentPlatform();
                string header = sr.ReadLine();
                if (header == "drifted_input")
                {
                    sr.ReadLine(); // Skip platform
                    bool isController = bool.Parse(sr.ReadLine());
                    int inputCounts = int.Parse(sr.ReadLine());
                    for(int i = 0; i < inputCounts; i++)
                    {
                        string inputStr = sr.ReadLine();
                        string[] inputSplit = inputStr.Split(':');
                        if(inputSplit.Length == 3)
                        {
                            string inputName, inputValue;
                            KeyType inputStyle;
                            inputName = inputSplit[0];
                            Enum.TryParse<KeyType>(inputSplit[1], out inputStyle);
                            inputValue = inputSplit[2];
                            if (inputStyle == KeyType.Axis) returnValue.AddInput(inputName, new ButtonOrAxis(inputValue));
                            else if (inputStyle == KeyType.Button)
                            {
                                KeyCode parsedCode;
                                Enum.TryParse<KeyCode>(inputValue, out parsedCode);
                                returnValue.AddInput(inputName, new ButtonOrAxis(parsedCode));
                            }
                            else Debug.Log($"Unhandled read case: {inputName}:{inputStyle}:{inputValue}");

                            //Debug.Log($"Parsed Input: {inputName} {inputValue} ({inputStyle})");
                        }
                    }
                    return returnValue;
                }
                else Debug.Log("Header mismatch.");
            }
            return null;
        }

        public static void WriteInputToFile(this DriftedInput dict, string path)
        {
            using (StreamWriter sw = new StreamWriter(path))
            {
                // Write header.
                sw.WriteLine("drifted_input");
                sw.WriteLine(dict.Platform);
                sw.WriteLine(dict.IsController);
                // Write Dictionary
                // Count
                sw.WriteLine(dict.InputDictionary.Count);
                // Keys
                foreach(var kvp in dict.InputDictionary)
                {
                    if (kvp.Value.InputStyle == KeyType.Button) sw.WriteLine($"{kvp.Key}:{kvp.Value.InputStyle}:{kvp.Value.KeyCode}");
                    else if (kvp.Value.InputStyle == KeyType.Axis) sw.WriteLine($"{kvp.Key}:{kvp.Value.InputStyle}:{kvp.Value.AxisName}");
                    else Debug.Log($"Unhandled: {kvp.Key}, {kvp.Value.InputStyle}, {kvp.Value.KeyCode}/{kvp.Value.AxisName}");
                }
            }
        }
    }

    public class DriftedInput : AbstractInputDictionary<string>
    {
        public override bool IsController { get; set; } = true;
        public override string DefinitionName { get; set; } = "Default PC";
        public override Platform Platform { get; set; } = Platform.macOS;

        public DriftedInput()
        {
            AddInput("OK", new ButtonOrAxis(KeyCode.Return));
            AddInput("Cancel", new ButtonOrAxis(KeyCode.Escape));
            AddInput("CameraPanX", new ButtonOrAxis("Mouse X"));
            AddInput("CameraPanY", new ButtonOrAxis("Mouse Y"));
            AddInput("ResetCamera", new ButtonOrAxis(KeyCode.R));
            AddInput("Jump", new ButtonOrAxis(KeyCode.Space));
            AddInput("MovementX", new ButtonOrAxis("Horizontal"));
            AddInput("MovementY", new ButtonOrAxis("Vertical"));

            AddInput("ToggleMoods", new ButtonOrAxis(KeyCode.M));
            AddInput("ToggleSkills", new ButtonOrAxis(KeyCode.O));
            AddInput("ToggleInventory", new ButtonOrAxis(KeyCode.I));

            AddInput("PanCameraLeft", new ButtonOrAxis(KeyCode.Q));
            AddInput("PanCameraRight", new ButtonOrAxis(KeyCode.E));
        }
    }

    [Serializable]
    public abstract class AbstractInputDictionary<T>
    {
        internal Dictionary<T, AbstractInput> InputDictionary = new Dictionary<T, AbstractInput>();

        public abstract bool IsController {get;set;}
        public abstract string DefinitionName {get;set;}
        public abstract Platform Platform {get;set;}

        public AbstractInputDictionary()
        {
            if (typeof(T) == typeof(Enum))
            {
                InputDictionary = new Dictionary<T, AbstractInput>(Enum.GetNames(typeof(T)).Length);
            }
            else InputDictionary = new Dictionary<T, AbstractInput>();
        }

        /// <summary>
        /// Add or replace an input.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <param name="input">Input.</param>
        internal void AddInput(T type, AbstractInput input)
        {
            if (InputDictionary.ContainsKey(type)) InputDictionary[type] = input;
            else InputDictionary.Add(type, input);
        }

        private AbstractInput GetInput(T input)
        {
            if(InputDictionary.ContainsKey(input))
            {
                return InputDictionary[input];
            }
            else return null;
        }

        public bool GetKeyDown(T button)
        {
            AbstractInput input = GetInput(button);
            if(input == null) return false;
            return UnityEngine.Input.GetKeyDown(input.KeyCode);
        }

        public bool GetKey(T button)
        {
            AbstractInput input = GetInput(button);
            if(input == null) return false;
            return UnityEngine.Input.GetKey(input.KeyCode);
        }

        public bool GetKeyUp(T button)
        {
            AbstractInput input = GetInput(button);
            if(input == null) return false;
            return UnityEngine.Input.GetKeyUp(input.KeyCode);
        }

        public float GetAxis(T axis)
        {
            AbstractInput input = GetInput(axis);
            if(input == null) return 0f;
            return UnityEngine.Input.GetAxis(input.AxisName);
        }
    }

    [Serializable]
    public abstract class AbstractInputDefinition
    {
        public abstract string DefinitionName { get; set; }
        public abstract Platform AppropriatePlatform { get; set; }

        public virtual string MovementAxisX { get; set; } = "MoveHorizontal";
        public virtual string MovementAxisY { get; set; } = "MoveVertical";

        public virtual string CameraPanX { get; set; } = "CameraHorizontal";
        public virtual string CameraPanY { get; set; } = "CameraVertical";

        public virtual string Jump { get; set; } = "Jump";
        public virtual string PanCameraLeft { get; set; } = "PanCameraLeft";
        public virtual string PanCameraRight { get; set; } = "PanCameraRight";
        public virtual string ResetCamera { get; set; } = "ResetCamera";
        public virtual string ToggleInventory { get; set; } = "ToggleInventory";
        public virtual string ToggleMoods { get; set; } = "ToggleMoods";
        public virtual string ToggleSkills { get; set; } = "ToggleSkills";

        public virtual string DpadUp { get; set; } = "";
        public virtual string DpadLeft { get; set; } = "";
        public virtual string DpadDown { get; set; } = "";
        public virtual string DpadRight { get; set; } = "";

        public virtual string OK { get; set; } = "";

        public virtual string Cancel { get; set; } = "";

        public virtual string MoveSpeedModifier { get; set; } = "";

        public abstract bool IsController { get; set; }

        public virtual string GetInputFromEnum(DriftedButtons button)
        {
            switch (button)
            {
                case DriftedButtons.MovementX: return MovementAxisX;
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
        }
    }

    [Serializable]
    public class DefaultKeyboard : AbstractInputDefinition
    {
        public override string DefinitionName { get; set; } = "Default PC";
        public override Platform AppropriatePlatform { get; set; } = Platform.macOS | Platform.Windows | Platform.Linux;
        public override bool IsController { get; set; } = false;
    }

    public static class CustomInputManager
    {
        private static bool initDone = false;
        public static bool IsController 
        { 
            get 
            {
                if (playerInstance != null) return playerInstance.controllers.joystickCount > 0;
                return false;
            } 
        }

        private static AbstractInputDefinition CurrentMapping = new DefaultKeyboard(); // The default PC controls

        public static AbstractInputDefinition GetCurrentMapping() => CurrentMapping;

        private static Rewired.Player playerInstance;
        public static int rewiredPlayerID = 0;

        private static void TestNew()
        {
            //Debug.Log("New input write test.");
            //
            //DriftedInput test2 = DriftedInputExtensions.ReadInputFromFile("new_input.binding");
        }

        public static void TryAutoInput()
        {
            try
            {
                playerInstance = ReInput.players.GetPlayer(rewiredPlayerID);
            }
#pragma warning disable RECS0022 // A catch clause that catches System.Exception and has an empty body
            catch { }
#pragma warning restore RECS0022 // A catch clause that catches System.Exception and has an empty body

            // Just so we can get a baseline...
            DebugControllers();

            //TestNew();

            Platform currentPlatform = PlatformUtilities.GetCurrentPlatform();
            Debug.Log("Current Platform: " + currentPlatform.ToString());

            if(UnityEngine.Application.isEditor && !Application.isPlaying)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }

        private static bool MappingsExist() => File.Exists(@"input.binding");

        private static int ControllersConnected()
        {
            string[] names = UnityEngine.Input.GetJoystickNames();
            if (names.Length == 1) if (String.IsNullOrEmpty(names[0])) return 0;
            return names.Length;
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

        private static KeyCode GetPressedKeyCode()
        {
            foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
            {
                if (UnityEngine.Input.GetKey(kcode)) return kcode;
            }

            return KeyCode.None;
        }

        private static bool dpadup = false, dpaddown = false, dpadright = false, dpadleft = false;
        private static float lastdpadx, lastdpady;

        public enum Dpad
        {
            Up, Down, Right, Left, None
        }
        [Obsolete]
        private static Dpad GetDpadValue()
        {
            return Dpad.None;
        }
        [Obsolete]
        private static bool DpadButtonDown(string input)
        {
            return KeyDown(input);
        }

        [Obsolete]
        public static bool KeyUp(string input) => false;
        [Obsolete]
        public static bool KeyDown(string input) => false;
        [Obsolete]
        public static bool GetKey(string input) => false;
        [Obsolete]
        public static float GetAxis(string axis) => 0f;

        public static void SetUIActive()
        {
            SetControllerCategory(true, "UI");
            SetControllerCategory(false, "Default");
        }

        public static void SetPlayActive()
        {
            SetControllerCategory(false, "UI");
            SetControllerCategory(true, "Default");
        }

        public static void SetControllerCategory(bool state, string categoryName)
        {
            playerInstance.controllers.maps.SetMapsEnabled(state, categoryName);
        }

    }
}
