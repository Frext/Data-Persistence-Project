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
            if (mainManagerScript.Points > _highScoreEntries.table[index].highScore)
            {
                if (!_startMenuData.playerName.Equals(_highScoreEntries.table[index].playerName))
                {
                    List<HighScoreEntries.SingleScoreEntry> list = _highScoreEntries.table.ToList();

                    list.Insert(index,
                        new HighScoreEntries.SingleScoreEntry(mainManagerScript.Points, _startMenuData.playerName));

                    // Remove the smallest value in the list.
                    list.Remove(list.Last());

                    _highScoreEntries.table = list.ToArray();

                    SaveHighScoreEntries();
                    ShowHighScoreTable();

                    index = 4;
                }

                else if (_startMenuData.playerName.Equals(_highScoreEntries.table[index].playerName))
                {
                    _highScoreEntries.table[index].highScore = mainManagerScript.Points;
                
                    SaveHighScoreEntries();
                    ShowHighScoreTable();
                
                    index = 4;
                }
            }
            
            else if (_startMenuData.playerName.Equals(_highScoreEntries.table[index].playerName))
            {
                index = 4;
            }
        }
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