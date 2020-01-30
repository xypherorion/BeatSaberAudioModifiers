using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AudioModifiers {
    public class ModConfiguration {
        //NOTE: Reminder that parsing Vector3 or Vector2 requires an extension

        public bool EnableCustomSounds = true;
        public bool DisableFireworks = true;
        public bool LeftWhoosh = true;
        public float LeftVolume = 1.0f;
        public bool RightWhoosh = true;
        public float RightVolume = 1.0f;
        public float PitchRange = 0.15f;
        public float SpeedMultiplier = 1.0f; //0.05f
        public float SpeedupSmoothing = 1.0f; //4.0f
        public float SlowdownSmoothing = 1.0f; //4.0f
        public bool ClashFX = true;
        public float ClashVolume = 0.85f;
        public float minMusicTime = 0.0f;
        public bool showDebugSpheres = false;

        public static ModConfiguration FromJson(string json) {
            return JsonUtility.FromJson<ModConfiguration>(json);
        }

        public override string ToString() {
            return JsonUtility.ToJson(this);
        }
    }
}
