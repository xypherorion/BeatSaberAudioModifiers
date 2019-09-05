using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioModifiers {
    using System;
    using UnityEngine;

    /// <summary>
    /// Borrowed from Beat Saber Old Build
    /// Thanks Jan and Hrincar
    /// </summary>
    public class SwordSound : MonoBehaviour {
        public Transform _swordPoint;
        public AudioSource _audioSource;
        public AnimationCurve _pitchBySpeedCurve;
        public AnimationCurve _gainBySpeedCurve;
        public float _speedMultiplier = 1.0f; //0.05f
        public float _upSmooth = 1.0f;
        public float _downSmooth = 1.0f;
        public Vector3 _prevPoint = default(Vector3);
        public float _speed = 0.0f;
        private float targetSpeed;

        private void Awake() {
            //Check.Null(this._swordPoint);
            //Check.Null(this._audioSource);
        }

        void Update() {
            if (_swordPoint != null) {
                targetSpeed = (Time.deltaTime == 0f) ? 
                    0f :
                    _speedMultiplier * Vector3.Distance(_swordPoint.position, _prevPoint) / Time.deltaTime;
                
                _speed = (targetSpeed < _speed) ? 
                    Mathf.Clamp01(Mathf.Lerp(_speed, targetSpeed, Time.deltaTime * _downSmooth)) :
                    Mathf.Clamp01(Mathf.Lerp(_speed, targetSpeed, Time.deltaTime * _upSmooth));

                if (_audioSource != null) {
                    _audioSource.pitch = _pitchBySpeedCurve.Evaluate(_speed);
                    _audioSource.volume = _gainBySpeedCurve.Evaluate(_speed);
                } else
                    AudioModifiersPlugin.Log("Houstin, where the hell is my audio source!?");

                _prevPoint = _swordPoint.position;
            } else
                AudioModifiersPlugin.Log("Wait a second...this sword isn't pointy!");
        }
    }
}
