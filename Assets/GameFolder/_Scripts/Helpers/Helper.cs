using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace SKC.Helpers
{
    public static class Helper
    {
        private static Camera _camera;
        public static Camera MainCamera
        {
            get
            {
                if (_camera == null) _camera = Camera.main;
                return _camera;
            }
        }

        private static readonly Dictionary<float, WaitForSeconds> WaitDictionary = new Dictionary<float, WaitForSeconds>();

        public static WaitForSeconds GetWait(float time)
        {
            if (WaitDictionary.TryGetValue(time, out var wait)) return wait;

            WaitDictionary[time] = new WaitForSeconds(time);
            return WaitDictionary[time];
        }

        private static PointerEventData _eventDataCurrentPosition;
        private static List<RaycastResult> _resuts;
        public static bool IsOverUI()
        {
            _eventDataCurrentPosition = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
            _resuts = new List<RaycastResult>();
            EventSystem.current.RaycastAll(_eventDataCurrentPosition, _resuts);
            return _resuts.Count > 0;
        }
        
        public static Vector2 GetWorldPositionOfCanvasElement(RectTransform element)
        {
            RectTransformUtility.ScreenPointToWorldPointInRectangle(element, element.position, MainCamera, out var result);
            return result;
        }
        
        public static void DeleteChildren(this Transform t)
        {
            foreach (Transform child in t) Object.Destroy(child.gameObject);
        }

        public static string GetDeviceId
        {
            get
            {
                string id = "unknown";
                if (Application.platform == RuntimePlatform.Android ||
                    Application.platform == RuntimePlatform.IPhonePlayer)
                {
#if UNITY_ANDROID
                    AndroidJavaClass clsUnity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                    AndroidJavaObject objActivity = clsUnity.GetStatic<AndroidJavaObject>("currentActivity");
                    AndroidJavaObject objResolver = objActivity.Call<AndroidJavaObject>("getContentResolver");
                    AndroidJavaClass clsSecure = new AndroidJavaClass("android.provider.Settings$Secure");
                    id = clsSecure.CallStatic<string>("getString", objResolver, "android_id");

#endif
#if UNITY_IPHONE
                id = UnityEngine.iOS.Device.vendorIdentifier;
#endif
                }
                else
                {
                    id = SystemInfo.deviceUniqueIdentifier;
                }

                return id;
            }
        }

        public static string NetworkStatus
        {
            get
            {
                string networkStatus = "";
                if (Application.internetReachability == NetworkReachability.NotReachable)
                {
                    networkStatus = "Not Reachable.";
                }
                else if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
                {
                    networkStatus = "Reachable via carrier data network.";
                }
                else if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
                {
                    networkStatus = "Reachable via Local Area Network.";
                }

                return networkStatus;
            }
        }

        public static bool ConnectionStatus
        {
            get
            {
                bool networkStatus = false;
                if (Application.internetReachability == NetworkReachability.NotReachable)
                {
                    networkStatus = false;
                }
                else if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
                {
                    networkStatus = true;
                }
                else if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
                {
                    networkStatus = true;
                }

                return networkStatus;
            }
        }
        
        public static void Shuffle<T>(this IList<T> ts) 
        {
            var count = ts.Count;
            var last = count - 1;
            for (var i = 0; i < last; ++i) {
                var r = UnityEngine.Random.Range(i, count);
                var tmp = ts[i];
                ts[i] = ts[r];
                ts[r] = tmp;
            }
        }
        
        public static void ShuffleVariant<T>(this IList<T> ts) 
        {
            var count = ts.Count;
            var last = count - 1;
            int random = Random.Range(2, Math.Min(5, count));
            int index = 0;

            while (index < random)
            {
                var r1 = UnityEngine.Random.Range(0, count);
                var r2 = UnityEngine.Random.Range(0, count);
                var tmp = ts[r1];
            
                ts[r1] = ts[r2];
                ts[r2] = tmp;
                index++;
            }
        }
        
        public static void ShuffleSmallGroups<T>(this IList<T> ts, int groupSize)
        {
            if (groupSize <= 1 || ts.Count <= groupSize) return;

            for (int i = 0; i < ts.Count; i += groupSize)
            {
                var group = ts.Skip(i).Take(groupSize).ToList();
                group.Shuffle();

                for (int j = 0; j < group.Count; j++)
                {
                    if (i + j < ts.Count)
                    {
                        ts[i + j] = group[j];
                    }
                }
            }
        }

        public static void SwapBlocksInList<T>(this List<T> list, int minBlockSize, int maxBlockSize)
        {
            if (list == null || list.Count < 2)
            {
                DebugError("Letter list is empty or does not have enough elements!");
                return;
            }

            int listSize = list.Count;

            // Select the start and end indices for the first random block
            int startIndex1 = Random.Range(0, listSize);
            int blockSize1 = Random.Range(minBlockSize, Mathf.Min(maxBlockSize + 1, listSize - startIndex1 + 1));
            int endIndex1 = startIndex1 + blockSize1 - 1;

            // Select the start and end indices for the second random block (making sure it doesn't overlap and has the same size)
            int startIndex2 = Random.Range(0, listSize);
            int blockSize2 = Random.Range(minBlockSize, Mathf.Min(maxBlockSize + 1, listSize - startIndex2 + 1));
            int endIndex2 = startIndex2 + blockSize2 - 1;

            // Check if the blocks overlap or have different sizes
            // Overlap check is simplified and can be improved for more complex scenarios.
            if (startIndex1 <= endIndex2 && endIndex1 >= startIndex2 || blockSize1 != blockSize2)
            {
                DebugError("Selected blocks overlap or have different sizes. You can try again.");
                return;
            }

            List<T> block1 = list.GetRange(startIndex1, blockSize1);
            List<T> block2 = list.GetRange(startIndex2, blockSize2);

            // Place the second block in the position of the first block
            list.RemoveRange(startIndex1, blockSize1);
            list.InsertRange(startIndex1, block2);

            // Place the first block in the original position of the second block (considering shifts)
            int secondBlockStartAfterSwap = startIndex2;
            if (startIndex2 > startIndex1)
            {
                secondBlockStartAfterSwap -= blockSize1;
            }
            else if (startIndex1 > startIndex2)
            {
                secondBlockStartAfterSwap += blockSize2;
            }

            list.RemoveRange(secondBlockStartAfterSwap, blockSize2);
            list.InsertRange(secondBlockStartAfterSwap, block1);
        }
        
        public static void Debug(string debug, bool wantInBuild = false)
        {
            if(wantInBuild)
            {
                UnityEngine.Debug.Log(debug);
                return;
            }

#if UNITY_EDITOR
            UnityEngine.Debug.Log(debug);
#endif
        }

        public static void DebugError(string debug, bool wantInBuild = false)
        {
            if (wantInBuild)
            {
                UnityEngine.Debug.LogError(debug);
                return;
            }

#if UNITY_EDITOR
            UnityEngine.Debug.LogError(debug);
#endif
        }

        public static void DebugWarning(string debug, bool wantInBuild = false)
        {
            if (wantInBuild)
            {
                UnityEngine.Debug.LogWarning(debug);
                return;
            }

#if UNITY_EDITOR
            UnityEngine.Debug.LogWarning(debug);
#endif
        }
    }
    
    [System.Serializable]
    public class SKCCoroutine
    {
        CancellationToken _token;
        CancellationTokenSource _cancellationTokenSource;
        float _duration;
        //Action _action1;
        //Action<T> _action2;
        //T _value;

        public SKCCoroutine(float duration)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _token = _cancellationTokenSource.Token;
            _duration = duration;
        }
        #region Run Action With No Paramaters
        public void Run(Action action)
        {
            TimeStepsWithNoParams(action);
        }
        async void TimeStepsWithNoParams(Action action)
        {
            await RunTimeStepsTaskWithNoParams(action);
        }
        async Task RunTimeStepsTaskWithNoParams(Action action)
        {
            if (_token.IsCancellationRequested || !Application.isPlaying || _cancellationTokenSource.IsCancellationRequested)
                return;

            try
            {
                _token.ThrowIfCancellationRequested();
                await Task.Delay(TimeSpan.FromSeconds(_duration), _token);
                if (action != null && Application.isPlaying)
                    action();
            }
            catch (TaskCanceledException)
            { }
        }
        #endregion
        #region Run Action With Paramaters
        public void Run<T>(Action<T> action, T value)
        {
            TimeStepsWithParams(action, value);
        }
        async void TimeStepsWithParams<T>(Action<T> action, T value)
        {
            await RunTimeStepsTaskWithParams(action, value);
        }
        async Task RunTimeStepsTaskWithParams<T>(Action<T> action, T value)
        {
            if (_token.IsCancellationRequested || !Application.isPlaying || _cancellationTokenSource.IsCancellationRequested)
                return;

            try
            {
                _token.ThrowIfCancellationRequested();
                await Task.Delay(TimeSpan.FromSeconds(_duration), _token);
                if (action != null && Application.isPlaying)
                    action(value);
            }
            catch (TaskCanceledException)
            { }
        }
        #endregion
        public void Stop()
        {
            if (_cancellationTokenSource != null/* && _cancellationTokenSource.Token.CanBeCanceled*/)
                _cancellationTokenSource.Cancel();
        }
    }
    public static class Timer
    {

        public static bool TimeCheck()
        {
            return false;
        }
        public static void Destroy(this SKCCoroutine skcCoroutine)
        {
            skcCoroutine.Stop();
            skcCoroutine = null;
        }
        public static SKCCoroutine RunAfter(float duration, Action action)
        {
            if (!Application.isPlaying)
                return null;

            SKCCoroutine skcCoroutine = new SKCCoroutine(duration);
            skcCoroutine.Run(action);
            return skcCoroutine;
        }
        public static SKCCoroutine RunAfter<T>(float duration, Action<T> action, T value)
        {
            if (!Application.isPlaying)
                return null;

            SKCCoroutine skcCoroutine = new SKCCoroutine(duration);
            skcCoroutine.Run(action, value);
            return skcCoroutine;
        }
        public static IEnumerator TimeSteps(float totalTime, float stepTime, Action onStart, Action<float> onUpdate, Action onComplete)
        {
            var elapsed = 0.0f;

            if (onStart != null)
                onStart();

            while (elapsed <= totalTime)
            {

                elapsed += stepTime;
                if (onUpdate != null)
                    onUpdate(elapsed);

                yield return new WaitForSeconds(stepTime);
            }

            if (onComplete != null)
                onComplete();
        }
        public static IEnumerator TimeSteps(float totalTime, Action onComplete)
        {
            yield return new WaitForSeconds(totalTime);

            if (onComplete != null)
                onComplete();
        }
        public static IEnumerator TimeSteps(float totalTime, Action<int> onComplete, int data)
        {
            yield return new WaitForSeconds(totalTime);

            if (onComplete != null)
                onComplete(data);
        }
        public static IEnumerator TimeSteps<T>(float totalTime, Action<T> onComplete, T data)
        {
            yield return new WaitForSeconds(totalTime);

            if (onComplete != null)
                onComplete(data);
        }
    }
}
