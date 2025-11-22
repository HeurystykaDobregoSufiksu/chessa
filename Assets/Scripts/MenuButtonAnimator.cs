using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Adds smooth animations to menu buttons for a modern feel
/// Handles hover, click, and transition effects
/// </summary>
[RequireComponent(typeof(Button))]
public class MenuButtonAnimator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Scale Animation")]
    [SerializeField] private bool enableScaleAnimation = true;
    [SerializeField] private float hoverScale = 1.1f;
    [SerializeField] private float clickScale = 0.95f;
    [SerializeField] private float scaleSpeed = 10f;

    [Header("Rotation Animation")]
    [SerializeField] private bool enableRotationAnimation = false;
    [SerializeField] private float hoverRotation = 2f;

    [Header("Glow Effect")]
    [SerializeField] private bool enableGlow = true;
    [SerializeField] private Image glowImage;
    [SerializeField] private float glowIntensity = 0.5f;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip hoverSound;
    [SerializeField] private AudioClip clickSound;

    private Vector3 originalScale;
    private Vector3 targetScale;
    private Quaternion originalRotation;
    private Quaternion targetRotation;
    private Button button;
    private AudioSource audioSource;
    private bool isHovered;
    private bool isPressed;

    private void Awake()
    {
        button = GetComponent<Button>();
        originalScale = transform.localScale;
        targetScale = originalScale;
        originalRotation = transform.localRotation;
        targetRotation = originalRotation;

        // Set up audio source if sounds are provided
        if (hoverSound != null || clickSound != null)
        {
            audioSource = gameObject.GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
            }
        }

        // Set up glow effect
        if (enableGlow && glowImage != null)
        {
            Color glowColor = glowImage.color;
            glowColor.a = 0f;
            glowImage.color = glowColor;
        }
    }

    private void Update()
    {
        // Smooth scale animation
        if (enableScaleAnimation)
        {
            transform.localScale = Vector3.Lerp(
                transform.localScale,
                targetScale,
                Time.unscaledDeltaTime * scaleSpeed
            );
        }

        // Smooth rotation animation
        if (enableRotationAnimation)
        {
            transform.localRotation = Quaternion.Lerp(
                transform.localRotation,
                targetRotation,
                Time.unscaledDeltaTime * scaleSpeed
            );
        }

        // Glow effect animation
        if (enableGlow && glowImage != null)
        {
            Color currentColor = glowImage.color;
            float targetAlpha = isHovered ? glowIntensity : 0f;
            currentColor.a = Mathf.Lerp(currentColor.a, targetAlpha, Time.unscaledDeltaTime * scaleSpeed);
            glowImage.color = currentColor;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!button.interactable) return;

        isHovered = true;
        targetScale = originalScale * hoverScale;

        if (enableRotationAnimation)
        {
            targetRotation = originalRotation * Quaternion.Euler(0f, 0f, hoverRotation);
        }

        // Play hover sound
        if (hoverSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hoverSound);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        isPressed = false;
        targetScale = originalScale;
        targetRotation = originalRotation;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!button.interactable) return;

        isPressed = true;
        targetScale = originalScale * clickScale;

        // Play click sound
        if (clickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;

        if (isHovered)
        {
            targetScale = originalScale * hoverScale;
        }
        else
        {
            targetScale = originalScale;
        }
    }

    /// <summary>
    /// Reset the button to its original state
    /// </summary>
    public void ResetButton()
    {
        transform.localScale = originalScale;
        transform.localRotation = originalRotation;
        isHovered = false;
        isPressed = false;
        targetScale = originalScale;
        targetRotation = originalRotation;
    }

    /// <summary>
    /// Set custom hover scale
    /// </summary>
    public void SetHoverScale(float scale)
    {
        hoverScale = scale;
    }

    /// <summary>
    /// Set custom animation speed
    /// </summary>
    public void SetAnimationSpeed(float speed)
    {
        scaleSpeed = speed;
    }
}
