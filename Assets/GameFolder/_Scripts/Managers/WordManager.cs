using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SKC.Events;
using SKC.Grid;
using SKC.Helpers;
using SKC.Trie;
using Random = UnityEngine.Random;

namespace SKC.Managers
{
    public class WordManager : MonoBehaviour
    {
        public GridManager GridManager;
        private TextAsset WordListJson; // Aassign the JSON file from the Unity Editor
        private Trie.Trie _trie = new Trie.Trie();
        private List<string> _wordList = new List<string>();
        private List<char> _charList = new List<char>();
        private List<string> _selectedWordList = new List<string>();
        private int wordIndex = 0;

        public void Initialize(TextAsset WordListJson)
        {
            this.WordListJson = WordListJson;
            LoadWords();
            PopulateTrie();
        }

        private void OnEnable()
        {
            EventBroker.OnGridReset += Reselect;
        }

        private void OnDisable()
        {
            EventBroker.OnGridReset -= Reselect;
        }

        public void Reselect()
        {
            wordIndex = 0;
            _charList.Clear();
            _selectedWordList.Clear();

            int selectCount = GridManager.Height * GridManager.Width;

            List<int> tempSelectedIndex = new List<int>();

            while (_selectedWordList.Count < 5)
            {
                int randomIndex = Random.Range(0, _wordList.Count);

                if (!tempSelectedIndex.Contains(randomIndex))
                {
                    _selectedWordList.Add(_wordList[randomIndex]);
                    tempSelectedIndex.Add(randomIndex);
                }
            }

            for (int i = 0; i < _selectedWordList.Count; i++)
            {
                for (int j = 0; j < _selectedWordList[i].Length; j++)
                {
                    _charList.Add(_selectedWordList[i][j]);
                }
            }
            
            Helper.ShuffleVariant(_charList);
        }

        private void LoadWords()
        {
            if (WordListJson == null)
            {
                Debug.LogError("Word list JSON file is not assigned!");
                return;
            }

            string jsonText = WordListJson.text;
            WordListWrapper wordListWrapper = JsonUtility.FromJson<WordListWrapper>(jsonText);
            _wordList = wordListWrapper.words;
        }

        private void PopulateTrie()
        {
            foreach (string word in _wordList)
            {
                _trie.Insert(word.ToUpper());
            }
        }

        public bool IsWord(string word)
        {
            return _trie.IsWord(word.ToUpper());
        }

        public bool IsPrefix(string prefix)
        {
            return _trie.IsPrefix(prefix.ToUpper());
        }

        public List<string> GetWordList()
        {
            return _wordList;
        }
        
        public char GetRandomLetter()
        {
            int randomIndex = wordIndex;

            if (wordIndex < _charList.Count - 1)
                wordIndex++;
            else
            {
                return (char)Random.Range('A', 'Z' + 1);
            }
                    
            return _charList[randomIndex];
            
        }

        // Helper class to wrap a list of words in a JSON file
        [System.Serializable]
        private class WordListWrapper
        {
            public List<string> words;
        }
    }
}
