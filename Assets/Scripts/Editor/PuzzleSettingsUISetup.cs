using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

#if UNITY_EDITOR
public class PuzzleSettingsUISetup : EditorWindow
{
    [MenuItem("Tools/Setup Puzzle Settings UI")]
    public static void ShowWindow()
    {
        GetWindow<PuzzleSettingsUISetup>("Puzzle Settings Setup");
    }

    private void OnGUI()
    {
        GUILayout.Label("Puzzle Settings UI Setup", EditorStyles.boldLabel);
        GUILayout.Space(10);

        GUILayout.Label("This will create a basic UI structure for puzzle filtering.", EditorStyles.wordWrappedLabel);
        GUILayout.Space(10);

        if (GUILayout.Button("Create Settings UI", GUILayout.Height(40)))
        {
            CreateSettingsUI();
        }

        GUILayout.Space(10);
        GUILayout.Label("Note: You'll still need to:", EditorStyles.wordWrappedLabel);
        GUILayout.Label("1. Position and style the UI elements", EditorStyles.wordWrappedLabel);
        GUILayout.Label("2. Assign references in PuzzleSettingsManager", EditorStyles.wordWrappedLabel);
        GUILayout.Label("3. Connect button events", EditorStyles.wordWrappedLabel);
    }

    private void CreateSettingsUI()
    {
        // Find or create Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("Canvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }

        // Create Settings Panel
        GameObject panelObj = new GameObject("PuzzleSettingsPanel");
        panelObj.transform.SetParent(canvas.transform, false);

        RectTransform panelRect = panelObj.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.2f, 0.2f);
        panelRect.anchorMax = new Vector2(0.8f, 0.8f);
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        Image panelImage = panelObj.AddComponent<Image>();
        panelImage.color = new Color(0.2f, 0.2f, 0.2f, 0.95f);

