# Puzzle Settings UI Setup Guide

This guide explains how to set up the puzzle filtering settings menu in Unity Editor.

## Overview

The puzzle filtering system allows users to filter puzzles by:
- **Rating Range**: Min and max ELO rating (e.g., 1000-2000)
- **Themes**: Chess tactical themes (fork, pin, mate, etc.)
- **Openings**: Chess opening categories (Sicilian, French, etc.)

## Files Added/Modified

### New Files:
- `Assets/Models/PuzzleSettings.cs` - Data model for filter settings
- `Assets/Scripts/PuzzleSettingsManager.cs` - Manager for settings UI and persistence
- `Assets/Scripts/PuzzleThemes.cs` - List of common chess themes and openings

### Modified Files:
- `Assets/Scripts/DBService.cs` - Added SQL filtering logic
- `Assets/Scripts/gameManager.cs` - Added ReloadPuzzles() method

## Unity Editor Setup

### Step 1: Create Settings Panel UI

1. **Open the Scene**: Open `Assets/Scenes/SampleScene.unity`

2. **Create Settings Panel GameObject**:
   - In Hierarchy, right-click on `Canvas` (or create one if it doesn't exist)
   - Create UI > Panel → Rename to "PuzzleSettingsPanel"
   - Position it in the center of the screen
   - Set Panel background color/transparency as desired

3. **Add Settings Panel Header**:
   - Right-click PuzzleSettingsPanel → UI > Text - TextMeshPro
   - Rename to "TitleText"
   - Set text to "Puzzle Filter Settings"
   - Position at top of panel

### Step 2: Add Rating Sliders

1. **Min Rating Slider**:
   - Right-click PuzzleSettingsPanel → UI > Slider
   - Rename to "MinRatingSlider"
   - In Inspector:
     - Min Value: 1
     - Max Value: 3000
     - Whole Numbers: ✓
     - Value: 1

2. **Min Rating Label**:
   - Right-click PuzzleSettingsPanel → UI > Text - TextMeshPro
   - Rename to "MinRatingText"
   - Set text to "Min: 1"
   - Position above/beside MinRatingSlider

3. **Max Rating Slider**:
   - Right-click PuzzleSettingsPanel → UI > Slider
   - Rename to "MaxRatingSlider"
   - In Inspector:
     - Min Value: 1
     - Max Value: 3000
     - Whole Numbers: ✓
     - Value: 3000

4. **Max Rating Label**:
   - Right-click PuzzleSettingsPanel → UI > Text - TextMeshPro
   - Rename to "MaxRatingText"
   - Set text to "Max: 3000"
   - Position above/beside MaxRatingSlider

### Step 3: Add Theme Toggles

Create a scrollable area for theme toggles:

1. **Create Scroll View**:
   - Right-click PuzzleSettingsPanel → UI > Scroll View
   - Rename to "ThemesScrollView"
   - Position in middle section of panel

2. **Add Theme Toggle Group**:
   - Right-click on the "Content" object inside ThemesScrollView
   - Add UI > Toggle for each theme you want to support
   - Recommended themes (from `PuzzleThemes.PopularThemes`):
     - mate, mateIn1, mateIn2, mateIn3
     - fork, pin, skewer, discoveredAttack
     - sacrifice, endgame, middlegame, opening
     - backRankMate, promotion, deflection

3. **Configure Each Toggle**:
   - Rename each Toggle to match the theme (e.g., "Toggle_mate", "Toggle_fork")
   - Set Label text to human-readable name (e.g., "Checkmate", "Fork")

### Step 4: Add Opening Toggles (Optional)

If you want opening filters:

1. **Create Another Scroll View** (similar to themes):
   - Right-click PuzzleSettingsPanel → UI > Scroll View
   - Rename to "OpeningsScrollView"

2. **Add Opening Toggles** for common openings:
   - Sicilian, French, Italian, Spanish, etc.

### Step 5: Add Action Buttons

1. **Apply Button**:
   - Right-click PuzzleSettingsPanel → UI > Button - TextMeshPro
   - Rename to "ApplyButton"
   - Set Button text to "Apply"
   - Position at bottom-right of panel

2. **Cancel Button**:
   - Right-click PuzzleSettingsPanel → UI > Button - TextMeshPro
   - Rename to "CancelButton"
   - Set Button text to "Cancel"
   - Position at bottom-center of panel

3. **Reset Button**:
   - Right-click PuzzleSettingsPanel → UI > Button - TextMeshPro
   - Rename to "ResetButton"
   - Set Button text to "Reset to Default"
   - Position at bottom-left of panel

### Step 6: Add PuzzleSettingsManager Component

1. **Create Empty GameObject**:
   - Right-click in Hierarchy → Create Empty
   - Rename to "PuzzleSettingsManager"

2. **Add Component**:
   - With PuzzleSettingsManager selected, click "Add Component"
   - Search for "PuzzleSettingsManager" and add it

3. **Assign References in Inspector**:
   - **Settings Panel**: Drag PuzzleSettingsPanel GameObject
   - **Min Rating Slider**: Drag MinRatingSlider
   - **Max Rating Slider**: Drag MaxRatingSlider
   - **Min Rating Text**: Drag MinRatingText
   - **Max Rating Text**: Drag MaxRatingText
   - **Theme Toggles**: Set array size to number of theme toggles, drag each one
   - **Theme Names**: Enter theme names matching each toggle (e.g., "mate", "fork", "pin")
   - **Opening Toggles**: If using openings, add them here
   - **Opening Names**: Enter opening names
   - **Game Manager**: Drag the GameObject with gameManager component

### Step 7: Connect Slider Events

1. **Min Rating Slider**:
   - Select MinRatingSlider
   - In Inspector, scroll to Slider component
   - In "On Value Changed" section, click +
   - Drag PuzzleSettingsManager GameObject to the object field
   - Select: PuzzleSettingsManager → UpdateRatingText()

2. **Max Rating Slider**:
   - Repeat same process for MaxRatingSlider

### Step 8: Connect Button Events

1. **Apply Button**:
   - Select ApplyButton
   - In Button component → On Click(), click +
   - Drag PuzzleSettingsManager to object field
   - Select: PuzzleSettingsManager → ApplySettings()

2. **Cancel Button**:
   - On Click() → PuzzleSettingsManager → CloseSettings()

3. **Reset Button**:
   - On Click() → PuzzleSettingsManager → ResetSettings()

### Step 9: Add Settings Button to Main Menu

1. **Find or Create Main Menu**:
   - Locate your main game menu (likely on hands menu or UI canvas)

2. **Add Settings Button**:
   - Create UI > Button - TextMeshPro
   - Rename to "SettingsButton"
   - Set text to "⚙ Settings" or "Filter Puzzles"

3. **Connect Button**:
   - In Button component → On Click(), click +
   - Drag PuzzleSettingsManager to object field
   - Select: PuzzleSettingsManager → OpenSettings()

### Step 10: Initial Panel State

1. **Hide Settings Panel by Default**:
   - Select PuzzleSettingsPanel in Hierarchy
   - In Inspector, uncheck the checkbox at the top to disable it
   - This ensures it's hidden when game starts

## Testing

1. **Play the Scene**
2. **Click Settings Button** - Panel should open
3. **Adjust Sliders** - Text should update showing current values
4. **Toggle Some Themes** - Select themes you want
5. **Click Apply** - Console should show "Loaded X puzzles with filters..."
6. **Start a New Puzzle** - Should only get puzzles matching your filters

## Advanced Configuration

### Adding More Themes

To add more themes, edit the theme toggles in the Inspector:
- Increase "Theme Toggles" array size
- Drag new toggle GameObjects
- Add corresponding theme names to "Theme Names" array

Theme names should match those in the database `Themes` column.

### Adjusting Rating Range

Default range is 1-3000. To change:
- Modify slider Min/Max values in Inspector
- Update default values in `PuzzleSettings.cs` constructor

### Styling

For VR optimization:
- Increase font sizes for better readability
- Use larger buttons and sliders for hand tracking
- Consider adding spatial UI positioning for VR comfort

## Common Themes Reference

Here are popular chess puzzle themes you can use:

**Tactical Patterns:**
- fork, pin, skewer, discoveredAttack
- deflection, decoy, attraction, clearance
- interference, xRayAttack

**Checkmate Patterns:**
- mate, mateIn1, mateIn2, mateIn3, mateIn4
- backRankMate, smotheredMate, hookMate
- doubleBishopMate

**Game Phases:**
- opening, middlegame, endgame
- pawnEndgame, rookEndgame, queenEndgame
- knightEndgame, bishopEndgame

**Other:**
- sacrifice, promotion, underPromotion
- enPassant, zugzwang, quietMove
- kingsideAttack, queensideAttack

## Troubleshooting

**No puzzles loaded after filtering:**
- Check console for "No puzzles found" warning
- Verify theme names match database exactly (case-sensitive)
- Try widening rating range or removing some filters

**Settings not persisting:**
- Check that PlayerPrefs are working (some platforms restrict them)
- Verify SaveSettings() is being called in ApplySettings()

**UI not appearing:**
- Ensure PuzzleSettingsPanel is child of Canvas
- Check Canvas has proper Render Mode set
- Verify Event System exists in scene

**Sliders not responding:**
- Check that UpdateRatingText() is connected to On Value Changed
- Verify PuzzleSettingsManager reference is assigned

## Code Integration Points

If you need to customize behavior:

- **PuzzleSettingsManager.cs**: Modify UI logic, add new filter types
- **DBService.cs**: Adjust SQL queries for different filtering logic
- **gameManager.cs**: Change when/how puzzles are reloaded
- **PuzzleThemes.cs**: Add/remove available themes and openings

## Next Steps

Consider adding:
- Favorites/bookmarking system for puzzles
- Difficulty progression (adaptive rating)
- Stats tracking for filter combinations
- Custom filter presets (e.g., "Beginner", "Tactical Training")
