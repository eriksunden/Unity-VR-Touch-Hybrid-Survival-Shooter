using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace CompleteProject
{
    public class HighScoreManager : MonoBehaviour
    {
        [SerializeField]
        public int numberOfHighScores = 5;

        [SerializeField]
        private List<int> highScorePoints = new List<int>();

        [SerializeField]
        private List<string> highScoreNames = new List<string>();

        private SortedList<int, string> highScores = new SortedList<int, string>(new DuplicateKeyComparer<int>());

        private VirtualKeyboard vk = new VirtualKeyboard();

        private bool gameOver = false;
        private int newScoreIndex = 0;
        private List<Text> highScoreText = new List<Text>();
        private int currentHighScoreIdx;

        public GUIText gt;

        void Awake ()
        {
            highScoreText.Add(GameObject.Find("HighScoreLine").GetComponent<Text>());
            for (int i = 1; i <= numberOfHighScores; i++)
            {
                highScoreText.Add(GameObject.Find("HighScore"+i.ToString()).GetComponent<Text>());
            }
            HideHighScore();

            LoadHighScores();
            UpdateHighScoreText();

            Debug.Log(Application.persistentDataPath);
        }

        void Update ()
        {
            /*if (Input.GetKeyDown(KeyCode.K))
            {
                OpenKeyboard();
            }
            if (Input.GetKeyDown(KeyCode.H))
            {
                CloseKeyboard();
            }*/

            if (gameOver)
            {
                foreach (char c in Input.inputString)
                {
                    if (c == "\b"[0])
                    {
                        if (highScoreNames[currentHighScoreIdx].Length != 0)
                        {
                            highScoreNames[currentHighScoreIdx] = highScoreNames[currentHighScoreIdx].Substring(0, highScoreNames[currentHighScoreIdx].Length - 1);
                        }
                    }
                    else
                    {
                        if (c == "\n"[0] || c == "\r"[0])
                            print("User entered their name: " + highScoreNames[currentHighScoreIdx]);
                        else if(highScoreNames[currentHighScoreIdx].Length < 12)
                            highScoreNames[currentHighScoreIdx] += c;
                    }

                    UpdateSingleHighScoreText(currentHighScoreIdx);
                }
                if(highScoreNames[currentHighScoreIdx].Length > 0)
                {
                    if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown("joystick button 0"))
                    {
                        gameOver = false;

                        SaveHighScores();

                        CloseKeyboard();
                        SceneManager.LoadScene(0);
                    }
                }
            }
        }

        public void NewScore(int score)
        {
            gameOver = true;
            // Check if new high score
            // Insert empty string with score and se where it ends up
            highScores.Add(score, "");
            newScoreIndex = highScores.IndexOfValue("");
            if(newScoreIndex > 0)
            {
                //Remove previous 5th place
                highScores.RemoveAt(0);

                FromDictionaryToLists();
                UpdateHighScoreText("Grattis!! Du är med på topplistan." + Environment.NewLine);

                // We have a new high score
                OpenKeyboard();

                // Change to correct place
                currentHighScoreIdx = newScoreIndex - 1;

                ShowHighScore();
            }
            else
            {
                // No new high score, remove it again
                highScores.RemoveAt(0);
                UpdateHighScoreText("Tyvärr, inget rekord.." + Environment.NewLine);
                ShowHighScore();
            }
        }

        void LoadHighScores()
        {
            if (File.Exists(Application.persistentDataPath + "/highScore.gd"))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(Application.persistentDataPath + "/highScore.gd", FileMode.Open);
                highScorePoints = (List<int>)bf.Deserialize(file);
                highScoreNames = (List<string>)bf.Deserialize(file);
                file.Close();
            }
            FromListsToDictionary();
            FromDictionaryToLists();
        }

        void SaveHighScores ()
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(Application.persistentDataPath + "/highScore.gd");
            bf.Serialize(file, highScorePoints);
            bf.Serialize(file, highScoreNames);
            file.Close();
        }

        void UpdateSingleHighScoreText(int idx)
        {
            int place = numberOfHighScores - idx;
            if (idx <= highScorePoints.Count)
            {
                highScoreText[place].text = place.ToString() + ")  " + highScoreNames[idx] + " : " + highScorePoints[idx];
            }
            else
            {
                highScoreText[place].text = place.ToString() + ")  - : -";
            }
        }

        void UpdateHighScoreText(string startText = "")
        {
            highScoreText[0].text = startText;
            for (int i = numberOfHighScores-1; i >= 0; i--)
            {
                UpdateSingleHighScoreText(i);
            }
        }

        // Copy high scores from lists to sorted dictionary
        void FromListsToDictionary()
        {
            for (int i = 0; i < numberOfHighScores; i++)
            {
                highScores.Add(highScorePoints[i], highScoreNames[i]);
            }
        }

        // Copy high scores from sorted dictionary to lists
        void FromDictionaryToLists()
        {
            highScorePoints.Clear();
            highScoreNames.Clear();
            foreach (KeyValuePair<int, string> highScore in highScores)
            {
                highScorePoints.Add(highScore.Key);
                highScoreNames.Add(highScore.Value);
            }
        }

        void ShowHighScore()
        {
            foreach (Text lbl in highScoreText)
            {
                lbl.enabled = true;
            }
        }

        void HideHighScore()
        {
            foreach (Text lbl in highScoreText)
            {
                lbl.enabled = false;
            }
        }

        void OpenKeyboard()
        {
            if (vk.IsWindows8OrNewer())
            {
                vk.ShowTouchKeyboard();
            }
            else
            {
                vk.ShowOnScreenKeyboard();
            }
        }

        void CloseKeyboard()
        {
            if (vk.IsWindows8OrNewer())
            {
                vk.HideTouchKeyboard();
            }
            else
            {
                vk.HideOnScreenKeyboard();
            }
        }

        public class DuplicateKeyComparer<TKey>:IComparer<TKey> where TKey : IComparable
        {
            #region IComparer<TKey> Members

            public int Compare(TKey x, TKey y)
            {
                int result = x.CompareTo(y);

                if (result == 0)
                    return 1;   // Handle equality as beeing bigger
                else
                    return result;
            }

            #endregion
        }
    }
}