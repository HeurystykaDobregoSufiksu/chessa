using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System.IO;
using System;
using System.Linq;
using System.Data.Common;

public class DBService
{
    public List<PuzzleModel> GetPuzzles(List<string>? themes, int minElo=1, int maxElo=int.MaxValue, int howMany = 100)
    {

        List<PuzzleModel> puzzles = new List<PuzzleModel>();
        IDbConnection dbConnection = Connect();
        try
        {
            IDbCommand dbCommandReadValues = dbConnection.CreateCommand();

            // Build WHERE clause for filtering
            List<string> conditions = new List<string>();

            // Rating filter
            if (minElo > 1 || maxElo < int.MaxValue)
            {
                conditions.Add(string.Format("Rating BETWEEN {0} AND {1}", minElo, maxElo));
            }

            // Theme filter - check if any theme matches
            if (themes != null && themes.Count > 0)
            {
                List<string> themeConditions = new List<string>();
                foreach (string theme in themes)
                {
                    themeConditions.Add(string.Format("Themes LIKE '%{0}%'", theme.Replace("'", "''")));
                }
                conditions.Add("(" + string.Join(" OR ", themeConditions) + ")");
            }

            // Build final query
            string whereClause = conditions.Count > 0 ? " WHERE " + string.Join(" AND ", conditions) : "";
            dbCommandReadValues.CommandText = string.Format("SELECT * FROM Puzzles{0} ORDER BY RANDOM() LIMIT {1}", whereClause, howMany);

            IDataReader dataReader = dbCommandReadValues.ExecuteReader();

            while (dataReader.Read())
            {
                PuzzleModel puzzle = new();
                puzzle.PuzzleId = dataReader.GetString(0);
                puzzle.FEN = dataReader.GetString(1);
                puzzle.Moves = dataReader.GetString(2);
                puzzle.Rating = dataReader.GetInt32(3);
                puzzle.RatingDeviation = dataReader.GetInt32(4);
                puzzle.Popularity = dataReader.GetInt32(5);
                puzzle.NbPlays = dataReader.GetInt32(6);
                puzzle.Themes = dataReader.GetString(7);
                puzzles.Add(puzzle);
            }
            return puzzles;
        }
        catch (Exception ex)
        {
            Debug.LogError("Error loading puzzles: " + ex.Message);
        }
        finally
        {
            dbConnection.Close();
        }
        return puzzles;
    }
    private IDbConnection Connect() // 3
    {
        IDbConnection dbConnection = null;

        try
        {
            string filePath = Path.Combine(Application.streamingAssetsPath, "ChessPuzzleDB.db");
            string dbUri = "URI=file:" + filePath;
            dbConnection = new SqliteConnection(dbUri);
            dbConnection.Open();
        }
        catch (Exception ex)
        {
            Debug.LogError("DB Connection error: " + ex.Message);
            return null;
        }
        return dbConnection;
    }
}
