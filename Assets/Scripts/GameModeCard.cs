using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;

/// <summary>
/// Represents a selectable game mode card in the menu
/// Handles hover effects, click events, and displays game mode information
/// </summary>
public class GameModeCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("UI References")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image iconImage;
    [SerializeField] private GameObject selectionHighlight;

    [Header("Visual Settings")]
    [SerializeField] private Color normalColor = new Color(0.2f, 0.2f, 0.25f, 0.9f);
    [SerializeField] private Color hoverColor = new Color(0.3f, 0.3f, 0.4f, 1f);
    [SerializeField] private Color selectedColor = new Color(0.4f, 0.6f, 0.8f, 1f);
    [SerializeField] private float hoverScale = 1.05f;
    [SerializeField] private float animationSpeed = 5f;

    [Header("Card Data")]
    [SerializeField] private GameMode gameMode;
    [SerializeField] private string cardTitle;
    [SerializeField] private string cardDescription;

    // Events
    public event Action<GameMode> OnCardSelected;

    private Vector3 originalScale;
    private Vector3 targetScale;
    private Color targetColor;
    private bool isHovered;
    private bool isSelected;

    private void Awake()
    {
        originalScale = transform.localScale;
        targetScale = originalScale;
        targetColor = normalColor;

        if (selectionHighlight != null)
            selectionHighlight.SetActive(false);

        if (backgroundImage != null)
            backgroundImage.color = normalColor;
    }

    private void Update()
    {
        // Smooth scale animation
        if (transform.localScale != targetScale)
        {
            transform.localScale = Vector3.Lerp(
                transform.localScale,
                targetScale,
                Time.deltaTime * animationSpeed
            );
        }

        // Smooth color animation
        if (backgroundImage != null && backgroundImage.color != targetColor)
        {
            backgroundImage.color = Color.Lerp(
                backgroundImage.color,
                targetColor,
                Time.deltaTime * animationSpeed
            );
        }
    }

    /// <summary>
    /// Initialize the card with game mode data
    /// </summary>
    public void Initialize(string title, string description, GameMode mode)
    {
        cardTitle = title;
        cardDescription = description;
        gameMode = mode;

        UpdateCardDisplay();
    }

    private void UpdateCardDisplay()
    {
        if (titleText != null)
            titleText.text = cardTitle;

        if (descriptionText != null)
            descriptionText.text = cardDescription;

        // Set icon based on game mode
        if (iconImage != null)
        {
            // You can assign specific sprites for each mode in the inspector
            // or load them dynamically
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isSelected) return;

        isHovered = true;
        targetScale = originalScale * hoverScale;
        targetColor = hoverColor;

        // Play hover sound effect here if available
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isSelected) return;

        isHovered = false;
        targetScale = originalScale;
        targetColor = normalColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SelectCard();
    }

    /// <summary>
    /// Select this card and trigger the game mode
    /// </summary>
    public void SelectCard()
    {
        isSelected = true;
        targetScale = originalScale;
        targetColor = selectedColor;

        if (selectionHighlight != null)
            selectionHighlight.SetActive(true);

        // Play selection sound effect here if available

        // Invoke the event to notify listeners
        OnCardSelected?.Invoke(gameMode);
    }

    /// <summary>
    /// Deselect this card
    /// </summary>
    public void DeselectCard()
    {
        isSelected = false;
        targetScale = originalScale;
        targetColor = normalColor;

        if (selectionHighlight != null)
            selectionHighlight.SetActive(false);
    }

    /// <summary>
    /// Set custom colors for the card
    /// </summary>
    public void SetColors(Color normal, Color hover, Color selected)
    {
        normalColor = normal;
        hoverColor = hover;
        selectedColor = selected;

        if (!isHovered && !isSelected)
            targetColor = normalColor;
    }

    /// <summary>
    /// Get the current game mode of this card
    /// </summary>
    public GameMode GetGameMode()
    {
        return gameMode;
    }
}
