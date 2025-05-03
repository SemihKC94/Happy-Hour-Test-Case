using System;
using System.Collections.Generic;
using DG.Tweening;
using SKC.Boot;
using SKC.Events;
using SKC.Grid;
using SKC.Helpers;
using SKC.Level;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

namespace SKC.Managers
{
    public class UIManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private SplashManager  splashManager;
        [SerializeField] private SoundManager soundManager;
        
        [Space, Header("UI Elements")]
        [SerializeField] private CanvasGroup gamePanel;
        [SerializeField] private CanvasGroup winPanel;
        [SerializeField] private Image backGroundImage;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI infoText;
        [SerializeField] private TextMeshProUGUI currentWordText;
        [SerializeField] private Button searchButton;
        [SerializeField] private Button refillButton;
        [SerializeField] private Button nextButton;

        [Space, Header("Others")] 
        [SerializeField] private GameObject _foundWordsHolder;
        [SerializeField] private Transform _foundWordsContainer;
        [SerializeField] private GameObject _targetVisual;
        [SerializeField] private Transform _targetContainer;
        [SerializeField] private CanvasGroup findableWordCanvas;
        [SerializeField] private TextMeshProUGUI[] findableWords;
        
        // Private 
        private LevelData _levelData;
        private List<string> _foundWords =  new List<string>();
        private List<string> _foundableWords =  new List<string>();
        private List<GameObject> _targets = new List<GameObject>();
        private int targetWordCount = 0;
        private int currentWordCount = 0;
        private string _currentWord = "";

        private void Start()
        {
            searchButton.onClick.AddListener(() => Search());
            refillButton.onClick.AddListener(() => Refill());
            nextButton.onClick.AddListener(() => GoNext());
            
            SetCanvasGroup(gamePanel, 2.0f);
        }

        private void OnEnable()
        {
            EventBroker.OnWordChange += ChangeCurrentWord;
            EventBroker.OnFoundWord += WordFound;
            EventBroker.OnFoundableWord += SetFoundableWords;
            EventBroker.OnLevelSuccess += Win;
        }

        private void OnDisable()
        {
            EventBroker.OnWordChange -= ChangeCurrentWord;
            EventBroker.OnFoundWord -= WordFound;
            EventBroker.OnFoundableWord -= SetFoundableWords;
            EventBroker.OnLevelSuccess -= Win;
        }

        public void Initialize(LevelData levelData, int  levelIndex)
        {
            currentWordCount = 0;
            _foundWords.Clear();
            _targets.Clear();
            _levelData = levelData;

            backGroundImage.sprite = _levelData.LevelImage;
            levelText.SetText($"LEVEL {levelIndex}");
            infoText.SetText($"FIND WORDS ABOUT " + levelData.LevelTitle.ToUpper());
            targetWordCount = _levelData.TargetWordCount;

            CreateTargets();
        }

        public void SetFoundableWords(string foundableWords)
        {
            if(!_foundableWords.Contains(foundableWords))
                _foundableWords.Add(foundableWords);
        }
        
        private void CreateTargets()
        {
            for (int i = 0; i < targetWordCount; i++)
            {
                GameObject target = (GameObject) Instantiate(_targetVisual,_targetContainer) as GameObject;
                _targets.Add(target.transform.GetChild(0).gameObject);
            }
        }

        private void ChangeCurrentWord(string word)
        {
            _currentWord = word;
            currentWordText.SetText(_currentWord);
        }

        private void WordFound(string word)
        {
            currentWordCount++;
            GameObject foundWord = (GameObject) Instantiate(_foundWordsHolder,_foundWordsContainer) as GameObject;
            foundWord.GetComponentInChildren<TextMeshProUGUI>().text = word;
            _targets[currentWordCount - 1].gameObject.SetActive(true);

            if (currentWordCount == targetWordCount)
            {
                soundManager.PlayWin();
                EventBroker.InvokeLevelSuccess();
            }
        }

        private void Search()
        {
            searchButton.interactable = false;
            
            Timer.RunAfter(1.0f, () =>
            {
                searchButton.interactable = true;
                SetFindableWords();
            });
        }

        private void SetFindableWords()
        {
            foreach (var item in findableWords)
            {
                item.SetText("");
            }

            if (_foundableWords.Count > 0)
            {
                for (int i = 0; i < _foundableWords.Count - 1 ; i++)
                {
                    findableWords[i].SetText(_foundableWords[i]);
                }
            }
            else
            {
                findableWords[8].SetText("THERE IS NO WORD TO FIND. PLEASE REFILL THE GRID!");
            }

            findableWordCanvas.alpha = 1.0f;
            findableWordCanvas.DOFade(0.0f, 0.20f).SetDelay(1.0f);
        }

        private void Refill()
        {
            _foundableWords.Clear();
            refillButton.interactable = false;
            EventBroker.InvokeGridReset();
            Timer.RunAfter(1.2f, () =>
            {
                refillButton.interactable = true;
            });
        }

        private void Win()
        {
            SetCanvasGroup(winPanel, 1.0f);
        }

        private void GoNext()
        {
            splashManager.ProcessOutro();

            Timer.RunAfter(0.30f, () => SceneManager.LoadScene("Main"));
        }

        private void SetCanvasGroup(CanvasGroup cg, float duration)
        {
            cg.alpha = 0;
            cg.interactable = false;
            cg.blocksRaycasts = false;
            
            cg.DOFade(1.0f, duration);
            Timer.RunAfter(duration, () =>
            {
                cg.interactable = true;
                cg.blocksRaycasts = true;
                cg.alpha = 1;
            });
        }
    }
}
