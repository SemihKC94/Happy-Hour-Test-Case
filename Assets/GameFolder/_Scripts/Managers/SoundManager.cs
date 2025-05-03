using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace SKC.Managers
{
    public class SoundManager : MonoBehaviour
    {
        [SerializeField] private AudioSource _blobSource;
        [SerializeField] private AudioSource _winSource;
        [SerializeField] private float _defaultPitchValue = .5f;
        
        // private 
        private float _pitchUpdate = .05f;
        private int _playCount = 0;
        
        public void PlayBlob()
        {
            if(_blobSource.isPlaying) _blobSource.Stop();
            _playCount++;
            _blobSource.pitch = _defaultPitchValue + (_playCount * _pitchUpdate);
            _blobSource.Play();
        }
        
        public void PlayWin()
        {
            _winSource.Play();
        }

        public void Reset()
        {
            _playCount = 0;
        }
    }
}
