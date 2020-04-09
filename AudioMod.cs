using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using IPA;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using HarmonyLib;

//TODO: Select Hit sounds based on score ranges
//Hit Score Visualizer for reference

namespace AudioModifiers {
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class AudioMod {
        public enum SoundCategory {
            Hit = 0,
            Miss,
            Music,
            Fireworks,
            Saber,
            Clash,
            Unknown
        }

        public const string Name = "AudioModifiers";
        public const string Version = "1.1.8.0";

        public bool writeLogOnExit = true;
        public static string logFilePath = $"./UserData/{Name}.log";
        public static string LogFileData = "";
        protected static string appFolder = ".\\";

        public static Harmony harmony = null;

        public static List<AudioClip> HitSounds = new List<AudioClip>();
        public static List<AudioClip> MissSounds = new List<AudioClip>();
        public static List<AudioClip> BGMusic = new List<AudioClip>();
        public static List<AudioClip> FireworksFX = new List<AudioClip>();
        public static List<AudioClip> SaberWhoosh = new List<AudioClip>();
        public static List<AudioClip> SaberClashFX = new List<AudioClip>();
        public static RandomObjectPicker<AudioClip> HitSoundPicker = null;
        public static RandomObjectPicker<AudioClip> MissSoundPicker = null;
        public static RandomObjectPicker<AudioClip> BGMusicPicker = null;
        public static RandomObjectPicker<AudioClip> FireworkSFXPicker = null;
        public static RandomObjectPicker<AudioClip> SaberClashPicker = null;

        public static AudioSource ClashSource = null;

        public static string cfgFilePath = $"./UserData/{Name}/{Name}.json";
        public static ModConfiguration cfg = null;

        public static void Log(string message) {
            Console.WriteLine("[AudioModifiers] {0}", message);
            LogFileData += message + '\n';
        }

        #region Harmony
        public static string strHarmonyInstance = "com.XypherOrion.AudioModifiers.BeatSaber";

        public static void ApplyPatches() {
            bool success = true;
            if (harmony != null) {
                Log("Applying Harmony Patches");
                try {
                    harmony.PatchAll(System.Reflection.Assembly.GetExecutingAssembly());
                } catch (Exception ex) {
                    Log(ex.ToString());
                    success = false;
                }

            } else {
                Log("Harmony has not been initialized...");
                success = false;
            }

            if (success)
                Log("Harmony Patches Successful");
            else
                Log("Harmony Patches FAILED");

            appFolder = Application.dataPath + "/";
        }

        public static void RemovePatches() {
            if(harmony.GetPatchedMethods().Count() > 0) {
                harmony.UnpatchAll(strHarmonyInstance);
            }
        }
        #endregion

        public static void WriteAllGameObjectsToFile() {
            List<GameObject> pss = GameObject.FindObjectsOfType<GameObject>().ToList();
            Log("Dumping Game Object Names:");
            string GameObjectNames = "";
            foreach (GameObject ps in pss) {
                Log(ps.name);
                GameObjectNames += ps.name + '\n';
            }
            File.WriteAllText("./GameObjects.txt", GameObjectNames);
        }

        AudioType GetAudioType(string ext) {
            ext = ext.ToLower();
            switch (ext) {
                case "wav":
                    return AudioType.WAV;
                case "ogg":
                    return AudioType.OGGVORBIS;
                case "mp3":
                    return AudioType.MPEG;
                default:
                    return AudioType.UNKNOWN;
            }
        }

        protected bool LoadSounds(string[] ext, SoundCategory category) {
            int f;
            string[] AudioFiles;
            AudioClip clip;
            UnityWebRequest webRequest = null;
            string kwd = "";
            List<AudioClip> ClipCategory = null;

            switch (category) {
                case SoundCategory.Hit:
                    kwd = "hits";
                    ClipCategory = HitSounds;
                    break;
                case SoundCategory.Miss:
                    kwd = "misses";
                    ClipCategory = MissSounds;
                    break;
                case SoundCategory.Music:
                    kwd = "music";
                    ClipCategory = BGMusic;
                    break;
                case SoundCategory.Fireworks:
                    kwd = "fireworks";
                    ClipCategory = FireworksFX;
                    break;
                case SoundCategory.Saber:
                    kwd = "saber";
                    ClipCategory = SaberWhoosh;
                    break;
                case SoundCategory.Clash:
                    kwd = "clash";
                    ClipCategory = SaberClashFX;
                    break;
                case SoundCategory.Unknown:
                    ClipCategory = new List<AudioClip>();
                    break;
            }

            string[] dirs = Directory.GetDirectories("CustomAudio");

            if (dirs.Length > 0) {
                string dir = "";
                for (int d = 0; d < dirs.Length; d++) {
                    dir = dirs[d].ToLower() + "\\";
                    if (dir.Contains(kwd)) {
                        for (int t = 0; t < ext.Length; t++) {
                            //AudioFiles = Directory.GetFiles("CustomHits", searchStr + "." + ext[t] + "*");
                            AudioFiles = Directory.GetFiles(dir, "*." + ext[t]);
                            if (AudioFiles.Length > 0) {
                                for (f = 0; f < AudioFiles.Length; f++) {
                                    webRequest = UnityWebRequestMultimedia.GetAudioClip("file://" + appFolder + "..\\" + AudioFiles[f], GetAudioType(ext[t]));
                                    webRequest.SendWebRequest();
                                    while (!webRequest.isDone) { }

                                    if (webRequest.isHttpError || webRequest.isHttpError) {
                                        Log("Unable to add " + AudioFiles[f] + " to " + category.ToString() + " Sounds");
                                    } else {
                                        clip = DownloadHandlerAudioClip.GetContent(webRequest);
                                        if (clip == null) {
                                            Log(AudioFiles[f] + " Clip is NULL");
                                            continue;
                                        } else if (!clip.LoadAudioData()) {
                                            Log("Unable to Load clip audio data " + clip.name);
                                        } else {
                                            clip.name = AudioFiles[f];
                                            ClipCategory.Add(clip);
                                            Log("Added " + AudioFiles[f] + " to " + category.ToString() + " Sounds");
                                        }
                                    }
                                }
                            } else
                                Log("No " + dir + "*." + ext[t] + " Files");
                        }
                    }
                }
            } else {
                Log("No Custom Audio Folders exist");
            }

            return ClipCategory.Count > 0;
        }

        public void LoadCustomSounds() {
            if (!Directory.Exists("CustomAudio")) 
                Log("CustomAudio folders do not exist! Creating.");
                Directory.CreateDirectory("./CustomAudio");
            if (!Directory.Exists("CustomAudio/Hits"))
                Directory.CreateDirectory("./CustomAudio/Hits");
            if (!Directory.Exists("CustomAudio/Misses"))
                Directory.CreateDirectory("./CustomAudio/Misses");
            if (!Directory.Exists("CustomAudio/Music"))
                Directory.CreateDirectory("./CustomAudio/Music");
            if (!Directory.Exists("CustomAudio/Fireworks"))
                Directory.CreateDirectory("./CustomAudio/Fireworks");
            if (!Directory.Exists("CustomAudio/Saber"))
                Directory.CreateDirectory("./CustomAudio/Saber");

            //TODO: Accuracy Categories
            // eg: 115_ hits assign SuperHit.ogg, 80-100 hits assign GoodHit.ogg, etc
            Log("Reading Custom Hit Sounds");

            string[] exts = { "ogg", "wav", "mp3" };

            if (!LoadSounds(exts, SoundCategory.Hit))
                Log("No Hit SFX found");
            else
                Log("Loaded Hit SFX");
            if (!LoadSounds(exts, SoundCategory.Miss))
                Log("No Miss SFX found");
            else
                Log("Loaded Miss SFX");
            if (!LoadSounds(exts, SoundCategory.Music))
                Log("No Music found");
            else
                Log("Loaded Music");
            if (!LoadSounds(exts, SoundCategory.Fireworks))
                Log("No Fireworks SFX found");
            else
                Log("Loaded Fireworks SFX");
            if (!LoadSounds(exts, SoundCategory.Saber))
                Log("No Saber SFX found");
            else
                Log("Loaded Saber SFX");
            if (!LoadSounds(exts, SoundCategory.Clash))
                Log("No Saber Clash FX found");
            else
                Log("Loaded Saber Clash SFX");

            HitSoundPicker = new RandomObjectPicker<AudioClip>(HitSounds.ToArray(), 0.0f);
            MissSoundPicker = new RandomObjectPicker<AudioClip>(MissSounds.ToArray(), 0.0f);
            BGMusicPicker = new RandomObjectPicker<AudioClip>(BGMusic.ToArray(), cfg.minMusicTime);
            FireworkSFXPicker = new RandomObjectPicker<AudioClip>(FireworksFX.ToArray(), 0.0f);
            SaberClashPicker = new RandomObjectPicker<AudioClip>(SaberClashFX.ToArray(), 0.0f);
        }


        protected static void LoadModConfiguration() {
            if (!Directory.Exists($"./UserData/{Name}")) {
                Log("Creating Config Folder");
                Directory.CreateDirectory($"./UserData/{Name}");
            }

            if (!File.Exists(cfgFilePath)) {
                Log("Writing default Audio Modifiers Configuration");

                if (AudioMod.cfg == null)
                    AudioMod.cfg = new ModConfiguration();

                File.WriteAllText(cfgFilePath, AudioMod.cfg.ToString(), System.Text.Encoding.ASCII);
            } else {
                Log("Loading Mod Configuration");
                AudioMod.cfg = ModConfiguration.FromJson(File.ReadAllText(cfgFilePath));
            }

            if(AudioMod.cfg == null) {
                Log("!!! Unable to load Configuration !!!");
            } else {
                Log("Mod Configuration Loaded");
            }
        }

        #region Unity Hooks
        public bool IsAtMainMenu = true;
        public bool IsApplicationExiting = false;

        [OnStart]
        public void OnApplicationStart() {
            LoadModConfiguration();
            
            //Log("Creating Harmony Instance " + strHarmonyInstance);
            if ((harmony = new Harmony(strHarmonyInstance)) != null) {
                ApplyPatches();
                LoadCustomSounds();
            }

            //TODO: UI
            //UI.SpinModSettingsUI.CreateMenu();

            // setup handle for fresh menu scene changes
            BS_Utils.Utilities.BSEvents.OnLoad();
            BS_Utils.Utilities.BSEvents.menuSceneLoadedFresh += OnMenuSceneLoadedFresh;
            BS_Utils.Utilities.BSEvents.gameSceneLoaded += OnGameSceneLoaded;

            // keep track of active scene
            BS_Utils.Utilities.BSEvents.menuSceneActive += () => { IsAtMainMenu = true; };
            BS_Utils.Utilities.BSEvents.gameSceneActive += () => { IsAtMainMenu = false; };
        }

        
        private void OnMenuSceneLoadedFresh() {
            
        }

        private void OnGameSceneLoaded() {
        
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode) {
            //Log(scene.name + " scene loaded " + sceneMode.ToString());
        }

        [OnExit]
        void OnApplicationQuit() {
            Log("Removing Hooks");
            //SceneManager.activeSceneChanged -= SceneManagerOnActiveSceneChanged;
            //SceneManager.sceneLoaded -= OnSceneLoaded;

            if (writeLogOnExit)
                File.WriteAllText(logFilePath, LogFileData, System.Text.Encoding.ASCII);
        }

        public void OnUpdate() {
            /*
            if((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))) {
                if (Input.GetKeyDown(KeyCode.O)) {
                    //Toggle Random Sounds
                    cfg.EnableCustomSounds = !cfg.EnableCustomSounds;
                }

                if (Input.GetKeyDown(KeyCode.KeypadMultiply)) {
                    cfg.DisableFireworks = !cfg.DisableFireworks;
                }
            }
            */
        }

        public void OnFixedUpdate() {
        }
        
        void OnSceneUnloaded(Scene scene) {
        }

        protected void AddSwordSound(GameObject saber, AudioClip clip, float volume) {
            if (saber == null) {
                Log("Saber is null!");
                return;
            }

            if (clip == null) {
                Log("Clip is null!");
                return;
            }

            Log("Assigning " + saber.name + " Audio \"" + clip.name + "\" " + volume + "");

            //Create an object to track the tip of the saber
            Transform tTip = saber.transform.Find("tip");
            if (tTip == null) {
                Log(saber.name + " adding tip object");
                tTip = new GameObject("tip").transform;
                tTip.transform.SetParent(saber.transform);
                tTip.localPosition = new Vector3(0.0f, 0.0f, 1.0f);

                if (cfg.showDebugSpheres) {
                    GameObject ts = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    ts.transform.SetParent(tTip);
                    ts.transform.localPosition = Vector3.zero;
                    ts.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                }
            } else {
                Log(saber.name + " tip found");
            }

            Log("Checking for existing SwordSound");
            SwordSound ss = null;
            if ((ss = saber.GetComponent<SwordSound>()) != null) {
                Log("Already has SwordSound");
            } else {
                Log("Adding Sword Sound to " + saber.name);
                ss = saber.AddComponent<SwordSound>();
            }

            //Create an object to track the tip of the saber
            ss._swordPoint = tTip;
            ss._prevPoint = ss._swordPoint.position;

            ss._upSmooth = AudioMod.cfg.SpeedupSmoothing;
            ss._downSmooth = AudioMod.cfg.SlowdownSmoothing;
            ss._speedMultiplier = AudioMod.cfg.SpeedMultiplier;

            ss._audioSource = saber.GetComponent<AudioSource>();
            if (ss._audioSource == null) {
                Log("Adding AudioSource to SwordSound");
                ss._audioSource = saber.AddComponent<AudioSource>();
            } else {
                Log("AudioSource already exists, not adding");
            }

            ss._audioSource.playOnAwake = true;
            //Log(saber.name + " Setting Clip Loop");
            ss._audioSource.loop = true;
            //Log(saber.name + " Setting Clip Doppler");
            ss._audioSource.dopplerLevel = 1.0f;
            //Log(saber.name + " Setting Clip Spatialize");
            ss._audioSource.spatialize = true;
            //Log(saber.name + " Setting Clip Spatial Blend");
            ss._audioSource.spatialBlend = 1.0f;
            //Log(saber.name + " Setting Clip Minimum Distance");
            ss._audioSource.minDistance = 0.1f;
            //Log(saber.name + " Setting Clip Maximum Distance");
            ss._audioSource.maxDistance = 50.0f;
            //Log(saber.name + " Setting Clip Volume");
            ss._audioSource.volume = 0.0f; //Start volume off so it doesn't BVZZRRTTPFFTTT
            //Log(saber.name + " Setting Clip Priority");
            ss._audioSource.priority = 32;
            //Log(saber.name + " Setting Clip Rolloff Mode");
            ss._audioSource.rolloffMode = AudioRolloffMode.Logarithmic;

            if(ss._pitchBySpeedCurve == null) {
                //Log(saber.name + " Adding Pitch-by-Speed Curve");
                ss._pitchBySpeedCurve = new AnimationCurve();
            }

            if (ss._pitchBySpeedCurve.length == 0) {
                //Log(saber.name + " Setting Clip Pitch-By-Speed Curve");
                ss._pitchBySpeedCurve.AddKey(0, 1.0f);
                ss._pitchBySpeedCurve.AddKey(0.5f, 0.95f);
                ss._pitchBySpeedCurve.AddKey(1.0f, 0.875f);
            } else {
                Log(saber.name + " Clip Pitch-By-Speed Curve already Present");
            }

            if (ss._gainBySpeedCurve == null) {
                //Log(saber.name + " Adding Gain-by-Speed Curve");
                ss._gainBySpeedCurve = new AnimationCurve();
            }

            if (ss._gainBySpeedCurve.length == 0) {
                //Log(saber.name + " Setting Clip Gain-By-Speed Curve");
                ss._gainBySpeedCurve.AddKey(0, 0.85f);
                ss._gainBySpeedCurve.AddKey(0.5f, 0.95f);
                ss._gainBySpeedCurve.AddKey(1.0f, 10.0f);
            } else {
                Log(saber.name + " Clip Gain-By-Speed Curve already Present");
            }

            if (clip.loadState == AudioDataLoadState.Unloaded)
                clip.LoadAudioData();

            //ss._audioSource.bypassEffects = true;

            Log(saber.name + " Setting Source Clip");
            ss._audioSource.clip = clip;// AudioModifiersPlugin.SaberWhoosh[UnityEngine.Random.Range(0, AudioModifiersPlugin.SaberWhoosh.Count)];
            ss._audioSource.Play();
            Log(saber.name + " Saber Sound Active");
        }

        protected void AssignSaberSounds(Scene scene) {
            if(cfg == null) {
                Log("CFG Has not been loaded yet!");
                return;
            }

            if (AudioMod.SaberWhoosh.Count <= 0) {
                Log("No Saber Sounds found");
                return;
            }

            GameObject gameCore = scene.GetRootGameObjects().FirstOrDefault(); //"GameCore" "Saber Loader"

            #region DEBUG
            /* Spit out GameCore Hierarchy 4 levels deep
            Transform tC, ttC, tttC, ttttC;
            for(int c = 0; c < gameCore.transform.childCount; c++) {
                tC = gameCore.transform.GetChild(c);
                Log("  " + tC.gameObject.name);

                if (tC.childCount > 0) {
                    for (int cc = 0; cc < tC.childCount; cc++) {
                        ttC = tC.GetChild(cc);
                        Log("   " + ttC.gameObject.name);

                        if (ttC.childCount > 0) {
                            for (int ccc = 0; ccc < ttC.childCount; ccc++) {
                                tttC = ttC.GetChild(ccc);
                                Log("    " + tttC.gameObject.name);

                                if(tttC.childCount > 0) {
                                    for(int cccc = 0; cccc < tttC.childCount; cccc++) {
                                        ttttC = tttC.GetChild(cccc);
                                        Log("     " + tttC.gameObject.name);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            */
            #endregion

            Transform tOrigin = gameCore.transform.Find("Origin");

            if(tOrigin == null) {
                Log("Unable to locate Origin");
                return;
            }

            Transform tVRGameCore = tOrigin.Find("VRGameCore");
            if(tVRGameCore == null) {
                Log("Unable to locate GameCore");
                return;
            }

            //Get Both Sabers
            Transform tSaber = tVRGameCore.Find("LeftSaber");
            if(tSaber == null) {
                Log("Unable to locate LeftSaber");
                return;
            }

            GameObject objLeftSaber = tSaber.gameObject;
            Log("Left Saber " + ((objLeftSaber != null) ? "found" : "not found"));

            if(cfg.LeftWhoosh && objLeftSaber != null) {
                if (AudioMod.SaberWhoosh.Count > 0) {
                    AudioClip clip = AudioMod.SaberWhoosh[UnityEngine.Random.Range(0, AudioMod.SaberWhoosh.Count)];
                    if (clip != null) {
                        AddSwordSound(objLeftSaber, clip, AudioMod.cfg.LeftVolume);
                    } else {
                        Log(objLeftSaber.name + " Saber Audio Clip is null");
                    }
                } else {
                    Log("No Saber Whoosh Sounds");
                }
            }

            tSaber = tVRGameCore.Find("RightSaber");
            if(tSaber == null) {
                Log("Unable to locate RightSaber");
                return;
            }

            GameObject objRightSaber = tSaber.gameObject;//GameObject.Find("ControllerRight"); //tRight.gameObject
            Log("Right Saber " + ((objRightSaber != null) ? "found" : "not found"));
            if (cfg.RightWhoosh && objRightSaber != null) {
                if (AudioMod.SaberWhoosh.Count > 0) {
                    AudioClip clip = AudioMod.SaberWhoosh[UnityEngine.Random.Range(0, AudioMod.SaberWhoosh.Count)];
                    if (clip != null) {
                        AddSwordSound(objRightSaber, clip, AudioMod.cfg.RightVolume);
                    } else {
                        Log(objRightSaber.name + " Saber Audio Clip is null");
                    }
                } else {
                    Log("No Saber Whoosh Sounds");
                }
            }

            //Add Clash Effect Sounds
            Transform tSaberClash = gameCore.transform.Find("SaberClashEffect");
            if (tSaberClash == null) {
                Log("Unable to locate SaberClashEffect");
            } else {
                GameObject objClashEffect = tSaberClash.gameObject;

                AudioSource srcClash = objClashEffect.GetComponent<AudioSource>();
                if (srcClash == null) {
                    Log("Adding AudioSource to Clash Effect");
                    srcClash = objClashEffect.AddComponent<AudioSource>();

                    srcClash.playOnAwake = true;
                    //Log(saber.name + " Setting Clip Loop");
                    srcClash.loop = true;
                    //Log(saber.name + " Setting Clip Doppler");
                    srcClash.dopplerLevel = 1.0f;
                    //Log(saber.name + " Setting Clip Spatialize");
                    srcClash.spatialize = true;
                    //Log(saber.name + " Setting Clip Spatial Blend");
                    srcClash.spatialBlend = 1.0f;
                    //Log(saber.name + " Setting Clip Minimum Distance");
                    srcClash.minDistance = 0.1f;
                    //Log(saber.name + " Setting Clip Maximum Distance");
                    srcClash.maxDistance = 80.0f;
                    //Log(saber.name + " Setting Clip Volume");
                    srcClash.volume = 0.0f; //Start volume off so it doesn't BVZZRRTTPFFTTT
                                            //Log(saber.name + " Setting Clip Priority");
                    srcClash.priority = 32;
                    //Log(saber.name + " Setting Clip Rolloff Mode");
                    srcClash.rolloffMode = AudioRolloffMode.Logarithmic;
                } else {
                    Log("Clash Effect AudioSource already exists, not adding");
                }

                if (srcClash != null) {
                    ClashSource = srcClash;

                    if (cfg.showDebugSpheres && (objClashEffect.transform.Find("debugSphere") == null)) {
                        GameObject ts = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        ts.name = "debugSphere";
                        ts.transform.SetParent(objClashEffect.transform);
                        ts.transform.localPosition = Vector3.zero;
                        ts.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                    }
                }
            }
        }

        bool playingSong = false;
        bool enteringMenu = false;
        void OnActiveSceneChanged(Scene prevScene, Scene nextScene) {
            //Log("Active Scene Changed from " + prevScene.name + " to " + nextScene.name);

            playingSong = (nextScene.name == "GameCore");
            enteringMenu = (prevScene.name == "MenuEnvironment" && nextScene.name == "MenuCore");

            if (playingSong) {
                Log("Playing Song");
                if(AudioMod.cfg.EnableCustomSounds)
                    AssignSaberSounds(nextScene);
            }

            if (enteringMenu) {
                Log("Entering Main Menu");
            }
        }
        #endregion
    }
}
