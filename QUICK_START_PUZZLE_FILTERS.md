# Quick Start: Puzzle Filter Settings

## What's New

Your puzzle mode now supports filtering by:
- ⭐ **Rating Range** (ELO): Filter puzzles between min/max difficulty
- 🎯 **Themes**: Select tactical patterns (fork, pin, mate, sacrifice, etc.)
- 📖 **Openings**: Filter by chess opening types (Sicilian, French, etc.)

## How It Works

The implementation includes:
1. **Database Filtering** - SQL queries now properly filter puzzles
2. **Settings Manager** - Stores user preferences and manages UI
3. **Auto-reload** - Puzzles reload when you apply new filters

## Quick Setup (3 Options)

### Option 1: Automated UI Setup (Recommended)

1. In Unity Editor, go to menu: **Tools > Setup Puzzle Settings UI**
2. Click "Create Settings UI" button
3. Select the created "PuzzleSettingsManager" in Hierarchy
4. In Inspector, assign the references:
   - Drag UI elements to corresponding fields
   - Set Theme Names array (e.g., "mate", "fork", "pin")
5. Connect button onClick events (see detailed guide)

### Option 2: Manual UI Setup

Follow the complete guide in `PUZZLE_SETTINGS_SETUP_GUIDE.md`

### Option 3: Script-Only (No UI)

If you just want to test filtering without UI:

```csharp
// In gameManager.cs Start() method, replace GetPuzzles call:
PuzzleSettings testSettings = new PuzzleSettings();
testSettings.MinRating = 1000;
testSettings.MaxRating = 2000;
testSettings.SelectedThemes.Add("fork");
testSettings.SelectedThemes.Add("pin");
ReloadPuzzles(testSettings);
```

## Testing

After setup, test by:

1. **Run the game**
2. **Open Settings** (click your settings button)
3. **Set filters**:
   - Move sliders to set rating range (e.g., 1500-2000)
   - Check some theme boxes (e.g., "mate", "fork")
4. **Click Apply**
5. **Check Console** - Should see: "Loaded X puzzles with filters..."
6. **Play puzzles** - Should only see filtered puzzles

## Available Themes

Popular themes you can use:

**Checkmate:** mate, mateIn1, mateIn2, mateIn3, backRankMate, smotheredMate

**Tactics:** fork, pin, skewer, discoveredAttack, sacrifice, deflection

**Game Phase:** opening, middlegame, endgame, pawnEndgame, rookEndgame

See `PuzzleThemes.cs` for complete list.

## File Changes Summary

```
NEW FILES:
✓ Assets/Models/PuzzleSettings.cs
✓ Assets/Scripts/PuzzleSettingsManager.cs
✓ Assets/Scripts/PuzzleThemes.cs
✓ Assets/Scripts/Editor/PuzzleSettingsUISetup.cs

MODIFIED:
✓ Assets/Scripts/DBService.cs (added SQL WHERE clause filtering)
✓ Assets/Scripts/gameManager.cs (added ReloadPuzzles method)
```

## Common Issues

**No puzzles found after filtering?**
- Widen your rating range
- Remove some theme filters
- Check theme names match database exactly

**UI not appearing?**
- Ensure panel is child of Canvas
- Check Event System exists in scene
- Verify panel is enabled when you want to show it

**Settings not saving?**
- Check PlayerPrefs are allowed on your platform
- Verify ApplySettings() is called when clicking Apply

## Next Steps

Consider adding:
- Settings button on your main menu
- Visual feedback when filters are active
- Preset filter combinations (Beginner, Advanced, etc.)
- Statistics on filtered puzzle sets

## Need Help?

See the detailed guide: `PUZZLE_SETTINGS_SETUP_GUIDE.md`
