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
        public float PitchRange = 0.1f;
        public float SpeedMultiplier = 0.05f;
        public float SpeedupSmoothing = 4f;
        public float SlowdownSmoothing = 4f;
        public bool ClashFX = true;
        public float ClashVolume = 0.5f;
        public float minMusicTime = 0.0f;

        public static ModConfiguration FromJson(string json) {
            return JsonUtility.FromJson<ModConfiguration>(json);
        }

        public override string ToString() {
            return JsonUtility.ToJson(this);
        }
    }
}
