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
        private void Awake() {
            //Check.Null(this._swordPoint);
            //Check.Null(this._audioSource);
        }

        private float targetSpeed;
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
                    AudioModifiersPlugin.Log("Houstin, where the is my audio source!?");

                _prevPoint = _swordPoint.position;
            }
        }

        [SerializeField]
        public Transform _swordPoint;

        [SerializeField]
        public AudioSource _audioSource;

        [SerializeField]
        public AnimationCurve _pitchBySpeedCurve;

        [SerializeField]
        public AnimationCurve _gainBySpeedCurve;

        [SerializeField]
        public float _speedMultiplier = 0.05f;

        [SerializeField]
        public float _upSmooth = 4f;

        [SerializeField]
        public float _downSmooth = 4f;

        public Vector3 _prevPoint = default(Vector3);

        public float _speed = 0.0f;
    }

}
