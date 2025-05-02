using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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
}
