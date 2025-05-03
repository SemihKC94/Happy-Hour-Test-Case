using System;
using System.Collections;
using System.Collections.Generic;
using SKC.Grid;
using SKC.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace SKC.Level
{
    public class LevelInitialize : MonoBehaviour
    {
        [SerializeField] private GridManager gridManager;
        [SerializeField] private WordManager  wordManager;
        [SerializeField] private UIManager uiManager;
        [SerializeField] private Camera mainCamera;
        
        private LevelData levelData;

        public void InitializeLevel(LevelData levelData, int levelIndex)
        {
            this.levelData = levelData;
            
            uiManager.Initialize(this.levelData, levelIndex);
            wordManager.Initialize(this.levelData.WordListJson);
            gridManager.InitializeGrid(this.levelData.GridWidth, levelData.GridHeight);
            mainCamera.transform.position = this.levelData.CameraPosition;
            mainCamera.orthographicSize = this.levelData.CameraZoom();
        }
    }
}
