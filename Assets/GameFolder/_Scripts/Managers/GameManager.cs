using System;
using System.Collections;
using System.Collections.Generic;
using SKC.Events;
using SKC.Helpers.Enums;
using UnityEngine;

namespace SKC.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            EventBroker.InvokeOnGameInitialize();
        }
    }
}
