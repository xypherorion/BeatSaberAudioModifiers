using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using UnityEngine;

namespace AudioModifiers.Harmony_Patches {
    class SongPreviewPlayerPatch {
        [HarmonyPatch(typeof(SongPreviewPlayer))]
        [HarmonyPatch("CrossfadeToDefault", MethodType.Normal)]
        public class SongPreviewPlayerCrossfadeToDefaultPatch {
            public static bool Prefix(ref SongPreviewPlayer __instance, ref AudioClip ____defaultAudioClip) {
                if(AudioModifiersPlugin.BGMusic.Count > 0)
                    ____defaultAudioClip = AudioModifiersPlugin.BGMusicPicker.PickRandomObject();
                return true;
            }
        }
    }
}
