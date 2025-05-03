using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SKC.Events;
using SKC.Helpers.Enums;
using DG.Tweening;
using SKC.Helpers;

namespace SKC.Boot
{
    public class SplashManager : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private bool onStart = false;
        [SerializeField] private SplashConfig _splashConfig;
        [SerializeField] private CanvasGroup _splashCanvasGroup;
        [SerializeField] private Ease _ease = Ease.Linear;
        [SerializeField] private float _duration = 1.0f;
        
        // Privates
        private Sequence _splashSequence;

        private void Awake()
        {
            if(!onStart) return;
            
            switch (_splashConfig)
            {
                case SplashConfig.Intro: ProcessIntro(); break;
                case SplashConfig.Outro:  ProcessOutro(true); break;
                case SplashConfig.Both:  ProcessBoth(); break;
            }
        }

        private void RunSplash(SplashConfig splashConfig)
        {
            switch (splashConfig)
            {
                case SplashConfig.Intro: ProcessIntro(); break;
                case SplashConfig.Outro: ProcessOutro(); break;
                case SplashConfig.Both:  ProcessBoth(); break;
                case SplashConfig.None: Helper.DebugError($"There is no function for {splashConfig}"); break;
            }
        }

        public void ProcessIntro()
        {
            Helper.Debug($"<color=blue>Intro</color> initializing ...");
            
            DOTween.Kill(_splashCanvasGroup);
            SetCanvasGroupImmediately(true);
            
            _splashCanvasGroup.DOFade(0.0f, _duration).SetEase(_ease).OnComplete(() => SetCanvasGroupImmediately(false));
        }
        
        public void ProcessOutro(bool afterRun = false)
        {
            Helper.Debug($"<color=blue>Outro</color> initializing ...");
            
            DOTween.Kill(_splashCanvasGroup);
            SetCanvasGroupImmediately(false);
            
            _splashCanvasGroup.DOFade(1.0f, _duration / 2.0f).SetEase(_ease).OnComplete(() =>
            {
                SetCanvasGroupImmediately(true);
                if (afterRun)
                {
                    ProcessIntro();
                }
            });
        }

        public void ProcessBoth()
        {
            Helper.Debug($"<color=blue>Splash</color> initializing ...");
            
            DOTween.Kill(_splashCanvasGroup);
            SetCanvasGroupImmediately(false);
            _splashCanvasGroup.blocksRaycasts = true;
            _splashSequence =  DOTween.Sequence();
            _splashSequence.Append(_splashCanvasGroup.DOFade(1.0f, 0.20f).SetEase(_ease));
            _splashSequence.Append(_splashCanvasGroup.DOFade(0.0f, _duration).SetEase(_ease).SetDelay(0.50f));
            _splashSequence.OnComplete(() => SetCanvasGroupImmediately(false));
            _splashSequence.Play();
        }

        private void SetCanvasGroupImmediately(bool on)
        {
            if (on)
            {
                _splashCanvasGroup.alpha = 1.0f;
                _splashCanvasGroup.blocksRaycasts = true;
                return;
            }
            
            _splashCanvasGroup.alpha = 0.0f;
            _splashCanvasGroup.blocksRaycasts = false;
        }
    }
}
