using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;

namespace AudioModifiers {

    [HarmonyPatch(typeof(NoteCutSoundEffect))]
    [HarmonyPatch("NoteWasCut", MethodType.Normal)]
    public class HitSoundsPatch {
        public static bool Prefix(ref NoteCutSoundEffect __instance, NoteController noteController, NoteCutInfo noteCutInfo, ref RandomObjectPicker<AudioClip> ____badCutRandomSoundPicker, ref NoteData ____noteData, ref bool ____noteWasCut, ref bool ____goodCut, ref bool ____handleWrongSaberTypeAsGood, ref AudioSource ____audioSource, ref double ____endDSPtime, ref float ____badCutVolume, ref float ____goodCutVolume) {
            if (!AudioMod.cfg.EnableCustomSounds)
                return true;

            if(AudioMod.MissSoundPicker != null && AudioMod.HitSoundPicker != null) {
                if (noteController.noteData.id != ____noteData.id)
                    return false;

                ____noteWasCut = true;

                if ((!____handleWrongSaberTypeAsGood && !noteCutInfo.allIsOK) ||
                    (____handleWrongSaberTypeAsGood && (!noteCutInfo.allExceptSaberTypeIsOK || noteCutInfo.saberTypeOK))) {
                    if (AudioMod.MissSounds.Count > 0)
                        ____audioSource.clip = AudioMod.MissSoundPicker.PickRandomObject();

                    ____audioSource.time = 0f;
                    ____audioSource.Play();
                    ____goodCut = false;
                    ____audioSource.volume = ____badCutVolume;
                    ____endDSPtime = AudioSettings.dspTime + (double)____audioSource.clip.length + 0.10000000149011612;
                } else {
                    //TODO: Detect hits and switch to different sound pickers
                    if (AudioMod.HitSounds.Count > 0)
                        ____audioSource.clip = AudioMod.HitSoundPicker.PickRandomObject();

                    ____audioSource.time = 0f;
                    //____endDSPtime = AudioSettings.dspTime + (double)____audioSource.clip.length + 0.10000000149011612;

                    ____audioSource.pitch = UnityEngine.Random.Range(1.0f - (AudioMod.cfg.PitchRange * 0.5f), 1.0f + (AudioMod.cfg.PitchRange * 0.5f));
                    ____goodCut = true;
                    ____audioSource.volume = ____goodCutVolume;
                    ____audioSource.Play();
                }
                __instance.transform.position = noteCutInfo.cutPoint;
            }
            return false;

            /*
            if (AudioModifiersPlugin.cfg.EnableCustomSounds) {
                if (noteController.noteData.id != ____noteData.id) {
                    return false;
                }
                ____noteWasCut = true;
                if ((!____handleWrongSaberTypeAsGood && !noteCutInfo.allIsOK) || (____handleWrongSaberTypeAsGood && (!noteCutInfo.allExceptSaberTypeIsOK || noteCutInfo.saberTypeOK))) {
                    if (AudioModifiersPlugin.MissSoundPicker != null) {
                        AudioClip clip = AudioModifiersPlugin.MissSoundPicker.PickRandomObject();
                        if (clip != null) {
                            ____audioSource.clip = clip;
                            ____audioSource.time = 0f;
                            ____audioSource.volume = ____badCutVolume;
                            ____audioSource.Play();
                            ____endDSPtime = AudioSettings.dspTime + (double)____audioSource.clip.length + 0.10000000149011612;
                        }
                         ____goodCut = false;
                    } else {
                        return true;
                    }
                } else {
                    if (AudioModifiersPlugin.HitSoundPicker != null) {
                        AudioClip clip = AudioModifiersPlugin.HitSoundPicker.PickRandomObject();
                        if (clip != null) {
                            ____audioSource.clip = clip;
                            ____audioSource.time = 0f;
                            ____audioSource.pitch = UnityEngine.Random.Range(AudioModifiersPlugin.cfg.PitchRange.x, AudioModifiersPlugin.cfg.PitchRange.y);
                            ____audioSource.volume = ____goodCutVolume;
                            ____audioSource.Play();
                        }
                        ____goodCut = true;
                    } else {
                        return true;
                    }
                }
                __instance.transform.position = noteCutInfo.cutPoint;
                return false;
            }
            return true;

            /* 0.13.x

            if (AudioModifiersPlugin.cfg.EnableCustomSounds) {
                if (noteController.noteData.id != ____noteData.id)
                    return false;

                ____noteWasCut = true;
                if ((!____handleWrongSaberTypeAsGood && !noteCutInfo.allIsOK) || (____handleWrongSaberTypeAsGood && (!noteCutInfo.allExceptSaberTypeIsOK || noteCutInfo.saberTypeOK))) {
                    if (AudioModifiersPlugin.MissSoundPicker != null) {
                        AudioClip clip = AudioModifiersPlugin.MissSoundPicker.PickRandomObject();

                        //Plugin.Log("Selected Miss Clip " + clip.name);
                        ____audioSource.clip = clip;
                        ____audioSource.time = 0f;
                        ____audioSource.volume = ____badCutVolume;
                        ____audioSource.Play();
                        ____goodCut = false;
                        ____endDSPtime = AudioSettings.dspTime + (double)____audioSource.clip.length + 0.10000000149011612;
                    } else {
                        AudioModifiersPlugin.Log("MissSoundPicker is missing");
                    }
                } else {
                    if (AudioModifiersPlugin.HitSoundPicker != null) {
                        AudioClip clip = AudioModifiersPlugin.HitSoundPicker.PickRandomObject();
                        //Plugin.Log("Selected Hit Clip " + clip.name);
                        ____audioSource.clip = clip;
                        ____audioSource.time = 0f;
                        ____audioSource.pitch = UnityEngine.Random.Range(AudioModifiersPlugin.cfg.PitchRange.x, AudioModifiersPlugin.cfg.PitchRange.y);
                        ____audioSource.volume = ____goodCutVolume;
                        ____audioSource.Play();

                        ____goodCut = true;
                    } else {
                        AudioModifiersPlugin.Log("HitSoundPicker is missing");
                    }
                }
                __instance.transform.position = noteCutInfo.cutPoint;

                return false;
            }
            return true;
            */
        }
    }
}