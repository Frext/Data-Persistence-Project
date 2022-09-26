using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
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
        for (int index = 0; index < _highScoreEntries.table.Length; index++)
        {
            // If a high score entry already exists, update it if it has a greater high score value.
            if (DoHighScoreEntryAlreadyExist())
            {
                if (mainManagerScript.Points > _highScoreEntries.table[index].highScore && _startMenuData.playerName.Equals(_highScoreEntries.table[index].playerName))
                {
                    _highScoreEntries.table[index].highScore = mainManagerScript.Points;
                }
            }

            // If a high score entry doesn't exist, insert it to the high score entries and delete the smallest high score.
            else if (mainManagerScript.Points > _highScoreEntries.table[index].highScore)
            {
                List<HighScoreEntries.SingleScoreEntry> list = _highScoreEntries.table.ToList();

                list.Insert(index,
                    new HighScoreEntries.SingleScoreEntry(mainManagerScript.Points, _startMenuData.playerName));

                list.Remove(list.Last());

                _highScoreEntries.table = list.ToArray();
            }
            
            SaveHighScoreEntries();
            ShowHighScoreTable();
        }
    }

    private bool DoHighScoreEntryAlreadyExist()
    {
        foreach (var singleHighScoreEntry in _highScoreEntries.table.ToList())
        {
            if (singleHighScoreEntry.playerName.Equals(_startMenuData.playerName))
            {
                return true;
            }
        }

        return false;
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