        // Create Title
        GameObject titleObj = new GameObject("TitleText");
        titleObj.transform.SetParent(panelObj.transform, false);
        RectTransform titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.1f, 0.9f);
        titleRect.anchorMax = new Vector2(0.9f, 0.98f);
        titleRect.offsetMin = Vector2.zero;
        titleRect.offsetMax = Vector2.zero;

        TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.text = "Puzzle Filter Settings";
        titleText.fontSize = 24;
        titleText.alignment = TextAlignmentOptions.Center;

        // Create Rating Section
        float yPos = 0.75f;
        CreateRatingSlider(panelObj, "MinRatingSlider", "Min Rating:", ref yPos);
        CreateRatingSlider(panelObj, "MaxRatingSlider", "Max Rating:", ref yPos);

        // Create Themes Section
        CreateScrollableToggles(panelObj, "ThemesScrollView", "Themes:", PuzzleThemes.PopularThemes.ToArray(), ref yPos);

        // Create Buttons
        CreateButton(panelObj, "ApplyButton", "Apply", 0.7f, 0.1f);
        CreateButton(panelObj, "CancelButton", "Cancel", 0.5f, 0.1f);
        CreateButton(panelObj, "ResetButton", "Reset", 0.3f, 0.1f);

        // Create Settings Manager
        GameObject managerObj = new GameObject("PuzzleSettingsManager");
        managerObj.AddComponent<PuzzleSettingsManager>();

        // Disable panel by default
        panelObj.SetActive(false);

        Debug.Log("Puzzle Settings UI created! Please assign references in PuzzleSettingsManager component.");
        Selection.activeGameObject = managerObj;
    }

    private void CreateRatingSlider(GameObject parent, string name, string label, ref float yPos)
    {
        GameObject sliderObj = new GameObject(name);
        sliderObj.transform.SetParent(parent.transform, false);

        RectTransform sliderRect = sliderObj.AddComponent<RectTransform>();
        sliderRect.anchorMin = new Vector2(0.3f, yPos - 0.08f);
        sliderRect.anchorMax = new Vector2(0.85f, yPos);
        sliderRect.offsetMin = Vector2.zero;
        sliderRect.offsetMax = Vector2.zero;

        Slider slider = sliderObj.AddComponent<Slider>();
        slider.minValue = 1;
        slider.maxValue = 3000;
        slider.wholeNumbers = true;
        slider.value = name.Contains("Min") ? 1 : 3000;

        // Create Background
        GameObject bg = new GameObject("Background");
        bg.transform.SetParent(sliderObj.transform, false);
        RectTransform bgRect = bg.AddComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        Image bgImage = bg.AddComponent<Image>();
        bgImage.color = new Color(0.1f, 0.1f, 0.1f);

        // Create Fill Area
        GameObject fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(sliderObj.transform, false);
        RectTransform fillRect = fillArea.AddComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;

        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(fillArea.transform, false);
        Image fillImage = fill.AddComponent<Image>();
        fillImage.color = new Color(0.3f, 0.6f, 0.9f);
        slider.fillRect = fill.GetComponent<RectTransform>();

        // Create Handle
        GameObject handleArea = new GameObject("Handle Slide Area");
        handleArea.transform.SetParent(sliderObj.transform, false);
        RectTransform handleAreaRect = handleArea.AddComponent<RectTransform>();
        handleAreaRect.anchorMin = Vector2.zero;
        handleAreaRect.anchorMax = Vector2.one;

        GameObject handle = new GameObject("Handle");
        handle.transform.SetParent(handleArea.transform, false);
        Image handleImage = handle.AddComponent<Image>();
        handleImage.color = Color.white;
        slider.handleRect = handle.GetComponent<RectTransform>();

        // Create Label
        GameObject labelObj = new GameObject(name + "Text");
        labelObj.transform.SetParent(parent.transform, false);
        RectTransform labelRect = labelObj.AddComponent<RectTransform>();
        labelRect.anchorMin = new Vector2(0.1f, yPos - 0.08f);
        labelRect.anchorMax = new Vector2(0.28f, yPos);
        TextMeshProUGUI labelText = labelObj.AddComponent<TextMeshProUGUI>();
        labelText.text = label;
        labelText.fontSize = 16;

        yPos -= 0.12f;
    }

    private void CreateScrollableToggles(GameObject parent, string name, string label, string[] items, ref float yPos)
    {
        // Create Label
        GameObject labelObj = new GameObject(label);
        labelObj.transform.SetParent(parent.transform, false);
        RectTransform labelRect = labelObj.AddComponent<RectTransform>();
        labelRect.anchorMin = new Vector2(0.1f, yPos - 0.05f);
        labelRect.anchorMax = new Vector2(0.9f, yPos);
        TextMeshProUGUI labelText = labelObj.AddComponent<TextMeshProUGUI>();
        labelText.text = label;
        labelText.fontSize = 18;

        yPos -= 0.08f;

        // Create Scroll View (simplified - just content area for now)
        GameObject scrollObj = new GameObject(name);
        scrollObj.transform.SetParent(parent.transform, false);
        RectTransform scrollRect = scrollObj.AddComponent<RectTransform>();
        scrollRect.anchorMin = new Vector2(0.1f, yPos - 0.25f);
        scrollRect.anchorMax = new Vector2(0.9f, yPos);
        scrollRect.offsetMin = Vector2.zero;
        scrollRect.offsetMax = Vector2.zero;

        Image scrollBg = scrollObj.AddComponent<Image>();
        scrollBg.color = new Color(0.15f, 0.15f, 0.15f);

        yPos -= 0.28f;
    }

    private void CreateButton(GameObject parent, string name, string text, float xCenter, float yPos)
    {
        GameObject buttonObj = new GameObject(name);
        buttonObj.transform.SetParent(parent.transform, false);

        RectTransform buttonRect = buttonObj.AddComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(xCenter - 0.08f, yPos);
        buttonRect.anchorMax = new Vector2(xCenter + 0.08f, yPos + 0.06f);
        buttonRect.offsetMin = Vector2.zero;
        buttonRect.offsetMax = Vector2.zero;

        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = new Color(0.3f, 0.5f, 0.8f);

        Button button = buttonObj.AddComponent<Button>();
        button.targetGraphic = buttonImage;

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;

        TextMeshProUGUI buttonText = textObj.AddComponent<TextMeshProUGUI>();
        buttonText.text = text;
        buttonText.fontSize = 16;
        buttonText.alignment = TextAlignmentOptions.Center;
    }
}
#endif
