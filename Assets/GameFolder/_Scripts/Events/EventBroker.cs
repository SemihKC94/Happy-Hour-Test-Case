using System;
using SKC.Helpers.Enums;

namespace SKC.Events
{
    public class EventBroker
    {
        public static event Action OnGameInitialize;
        public static void InvokeOnGameInitialize()
        {
            OnGameInitialize?.Invoke();
        }
        
        public static event Action<SplashConfig> OnSplash;
        public static void InvokeOnSplash(SplashConfig splashConfig)
        {
            OnSplash?.Invoke(splashConfig);
        }
        
        public static event Action OnPlay;
        public static void InvokePlay()
        {
            OnPlay?.Invoke();
        }

        public static event Action OnFail;
        public static void InvokeFail()
        {
            OnFail?.Invoke();
        }
    
        public static event Action OnLevelReset;
        public static void InvokeLevelReset()
        {
            OnLevelReset?.Invoke();
        }

        public static event Action OnLevelSuccess;
        public static void InvokeLevelSuccess()
        {
            OnLevelSuccess?.Invoke();
        }

        public static event Action OnNextLevel;
        public static void InvokeNextLevel()
        {
            OnNextLevel?.Invoke();
        }
    }
}
