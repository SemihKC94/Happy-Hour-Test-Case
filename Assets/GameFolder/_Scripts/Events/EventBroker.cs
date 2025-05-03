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

        public static event Action OnPlay;
        public static void InvokePlay()
        {
            OnPlay?.Invoke();
        }

        public static event Action<string> OnFoundableWord;
        public static void InvokeFoundableWord(string word)
        {
            OnFoundableWord?.Invoke(word);
        }

        public static event Action OnLevelSuccess;
        public static void InvokeLevelSuccess()
        {
            OnLevelSuccess?.Invoke();
        }
        public static event Action OnGridReset;
        public static void InvokeGridReset()
        {
            OnGridReset?.Invoke();
        }

        public static event Action<string> OnFoundWord;
        public static void InvokeFoundWord(string word)
        {
            OnFoundWord?.Invoke(word);
        }

        public static event Action<string> OnWordChange;
        public static void InvokeWordChange(string word)
        {
            OnWordChange?.Invoke(word);
        }
    }
}
