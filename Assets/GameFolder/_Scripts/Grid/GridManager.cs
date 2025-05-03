using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using SKC.Events;
using SKC.Helpers;
using SKC.Managers;

namespace SKC.Grid
{
    public class GridManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private SoundManager soundManager;
        public GameObject GridCellPrefab;
        public WordManager WordManager;
        
        [Space, Header("Grid Settings")]
        [Tooltip("This value taken from scriptable object, it's just for test")] public int Width;
        [Tooltip("This value taken from scriptable object, it's just for test")] public int Height;
        public float CellSize = 1.0f; // Cell size
        public float CellSpacing = 0.1f; 

        private GridCell[,] _gridCells;
        private List<GridCell> _selectedCells = new List<GridCell>();
        private List<GridCell> _allCells = new List<GridCell>();
        
        private List<string> _playerFounds = new List<string>(); // Stores the found words from player input
        private List<string> _foundWords = new List<string>(); // Stores the findable words
        private string _currentWord = "";
        private int _width,_height;
        
        // Array representing directions (row and column changes)
        private readonly int[] _rowDirections = { -1, -1, -1, 0, 0, 1, 1, 1 };
        private readonly int[] _colDirections = { -1, 0, 1, -1, 1, -1, 0, 1 };

        private void OnEnable()
        {
            EventBroker.OnGridReset += Refill;
        }

        private void OnDisable()
        {
            EventBroker.OnGridReset -= Refill;
        }

        private void Refill()
        {
            StartCoroutine(InitGrid());   
        }
        
        public IEnumerator InitGrid()
        {
            yield return Helper.GetWait(1.0f);
            _foundWords.Clear();
            WordManager.Reselect();
            yield return Helper.GetWait(.20f);
            CreateGrid();
        }

        [ContextMenu("Generate Grid")]
        public void InitializeGrid(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            StartCoroutine(InitGrid());    
        }
        
        public void CreateGrid()
        {
            // Clear old grid if it exist
            _allCells.Clear();
            ClearGrid();

            _width = Width;
            _height = Height;

            _gridCells = new GridCell[_width, _height];

            for (int row = 0; row < _height; row++)
            {
                for (int col = 0; col < _width; col++)
                {
                    Vector3 cellPosition = new Vector3(
                        col * (CellSize + CellSpacing),
                        -row * (CellSize + CellSpacing),
                        0f
                    ) + transform.position; // add grid container position

                    GameObject cellObject = Instantiate(GridCellPrefab, cellPosition, Quaternion.identity, transform);
                    GridCell cell = cellObject.GetComponent<GridCell>();
                    char randomLetter = GetRandomLetter();
                    cell.Initialize(randomLetter, row, col);
                    cell.OnCellSelected += OnCellSelected;
                    cell.OnCellDeselected += OnCellDeselected;
                    _gridCells[col, row] = cell;
                    _allCells.Add(cell);
                }
            }

            Timer.RunAfter(0.10f, () => FindAllWords());
        }

        private void ClearGrid()
        {
            if (_gridCells != null)
            {
                foreach (GridCell cell in GetAllGridCells())
                {
                    Destroy(cell.gameObject);
                }
            }
            _selectedCells.Clear();
            _currentWord = "";
        }

        private List<GridCell> GetAllGridCells()
        {
            List<GridCell> cells = new List<GridCell>();
            if (_gridCells == null) return cells;

            for (int row = 0; row < _height; row++)
            {
                for (int col = 0; col < _width; col++)
                {
                    cells.Add(_gridCells[col, row]);
                }
            }
            return cells;
        }

        private char GetRandomLetter()
        {
            return WordManager.GetRandomLetter();
        }

        private void OnCellSelected(GridCell cell)
        {
            if (_selectedCells.Count == 0 || IsAdjacent(cell, _selectedCells[_selectedCells.Count - 1]))
            {
                if(_selectedCells.Contains(cell)) return;
                _selectedCells.Add(cell);
                _currentWord += cell.Letter;
                EventBroker.InvokeWordChange(_currentWord);
                soundManager.PlayBlob();
            }
        }

        private void OnCellDeselected(GridCell cell)
        {
            if (_selectedCells.Contains(cell))
            {
                _currentWord = GetCurrentWord();
                _selectedCells.Remove(cell);
                FindWords();
            }
            
            ClearSelection();
        }

        private string GetCurrentWord()
        {
            string word = "";
            foreach (GridCell cell in _selectedCells)
            {
                word += cell.Letter;
            }
            return word;
        }

        private bool IsAdjacent(GridCell cell1, GridCell cell2)
        {
            if (cell1 == null || cell2 == null) return true;

            return Mathf.Abs(cell1.Row - cell2.Row) <= 1 && Mathf.Abs(cell1.Col - cell2.Col) <= 1;
        }

        public void FindWords()
        {
            if(_playerFounds.Contains(_currentWord)) return; // Show UI Later
            Helper.Debug("Finding words...");
            
            // Just detect only selected word
            if( _currentWord.Length <= 0) return; // Do UI Popup
            if (WordManager.IsWord(_currentWord) && !_playerFounds.Contains(_currentWord))
            {
                _playerFounds.Add(_currentWord);
                Helper.Debug("Player Found word: " + _currentWord);
                EventBroker.InvokeFoundWord(_currentWord);
            }
            else
            {
                Helper.Debug("Word not found: " + _currentWord);
                ClearSelection();
                return;
            }

            // Clear selected
            ClearSelection();
        }
        
        public void FindAllWords()
        {
            Helper.Debug("Finding all words in the grid...");
            for (int row = 0; row < Height; row++)
            {
                for (int col = 0; col < Width; col++)
                {
                    FindWordsRecursive(row, col, "", new HashSet<(int, int)>());
                }
            }
        }
        
        private void FindWordsRecursive(int row, int col, string currentWord, HashSet<(int, int)> visitedCells)
        {
            if (row < 0 || row >= Height || col < 0 || col >= Width || visitedCells.Contains((row, col)))
            {
                return;
            }

            currentWord += _gridCells[col, row].Letter;

            // TRIE OPTIMIZATION: Check if the current word is a prefix
            if (!WordManager.IsPrefix(currentWord))
            {
                return; // Stop exploring this path if it's not a prefix
            }

            if (currentWord.Length > 1 && WordManager.IsWord(currentWord) && !_foundWords.Contains(currentWord))
            {
                _foundWords.Add(currentWord);
                EventBroker.InvokeFoundableWord(currentWord);
                Helper.Debug("Foundable word: " + currentWord);
            }

            visitedCells.Add((row, col));

            for (int dir = 0; dir < 8; dir++)
            {
                int nextRow = row + _rowDirections[dir];
                int nextCol = col + _colDirections[dir];
                FindWordsRecursive(nextRow, nextCol, currentWord, new HashSet<(int, int)>(visitedCells));
            }
        }

        private void ClearSelection()
        {
            foreach (GridCell tempCell in _allCells)
            {
                tempCell.ResetCell();
            }
            soundManager.Reset();
            _selectedCells.Clear();
            _currentWord = "";
            EventBroker.InvokeWordChange(_currentWord);
        }
    }
}