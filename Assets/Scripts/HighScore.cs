using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HighScore : MonoBehaviour
{
    [SerializeField] private Text highScoreTableText;
    [SerializeField] private MainManager mainManagerScript;


    private StartMenu _startMenuData;

    private HighScoreEntries _highScoreEntries = new HighScoreEntries();

    void Start()
    {
        _startMenuData = GameObject.Find("StartMenuData").GetComponent<StartMenu>();

        LoadHighScoreEntries();
        ShowHighScoreTable();
    }

    // This method is called by the main manager.
    public void AddNewHighScoreEntryIfGreater()
    {
        var highScoreEntriesList = _highScoreEntries.table.ToList();

        // Update the high score value if an high score entry with the same name already exists
        if (DoCurrentHighScoreEntryAlreadyExist(highScoreEntriesList))
        {
            var existingHighScoreEntry =
                highScoreEntriesList.Find(highScoreEntry => highScoreEntry.playerName == _startMenuData.playerName);

            if (mainManagerScript.Points > existingHighScoreEntry.highScore)
            {
                existingHighScoreEntry.highScore = mainManagerScript.Points;
            }
        }
        // Add a new high score entry if there isn't already.
        else
        {
            highScoreEntriesList.Add(
                new HighScoreEntries.SingleScoreEntry(mainManagerScript.Points, _startMenuData.playerName));
        }

        highScoreEntriesList =
            highScoreEntriesList.OrderByDescending(highScoreEntry => highScoreEntry.highScore).ToList();

        // Remove the smallest high score if there is a greater and new high score entry.
        if (highScoreEntriesList.Count > _highScoreEntries.table.Length)
        {
            highScoreEntriesList.RemoveAt(highScoreEntriesList.Count - 1);
        }

        _highScoreEntries.table = highScoreEntriesList.ToArray();


        SaveHighScoreEntries();

        ShowHighScoreTable();
    }

    private bool DoCurrentHighScoreEntryAlreadyExist(List<HighScoreEntries.SingleScoreEntry> highScoreEntriesList)
    {
        return highScoreEntriesList.Find(highScoreEntry => highScoreEntry.playerName == _startMenuData.playerName) !=
               null;
    }

    private void ShowHighScoreTable()
    {
        highScoreTableText.text = string.Empty;

        for (int index = 0; index < _highScoreEntries.table.Length; index++)
        {
            highScoreTableText.text += index + 1 + " \"" + _highScoreEntries.table[index].playerName + "\" " +
                                       _highScoreEntries
                                           .table[index].highScore + "\n";
        }
    }

    #region Data Persistence

    [Serializable]
    class HighScoreEntries
    {
        [Serializable]
        public class SingleScoreEntry
        {
            public int highScore;
            public string playerName;

            public SingleScoreEntry(int highScore = 0, string playerName = "-")
            {
                this.highScore = highScore;
                this.playerName = playerName;
            }
        }

        public SingleScoreEntry[] table = new SingleScoreEntry[5];

        public HighScoreEntries()
        {
            for (int index = 0; index < table.Length; index++)
            {
                table[index] = new SingleScoreEntry();
            }
        }
    }

    private void SaveHighScoreEntries()
    {
        string json = JsonUtility.ToJson(_highScoreEntries);

        File.WriteAllText(Application.persistentDataPath + "/saveFile.json", json);
    }

    private void LoadHighScoreEntries()
    {
        string path = Application.persistentDataPath + "/saveFile.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            HighScoreEntries latestHighScoreEntries = JsonUtility.FromJson<HighScoreEntries>(json);

            _highScoreEntries = latestHighScoreEntries;
        }
    }

    #endregion
}