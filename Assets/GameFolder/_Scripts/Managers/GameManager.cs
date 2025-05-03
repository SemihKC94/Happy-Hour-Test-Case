using System;
using System.Collections;
using System.Collections.Generic;
using SKC.Boot;
using SKC.Events;
using SKC.Helpers;
using SKC.Helpers.Enums;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SKC.Core
{
    public class GameManager : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private SplashManager _splashManager;
        [SerializeField] private string levelSceneName;
        [SerializeField] private Button playButton;
        
        private void Start()
        {
            EventBroker.InvokeOnGameInitialize();
            
            playButton.onClick.AddListener(() => EventBroker.InvokePlay());
        }

        private void OnEnable()
        {
            EventBroker.OnPlay += Play;
        }

        private void OnDisable()
        {
            EventBroker.OnPlay -= Play;
        }

        private void Play()
        {
            StartCoroutine(LoadLevel());
        }
        
        IEnumerator LoadLevel()
        {
            // Do later => Save System or 3pt tools (Ad, Analytics etc)
            yield return null;
            // Load scene async after save file restored
            AsyncOperation operation = SceneManager.LoadSceneAsync(levelSceneName);
            operation.allowSceneActivation = false;
            _splashManager.ProcessOutro();
            while (!operation.isDone)
            {
                
                if (operation.progress >= 0.9f)
                {
                    operation.allowSceneActivation = true;
                }

                yield return null;
            }
        }
    }
}
