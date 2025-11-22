using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manages modern UI styling and theming for the menu system
/// Provides consistent color schemes and visual effects
/// </summary>
public class UIStyleManager : MonoBehaviour
{
    [Header("Color Scheme")]
    [SerializeField] private Color primaryColor = new Color(0.2f, 0.4f, 0.8f, 1f);
    [SerializeField] private Color secondaryColor = new Color(0.15f, 0.15f, 0.2f, 1f);
    [SerializeField] private Color accentColor = new Color(0.3f, 0.7f, 0.9f, 1f);
    [SerializeField] private Color backgroundColor = new Color(0.05f, 0.05f, 0.1f, 1f);
    [SerializeField] private Color textColor = new Color(0.9f, 0.9f, 0.95f, 1f);

    [Header("Auto-Style Elements")]
    [SerializeField] private bool autoStyleOnStart = true;
    [SerializeField] private GameObject rootUIObject;

    private static UIStyleManager instance;
    public static UIStyleManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<UIStyleManager>();
            return instance;
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void Start()
    {
        if (autoStyleOnStart && rootUIObject != null)
        {
            StyleAllElements(rootUIObject);
        }
    }

    /// <summary>
    /// Apply modern styling to all UI elements under the root object
    /// </summary>
    public void StyleAllElements(GameObject root)
    {
        if (root == null) return;

        // Style all buttons
        Button[] buttons = root.GetComponentsInChildren<Button>(true);
        foreach (Button button in buttons)
        {
            StyleButton(button);
        }

        // Style all images
        Image[] images = root.GetComponentsInChildren<Image>(true);
        foreach (Image image in images)
        {
            if (image.GetComponent<Button>() == null) // Don't style button backgrounds twice
            {
                StyleImage(image);
            }
        }

        // Style all text elements
        TMP_Text[] texts = root.GetComponentsInChildren<TMP_Text>(true);
        foreach (TMP_Text text in texts)
        {
            StyleText(text);
        }
    }

    /// <summary>
    /// Style a button with modern colors and effects
    /// </summary>
    public void StyleButton(Button button)
    {
        if (button == null) return;

        Image buttonImage = button.GetComponent<Image>();
        if (buttonImage != null)
        {
            buttonImage.color = primaryColor;
        }

        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(1.2f, 1.2f, 1.2f, 1f);
        colors.pressedColor = new Color(0.8f, 0.8f, 0.8f, 1f);
        colors.selectedColor = accentColor;
        colors.disabledColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        colors.colorMultiplier = 1f;
        colors.fadeDuration = 0.1f;
        button.colors = colors;

        // Add shadow effect if Shadow component doesn't exist
        Shadow shadow = button.GetComponent<Shadow>();
        if (shadow == null)
        {
            shadow = button.gameObject.AddComponent<Shadow>();
            shadow.effectColor = new Color(0f, 0f, 0f, 0.5f);
            shadow.effectDistance = new Vector2(2f, -2f);
        }
    }

    /// <summary>
    /// Style an image with theme colors
    /// </summary>
    public void StyleImage(Image image)
    {
        if (image == null) return;

        // Check if this is a background or panel image
        if (image.name.ToLower().Contains("background") ||
            image.name.ToLower().Contains("panel"))
        {
            image.color = secondaryColor;
        }
    }

    /// <summary>
    /// Style text with consistent colors
    /// </summary>
    public void StyleText(TMP_Text text)
    {
        if (text == null) return;

        text.color = textColor;

        // Add outline for better readability
        if (!text.gameObject.GetComponent<Outline>())
        {
            Outline outline = text.gameObject.AddComponent<Outline>();
            outline.effectColor = new Color(0f, 0f, 0f, 0.5f);
            outline.effectDistance = new Vector2(1f, -1f);
        }
    }

    /// <summary>
    /// Create a gradient background for panels
    /// </summary>
    public void ApplyGradientBackground(Image image, Color topColor, Color bottomColor)
    {
        if (image == null) return;

        // This would require a custom shader or a gradient texture
        // For now, we'll use a simple averaged color
        Color averageColor = (topColor + bottomColor) / 2f;
        image.color = averageColor;
    }

    /// <summary>
    /// Get the current color scheme
    /// </summary>
    public ColorScheme GetColorScheme()
    {
        return new ColorScheme
        {
            Primary = primaryColor,
            Secondary = secondaryColor,
            Accent = accentColor,
            Background = backgroundColor,
            Text = textColor
        };
    }

    /// <summary>
    /// Set a new color scheme
    /// </summary>
    public void SetColorScheme(ColorScheme scheme)
    {
        primaryColor = scheme.Primary;
        secondaryColor = scheme.Secondary;
        accentColor = scheme.Accent;
        backgroundColor = scheme.Background;
        textColor = scheme.Text;

        // Re-style all elements if root is set
        if (rootUIObject != null)
        {
            StyleAllElements(rootUIObject);
        }
    }

    [System.Serializable]
    public struct ColorScheme
    {
        public Color Primary;
        public Color Secondary;
        public Color Accent;
        public Color Background;
        public Color Text;
    }
}
