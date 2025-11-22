using System;
using System.Collections.Generic;

[Serializable]
public class PuzzleSettings
{
    public int MinRating { get; set; }
    public int MaxRating { get; set; }
    public List<string> SelectedThemes { get; set; }
    public List<string> SelectedOpenings { get; set; }

    public PuzzleSettings()
    {
        MinRating = 1;
        MaxRating = int.MaxValue;
        SelectedThemes = new List<string>();
        SelectedOpenings = new List<string>();
    }

    public void Reset()
    {
        MinRating = 1;
        MaxRating = int.MaxValue;
        SelectedThemes.Clear();
        SelectedOpenings.Clear();
    }
}
