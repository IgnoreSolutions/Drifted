using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

using Drifted.UI;
using Drifted.UI.WindowManager;

public class NewEditorScript : ScriptableObject
{
    private static GameObject MakeWindowTitle(MikeWindowManager manager, GameObject parent, string title, Color textColor, Vector2 windowSize, bool needsCloseButton)
    {
        GameObject titleText = new GameObject(title);
        var horzLayout = titleText.AddComponent<HorizontalLayoutGroup>();
        titleText.AddComponent<LayoutElement>();

        Text actualText = titleText.AddComponent<Text>();
        actualText.font = MikeWindowManager.DefaultFont;
        actualText.name = "Window Title";
        actualText.alignment = TextAnchor.UpperCenter;
        actualText.text = title;
        actualText.transform.SetParent(titleText.transform);
        actualText.transform.localPosition = Vector3.zero;
        actualText.color = textColor;

        titleText.transform.SetParent(parent.transform);

        var titleTextGoObjRect = titleText.GetComponent<RectTransform>();
        var newTextPosition = new Vector3(
            -((windowSize.x / 2) - (titleTextGoObjRect.rect.width / 2)),
            ((windowSize.y / 2) - titleTextGoObjRect.rect.height / 2),
            0
        );
        titleText.transform.localPosition = newTextPosition;

        if (needsCloseButton)
        {
            var closeButtonGO = new GameObject();
            closeButtonGO.name = "Close Button";
            Text closeButton = closeButtonGO.AddComponent<Text>();
            closeButtonGO.AddComponent<LayoutElement>();
            closeButton.font = actualText.font;
            closeButton.text = "x";
            closeButton.color = textColor;
            var closeController = closeButton.transform.gameObject.AddComponent<MikeCloseButtonController>();
            closeController.DestroyOnClose = false;
            closeController.WindowName = parent.name;
            closeController.WindowManager = manager;
            closeController.CloseButtonText = closeButton;
            closeButtonGO.transform.SetParent(titleText.transform);
            var closeButtonTransform = closeButtonGO.GetComponent<RectTransform>();
            //closeButtonTransform.sizeDelta = new Vector2(32f, 18f);
            closeButtonTransform.anchorMax = new Vector2(1.0f, 0.0f);
        }
        horzLayout.padding.left = (int)(windowSize.x - 32);

        return titleText;
    }

    [UnityEditor.MenuItem("Tools/Mike's Window Manager/Windowize!")]
    static void DoIt()
    {
        EditorUtility.DisplayDialog("MyTool", "Do It in C# !", "OK", "");

        GameObject selectedGameObject = Selection.activeGameObject;
        Canvas masterCanvas = selectedGameObject.transform.parent.gameObject.GetComponent<Canvas>();
        if (masterCanvas != null) // Is a part of a canvas
        {
            Image bgImage = selectedGameObject.GetComponent<Image>();
            if (bgImage == null) { EditorUtility.DisplayDialog("Error", "This selected game object can't be a window, it doesn't have an Image component attached to it!", "Bruh"); return; }

            MikeWindowManager WM = selectedGameObject.transform.parent.gameObject.GetComponent<MikeWindowManager>();
            if(WM == null) { EditorUtility.DisplayDialog("Error", "This GameObject's parent doesn't have a WindowManager attached to it!", "Pass the bong then."); return; }

            string title = selectedGameObject.name;

            Vector2 windowSize = selectedGameObject.GetComponent<RectTransform>().sizeDelta;

            // Here, we are assuming that someone has already designed a window how they like and now they just want to make sure it's honored by the Window Manager.
            WindowProperties newWindowProperties = selectedGameObject.AddComponent<WindowProperties>();

            newWindowProperties.WindowColor = Color.black;
            newWindowProperties.TextColor = Color.white;
            bgImage.color = newWindowProperties.WindowColor;

            VerticalLayoutGroup vertLayoutGroup = selectedGameObject.AddComponent<VerticalLayoutGroup>();

            GameObject newTitle = MakeWindowTitle(WM, selectedGameObject, title, newWindowProperties.TextColor, windowSize, true);
            newTitle.transform.SetParent(selectedGameObject.transform);
        }
    }
}
