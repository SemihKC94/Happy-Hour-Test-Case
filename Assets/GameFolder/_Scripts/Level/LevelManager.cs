using System;
using System.Collections;
using System.Collections.Generic;
using SKC.Events;
using SKC.Level;
using UnityEngine;

namespace SKC.Managers
{
    public class LevelManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private LevelInitialize levelInitialize;
        
        [Space, Header("Level Datas")]
        [SerializeField] private LevelData[] _levelDatas;

        private int LevelIndex;
        private int tempLevelIndex;
        private void Awake()
        {
            CheckSave(); // Temporary save
            if (LevelIndex >= _levelDatas.Length)
            {
                tempLevelIndex = LevelIndex % _levelDatas.Length;
                levelInitialize.InitializeLevel(_levelDatas[tempLevelIndex], LevelIndex + 1);
            }
            else
            {
                levelInitialize.InitializeLevel(_levelDatas[LevelIndex], LevelIndex + 1);
            }
        }

        private void OnEnable()
        {
            EventBroker.OnLevelSuccess += Win;
        }

        private void OnDisable()
        {
            EventBroker.OnLevelSuccess -= Win;
        }

        private void CheckSave()
        {
            if(PlayerPrefs.HasKey("LevelIndex")) LevelIndex =  PlayerPrefs.GetInt("LevelIndex");
            else
            {
                PlayerPrefs.SetInt("LevelIndex", 0);
                LevelIndex =  PlayerPrefs.GetInt("LevelIndex");
            }
        }
        

        private void Win()
        {
            LevelIndex++;
            PlayerPrefs.SetInt("LevelIndex", LevelIndex);
        }
    }
}
