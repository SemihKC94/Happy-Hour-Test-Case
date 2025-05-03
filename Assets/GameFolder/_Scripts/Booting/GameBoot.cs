using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;
using System.Collections;
using SKC.Events;
using SKC.Helpers;
using SKC.Helpers.Enums;

namespace SKC.Boot
{
    public class GameBoot : MonoBehaviour
    {
        [SerializeField] private Image _progressBarImage;
        [SerializeField] private string _loadingLevelName;
        
        IEnumerator Start()
        {
            // Do later => Save System or 3pt tools (Ad, Analytics etc)
            yield return null;
            // Load scene async after save file restored
            AsyncOperation operation = SceneManager.LoadSceneAsync(_loadingLevelName);
            operation.allowSceneActivation = false;

            while (!operation.isDone)
            {
                _progressBarImage.fillAmount = operation.progress;

                if (operation.progress >= 0.9f)
                {
                    operation.allowSceneActivation = true;
                }

                yield return null;
            }
        }
    }
}
