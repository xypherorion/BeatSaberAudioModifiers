using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using Harmony;
using UnityEngine;

namespace AudioModifiers {

    [HarmonyPatch(typeof(NoteCutSoundEffect))]
    [HarmonyPatch("NoteWasCut", MethodType.Normal)]
    public class HitSoundsPatch {

        public static bool Prefix(ref NoteCutSoundEffect __instance, NoteController noteController, NoteCutInfo noteCutInfo, ref NoteData ____noteData, ref bool ____noteWasCut, ref bool ____goodCut, ref bool ____handleWrongSaberTypeAsGood, ref AudioSource ____audioSource, ref double ____endDSPtime, ref float ____badCutVolume, ref float ____goodCutVolume) {
            if (AudioModifiersPlugin.cfg.EnableCustomSounds) {
                if (noteController.noteData.id != ____noteData.id)
                    return false;

                ____noteWasCut = true;
                if ((!____handleWrongSaberTypeAsGood && !noteCutInfo.allIsOK) || (____handleWrongSaberTypeAsGood && (!noteCutInfo.allExceptSaberTypeIsOK || noteCutInfo.saberTypeOK))) {
                    AudioClip clip = AudioModifiersPlugin.MissSoundPicker.PickRandomObject();

                    //Plugin.Log("Selected Miss Clip " + clip.name);
                    ____audioSource.clip = clip;
                    ____audioSource.time = 0f;
                    ____audioSource.Play();
                    ____goodCut = false;
                    ____audioSource.volume = ____badCutVolume;
                    ____endDSPtime = AudioSettings.dspTime + (double)____audioSource.clip.length + 0.10000000149011612;
                } else {
                    AudioClip clip = AudioModifiersPlugin.HitSoundPicker.PickRandomObject();
                    //Plugin.Log("Selected Hit Clip " + clip.name);
                    ____audioSource.clip = clip;
                    ____audioSource.time = 0f;
                    ____audioSource.Play();

                    if (AudioModifiersPlugin.cfg.UseRandomPitch)
                        ____audioSource.pitch = UnityEngine.Random.Range(0.9f, 1.2f);
                    ____goodCut = true;
                    ____audioSource.volume = ____goodCutVolume;
                }
                __instance.transform.position = noteCutInfo.cutPoint;

                return false;
            }
            return true;
        }
    }
}