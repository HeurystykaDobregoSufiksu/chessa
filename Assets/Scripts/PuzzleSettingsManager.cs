using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PuzzleSettingsManager : MonoBehaviour
{
    [Header("Settings Panel")]
    public GameObject settingsPanel;

    [Header("Rating Controls")]
    public Slider minRatingSlider;
    public Slider maxRatingSlider;
    public TextMeshProUGUI minRatingText;
    public TextMeshProUGUI maxRatingText;

    [Header("Theme Toggles")]
    public Toggle[] themeToggles;
    public string[] themeNames;

    [Header("Opening Toggles")]
    public Toggle[] openingToggles;
    public string[] openingNames;

    [Header("References")]
    public gameManager gameManager;

    private PuzzleSettings settings;

    void Start()
    {
        settings = new PuzzleSettings();
        LoadSettings();
        UpdateUI();

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }

    public void OpenSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
            UpdateUI();
        }
    }

    public void CloseSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }

    public void ApplySettings()
    {
        // Update settings from UI
        settings.MinRating = (int)minRatingSlider.value;
        settings.MaxRating = (int)maxRatingSlider.value;

        // Collect selected themes
        settings.SelectedThemes.Clear();
        for (int i = 0; i < themeToggles.Length && i < themeNames.Length; i++)
        {
            if (themeToggles[i] != null && themeToggles[i].isOn)
            {
                settings.SelectedThemes.Add(themeNames[i]);
            }
        }

        // Collect selected openings
        settings.SelectedOpenings.Clear();
        for (int i = 0; i < openingToggles.Length && i < openingNames.Length; i++)
        {
            if (openingToggles[i] != null && openingToggles[i].isOn)
            {
                settings.SelectedOpenings.Add(openingNames[i]);
            }
        }

        SaveSettings();

        // Reload puzzles with new filters
        if (gameManager != null)
        {
            gameManager.ReloadPuzzles(settings);
        }

        CloseSettings();
    }

    public void ResetSettings()
    {
        settings.Reset();
        SaveSettings();
        UpdateUI();
    }

    public void UpdateRatingText()
    {
        if (minRatingText != null)
        {
            minRatingText.text = "Min: " + (int)minRatingSlider.value;
        }

        if (maxRatingText != null)
        {
            maxRatingText.text = "Max: " + (int)maxRatingSlider.value;
        }

        // Ensure min is not greater than max
        if (minRatingSlider.value > maxRatingSlider.value)
        {
            maxRatingSlider.value = minRatingSlider.value;
        }
    }

    private void UpdateUI()
    {
        if (minRatingSlider != null)
        {
            minRatingSlider.value = settings.MinRating;
        }

        if (maxRatingSlider != null)
        {
            maxRatingSlider.value = settings.MaxRating;
        }

        UpdateRatingText();

        // Update theme toggles
        for (int i = 0; i < themeToggles.Length && i < themeNames.Length; i++)
        {
            if (themeToggles[i] != null)
            {
                themeToggles[i].isOn = settings.SelectedThemes.Contains(themeNames[i]);
            }
        }

        // Update opening toggles
        for (int i = 0; i < openingToggles.Length && i < openingNames.Length; i++)
        {
            if (openingToggles[i] != null)
            {
                openingToggles[i].isOn = settings.SelectedOpenings.Contains(openingNames[i]);
            }
        }
    }

    private void SaveSettings()
    {
        PlayerPrefs.SetInt("PuzzleMinRating", settings.MinRating);
        PlayerPrefs.SetInt("PuzzleMaxRating", settings.MaxRating);
        PlayerPrefs.SetString("PuzzleThemes", string.Join(",", settings.SelectedThemes));
        PlayerPrefs.SetString("PuzzleOpenings", string.Join(",", settings.SelectedOpenings));
        PlayerPrefs.Save();
    }

    private void LoadSettings()
    {
        settings.MinRating = PlayerPrefs.GetInt("PuzzleMinRating", 1);
        settings.MaxRating = PlayerPrefs.GetInt("PuzzleMaxRating", 3000);

        string themesStr = PlayerPrefs.GetString("PuzzleThemes", "");
        if (!string.IsNullOrEmpty(themesStr))
        {
            settings.SelectedThemes = new List<string>(themesStr.Split(','));
        }

        string openingsStr = PlayerPrefs.GetString("PuzzleOpenings", "");
        if (!string.IsNullOrEmpty(openingsStr))
        {
            settings.SelectedOpenings = new List<string>(openingsStr.Split(','));
        }
    }

    public PuzzleSettings GetSettings()
    {
        return settings;
    }
}
