using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AudioModifiers {
    public class ModConfiguration {
        public bool DisableFireworks = true;
        public bool EnableCustomSounds = true;
        public bool UseRandomPitch = true;

        public static ModConfiguration FromJson(string json) {
            return JsonUtility.FromJson<ModConfiguration>(json);
        }

        public override string ToString() {
            return JsonUtility.ToJson(this);
        }
    }
}
