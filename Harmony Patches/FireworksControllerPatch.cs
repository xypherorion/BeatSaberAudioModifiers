using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;

namespace AudioModifiers {
    [HarmonyPatch(typeof(FireworkItemController))]
    [HarmonyPatch("PlayExplosionSound", MethodType.Normal)]
    public class FireworksControllerExplosionSoundPatch {
        public static bool Prefix(ref FireworkItemController __instance) {
            return !AudioMod.cfg.DisableFireworks;
        }
    }

    [HarmonyPatch(typeof(FireworkItemController))]
    [HarmonyPatch("Awake", MethodType.Normal)]
    public class FireworksControllerAwakePatch {
        public static bool Prefix(ref FireworkItemController __instance, ref RandomObjectPicker<AudioClip> ____randomAudioPicker) {
            if (AudioMod.FireworksFX.Count > 0) {
                ____randomAudioPicker = AudioMod.FireworkSFXPicker;
                return false;
            }
            return true;
        }
    }
}