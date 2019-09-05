using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using UnityEngine;
using UnityEngine.XR;

namespace AudioModifiers.Harmony_Patches {
    class SaberClashEffectPatch {
        [HarmonyPatch(typeof(SaberClashEffect))]
        [HarmonyPatch("LateUpdate", MethodType.Normal)]
        public virtual void SaberClashEffectLateUpdatePatch(ref SaberClashEffect __instance, ref SaberClashChecker ____saberClashChecker, ref HapticFeedbackController ____hapticFeedbackController, ref bool ____sabersAreClashing, ref ParticleSystem.EmissionModule ____sparkleParticleSystemEmmisionModule, ref ParticleSystem.EmissionModule ____glowParticleSystemEmmisionModule, ref ParticleSystem ____glowParticleSystem) {
            if (____saberClashChecker.sabersAreClashing) {
                __instance.transform.position = ____saberClashChecker.clashingPoint;
                ____hapticFeedbackController.ContinuousRumble(XRNode.LeftHand);
                ____hapticFeedbackController.ContinuousRumble(XRNode.RightHand);

                if (!____sabersAreClashing) {
                    ____sabersAreClashing = true;
                    ____sparkleParticleSystemEmmisionModule.enabled = true;
                    ____glowParticleSystemEmmisionModule.enabled = true;


                    if (AudioModifiersPlugin.cfg.ClashFX && AudioModifiersPlugin.ClashSource != null) {
                        if (!AudioModifiersPlugin.ClashSource.isPlaying) {
                            //Play random clash sound if previous has finished playing
                            if (AudioModifiersPlugin.SaberClashFX.Count > 0) {
                                AudioModifiersPlugin.ClashSource.loop = false;
                                AudioModifiersPlugin.ClashSource.spatialize = true;
                                AudioModifiersPlugin.ClashSource.spatialBlend = 1.0f;
                                AudioModifiersPlugin.ClashSource.bypassEffects = true;
                                AudioModifiersPlugin.ClashSource.volume = AudioModifiersPlugin.cfg.ClashVolume;
                                AudioModifiersPlugin.ClashSource.transform.position = ____saberClashChecker.clashingPoint;
                                AudioModifiersPlugin.ClashSource.clip = AudioModifiersPlugin.SaberClashPicker.PickRandomObject();
                                AudioModifiersPlugin.ClashSource.priority = 32;
                                AudioModifiersPlugin.ClashSource.rolloffMode = AudioRolloffMode.Logarithmic;
                                if (AudioModifiersPlugin.ClashSource.clip != null) {
                                    AudioModifiersPlugin.ClashSource.Play();
                                    AudioModifiersPlugin.Log("Set Clash SFX to " + AudioModifiersPlugin.ClashSource.clip.name);
                                } else {

                                    AudioModifiersPlugin.Log("Set Clash SFX to ... well ... nothing.");
                                }
                            }
                        }
                    }
                }
            } else if (____sabersAreClashing) {
                ____sabersAreClashing = false;
                ____sparkleParticleSystemEmmisionModule.enabled = false;
                ____glowParticleSystemEmmisionModule.enabled = false;
                ____glowParticleSystem.Clear();
            }
        }

        /*
        public virtual void SaberClashEffectUpdatePatch(ref SaberClashEffect __instance, ref bool ____sabersAreClashing, ref SaberClashChecker ____saberClashChecker, ref HapticFeedbackController ____hapticFeedbackController, ref ParticleSystem.EmissionModule ____sparkleParticleSystemEmmisionModule, ref ParticleSystem.EmissionModule ____glowParticleSystemEmmisionModule, ref ParticleSystem ____glowParticleSystem) {
            if (____saberClashChecker.sabersAreClashing) {
                __instance.transform.position = ____saberClashChecker.clashingPoint;
                ____hapticFeedbackController.ContinuousRumble(XRNode.LeftHand);
                ____hapticFeedbackController.ContinuousRumble(XRNode.RightHand);

                if (!____sabersAreClashing) {
                    ____sabersAreClashing = true;
                    ____sparkleParticleSystemEmmisionModule.enabled = true;
                    ____glowParticleSystemEmmisionModule.enabled = true;

                    if (AudioModifiersPlugin.cfg.ClashFX && AudioModifiersPlugin.ClashSource != null) {
                        if (!AudioModifiersPlugin.ClashSource.isPlaying) {
                            //Play random clash sound if previous has finished playing
                            if (AudioModifiersPlugin.SaberClashFX.Count > 0) {
                                AudioModifiersPlugin.ClashSource.loop = false;
                                AudioModifiersPlugin.ClashSource.spatialize = true;
                                AudioModifiersPlugin.ClashSource.spatialBlend = 1.0f;
                                AudioModifiersPlugin.ClashSource.bypassEffects = true;
                                AudioModifiersPlugin.ClashSource.volume = AudioModifiersPlugin.cfg.ClashVolume;
                                AudioModifiersPlugin.ClashSource.transform.position = ____saberClashChecker.clashingPoint;
                                AudioModifiersPlugin.ClashSource.clip = AudioModifiersPlugin.SaberClashPicker.PickRandomObject();
                                AudioModifiersPlugin.ClashSource.priority = 32;
                                AudioModifiersPlugin.ClashSource.rolloffMode = AudioRolloffMode.Logarithmic;
                                if (AudioModifiersPlugin.ClashSource.clip != null) {
                                    AudioModifiersPlugin.ClashSource.Play();
                                    AudioModifiersPlugin.Log("Set Clash SFX to " + AudioModifiersPlugin.ClashSource.clip.name);
                                } else {

                                    AudioModifiersPlugin.Log("Set Clash SFX to ... well ... nothing.");
                                }
                            }
                        }
                    }
                }
            } else if (____sabersAreClashing) {
                ____sabersAreClashing = false;
                ____sparkleParticleSystemEmmisionModule.enabled = false;
                ____glowParticleSystemEmmisionModule.enabled = false;
                ____glowParticleSystem.Clear();
            }
        }
        */
    }
}
