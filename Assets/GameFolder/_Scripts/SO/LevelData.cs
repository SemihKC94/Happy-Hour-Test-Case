using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SKC.Level
{
    [CreateAssetMenu(fileName = nameof(LevelData), menuName = nameof(SKC) + "/" + nameof(Level) + "/" + nameof(LevelData), order = 1)]
    public class LevelData : ScriptableObject
    {
        public string LevelTitle;
        public Sprite LevelImage; 
        [Tooltip("Assign word json")]public TextAsset WordListJson; // Aassign the JSON file from the Unity Editor
        public int GridWidth;
        public int GridHeight;
        public int TargetWordCount;
        public Vector3 CameraPosition {get {return new Vector3(GridWidth - 1, -GridHeight + 1, -10);}}

        public float CameraZoom()
        {
            return 6 + GridWidth + 2;
        }
    }
}
