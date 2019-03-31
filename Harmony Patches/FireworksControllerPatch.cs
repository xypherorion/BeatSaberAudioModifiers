using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using Harmony;
using UnityEngine;

namespace AudioModifiers {
    [HarmonyPatch(typeof(FireworkItemController))]
    [HarmonyPatch("PlayExplosionSound", MethodType.Normal)]
    public class FireworksControllerPatch {
        public static bool Prefix(ref FireworkItemController __instance) {
            return !AudioModifiersPlugin.cfg.DisableFireworks;
        }
    }
}