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
            IDbCommand dbCommandReadValues = dbConnection.CreateCommand(); // 15
            dbCommandReadValues.CommandText = string.Format("SELECT * FROM Puzzles LIMIT {0}", howMany); // 16
            IDataReader dataReader = dbCommandReadValues.ExecuteReader(); // 17
            
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
        catch (Exception ex) { }
        finally
        {
            dbConnection.Close(); // 20
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
