using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using TMPro;
using Unity.VisualScripting;

namespace SKC.Grid
{
    public class GridCell : MonoBehaviour
    {
        public char Letter { get; set; }
        public int Row { get; private set; }
        public int Col { get; private set; }

        public MeshRenderer _renderer;
        public TextMeshPro _textMesh;
        public Material[] selectedMaterials, unselectedMaterials;
        private bool _isSelected = false;

        public event Action<GridCell> OnCellSelected;
        public event Action<GridCell> OnCellDeselected;

        public void Initialize(char letter, int row, int col)
        {
            Letter = letter;
            Row = row;
            Col = col;
            _textMesh.text = letter.ToString();
            Deselect();
        }

        public void Select()
        {
            _isSelected = true;
            // when selected, change visual
            _renderer.sharedMaterials = selectedMaterials;
            OnCellSelected?.Invoke(this);
        }

        public void Deselect()
        {
            _isSelected = false;
            // when unselect, change visual to normal
            _renderer.sharedMaterials = unselectedMaterials;
            OnCellDeselected?.Invoke(this);
        }

        public void ResetCell()
        {
            _isSelected = false;
            // when unselect, change visual to normal
            _renderer.sharedMaterials = unselectedMaterials;
        }

        private void OnMouseDown()
        {
            Select();
        }

        private void OnMouseUp()
        {
            Deselect();
        }

        private void OnMouseEnter()
        {
            if (Input.GetMouseButton(0)) // if player click
            {
                Select();
            }
        }
    }
}
