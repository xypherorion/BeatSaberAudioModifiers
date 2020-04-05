using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;

namespace AudioModifiers.Harmony_Patches {
    [HarmonyPatch(typeof(SongPreviewPlayer))]
    [HarmonyPatch("CrossfadeToDefault", MethodType.Normal)]
    public class SongPreviewPlayerCrossfadeToDefaultPatch {
        public static bool Prefix(ref SongPreviewPlayer __instance, ref AudioClip ____defaultAudioClip) {
            if (AudioMod.cfg.EnableCustomSounds &&
                (AudioMod.BGMusic.Count > 0)) {
                ____defaultAudioClip = AudioMod.BGMusicPicker.PickRandomObject();
            }
            return true;
        }
    }
}
