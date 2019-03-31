using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using IllusionPlugin;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using Harmony;

namespace AudioModifiers {
    public class AudioModifiersPlugin : IPlugin {
        public string Name => "AudioModifiers";
        public string Version => "0.0.2";

        public bool writeLogOnExit = true;
        public static string logFilePath = "./AudioModifiers.log";
        public static string LogFileData = "";

        public static HarmonyInstance harmony = null;

        protected static List<AudioClip> HitSounds = new List<AudioClip>();
        protected static List<AudioClip> MissSounds = new List<AudioClip>();
        public static RandomObjectPicker<AudioClip> HitSoundPicker = null;
        public static RandomObjectPicker<AudioClip> MissSoundPicker = null;
        
        public static string cfgFilePath = "./Config/AudioModifiers.json";
        public static ModConfiguration cfg = null;

        public static void Log(string message) {
            Console.WriteLine("[{0}] {1}", "AudioModifiers", message);
            LogFileData += message + '\n';
        }


        #region Harmony
        public static string strHarmonyInstance = "com.XypherOrion.SynthRiders.Weaponcraft";

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
        }

        public static void RemovePatches() {
            if(harmony.HasAnyPatches(strHarmonyInstance))
                harmony.UnpatchAll(strHarmonyInstance);
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

        public void LoadHitSounds() {
            //TODO: Make folders for sounds in addition to keying on substrings
            // eg: hit/ miss/ 
            //TODO: Accuracy Categories
            // eg: 110_ hits assign SuperHit.ogg, 80-100 hits assign GoodHit.ogg, etc
            if (Directory.Exists("CustomHits")) {
                Log("Reading Custom Hit Sounds");

                WWW www = null;

                string appFolder = Application.dataPath + "/../";

                int f;
                string[] AudioFiles;
                AudioClip clip;

                //Super lazy copypasta
                #region Load Copypasta
                AudioFiles = Directory.GetFiles("CustomHits", "*hit*.ogg");
                if (AudioFiles.Length > 0) {
                    for (f = 0; f < AudioFiles.Length; f++) {
                        www = new WWW("file://" + appFolder + AudioFiles[f]);

                        while (!www.isDone)
                            Log("Downloading... " + "file://" + appFolder + AudioFiles[f]);

                        clip = www.GetAudioClip(true);
                        clip.name = AudioFiles[f];

                        if (!clip.LoadAudioData())
                            Log("Unable to Load clip audio data " + clip.name);

                        //clip = (AudioClip)Resources.Load("CustomHits/" + AudioFiles[f]);
                        if (clip != null) {
                            HitSounds.Add(clip);
                            Log("Added " + AudioFiles[f] + " to Hit Sounds");
                        } else {
                            Log("Unable to add " + AudioFiles[f] + " to Hit sounds");
                        }
                    }
                } else
                    Log("No OGG Hit Files");

                AudioFiles = Directory.GetFiles("CustomHits", "*hit*.wav");
                if (AudioFiles.Length > 0) {
                    for (f = 0; f < AudioFiles.Length; f++) {
                        www = new WWW("file://" + appFolder + AudioFiles[f]);

                        while (!www.isDone)
                            Log("Downloading... " + "file://" + appFolder + AudioFiles[f]);

                        clip = www.GetAudioClip(true);
                        clip.name = AudioFiles[f];

                        if (!clip.LoadAudioData())
                            Log("Unable to Load clip audio data " + clip.name);

                        //clip = (AudioClip)Resources.Load("CustomHits/" + AudioFiles[f]);
                        if (clip != null) {
                            HitSounds.Add(clip);
                            Log("Added " + AudioFiles[f] + " to Hit Sounds");
                        } else {
                            Log("Unable to add " + AudioFiles[f] + " to Hit sounds");
                        }
                    }
                } else
                    Log("No WAV Hit Files");

                HitSoundPicker = new RandomObjectPicker<AudioClip>(HitSounds.ToArray(), 0.0f);

                AudioFiles = Directory.GetFiles("CustomHits", "*miss*.ogg");
                if (AudioFiles.Length > 0) {
                    for (f = 0; f < AudioFiles.Length; f++) {
                        www = new WWW("file://" + appFolder + AudioFiles[f]);

                        while (!www.isDone)
                            Log("Downloading... " + "file://" + appFolder + AudioFiles[f]);

                        clip = www.GetAudioClip(true);
                        clip.name = AudioFiles[f];

                        if (!clip.LoadAudioData())
                            Log("Unable to Load clip audio data " + clip.name);

                        //clip = (AudioClip)Resources.Load("CustomHits/" + AudioFiles[f]);
                        if (clip != null) {
                            MissSounds.Add(clip);
                            Log("Added " + AudioFiles[f] + " to Miss Sounds");
                        } else {
                            Log("Unable to add " + AudioFiles[f] + " to Miss sounds");
                        }
                    }
                } else
                    Log("No OGG Miss Files");

                AudioFiles = Directory.GetFiles("CustomHits", "*miss*.wav");
                if (AudioFiles.Length > 0) {
                    for (f = 0; f < AudioFiles.Length; f++) {
                        www = new WWW("file://" + appFolder + AudioFiles[f]);

                        while (!www.isDone)
                            Log("Downloading... " + "file://" + appFolder + AudioFiles[f]);

                        clip = www.GetAudioClip(true);
                        clip.name = AudioFiles[f];

                        if (!clip.LoadAudioData())
                            Log("Unable to Load clip audio data " + clip.name);

                        //clip = (AudioClip)Resources.Load("CustomHits/" + AudioFiles[f]);
                        if (clip != null) {
                            MissSounds.Add(clip);
                            Log("Added " + AudioFiles[f] + " to Miss Sounds");
                        } else {
                            Log("Unable to add " + AudioFiles[f] + " to Miss sounds");
                        }
                    }
                } else
                    Log("No WAV Miss Files");
                #endregion

                MissSoundPicker = new RandomObjectPicker<AudioClip>(MissSounds.ToArray(), 0.0f);
            } else {
                Log("CustomHits folder does not exist! Creating.");
                Directory.CreateDirectory("./CustomHits");
            }
        }


        protected static void LoadModConfiguration() {
            if (!File.Exists(cfgFilePath)) {
                Log("Writing default Audio Modifiers Configuration");
                if (!Directory.Exists("./Config")) {
                    Log("Creating Config Folder");
                    Directory.CreateDirectory("./Config");
                }

                if (AudioModifiersPlugin.cfg == null)
                    AudioModifiersPlugin.cfg = new ModConfiguration();

                File.WriteAllText(cfgFilePath, AudioModifiersPlugin.cfg.ToString(), System.Text.Encoding.ASCII);
            } else {
                AudioModifiersPlugin.cfg = ModConfiguration.FromJson(File.ReadAllText(cfgFilePath));
                Log(AudioModifiersPlugin.cfg.ToString());
            }
        }

        #region Unity Hooks
        public void OnApplicationStart() {
            LoadModConfiguration();

            //Log("Hooking into activeSceneChanged");
            SceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;
            //Log("Hooking into SceneLoaded");
            SceneManager.sceneLoaded += SceneManager_sceneLoaded;

            //Log("Creating Harmony Instance " + strHarmonyInstance);
            if((harmony = HarmonyInstance.Create(strHarmonyInstance)) != null)
                ApplyPatches();

            //TODO: Load Random Sounds into Assets
            LoadHitSounds();
        }

        private void SceneManagerOnActiveSceneChanged(Scene arg0, Scene arg1) {
            //Log("Active Scene Changed: " + arg0.name + " to " + arg1.name);

        }

        private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1) {
            //Log("Scene Loaded: " + arg0.name + " " + arg1.ToString());
        }

        public void OnApplicationQuit() {
            Log("Removing Hooks");
            SceneManager.activeSceneChanged -= SceneManagerOnActiveSceneChanged;
            SceneManager.sceneLoaded -= SceneManager_sceneLoaded;

            if (writeLogOnExit)
                File.WriteAllText(logFilePath, LogFileData, System.Text.Encoding.ASCII);
        }

        public void OnUpdate() {
            if((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))) {
                //Holding Shift
                if (Input.GetKeyDown(KeyCode.O)) {
                    //Toggle Random Sounds
                    cfg.EnableCustomSounds = !cfg.EnableCustomSounds;
                }

                if (Input.GetKeyDown(KeyCode.P)) {
                    //Toggle Random Pitch
                    cfg.UseRandomPitch = !cfg.UseRandomPitch;
                }

                if (Input.GetKeyDown(KeyCode.KeypadMultiply)) {
                    cfg.DisableFireworks = !cfg.DisableFireworks;
                }
            }
        }

        public void OnFixedUpdate() {
        }

        public void OnLevelWasLoaded(int level) {
        }

        public void OnLevelWasInitialized(int level) {
        }
        #endregion
    }
}
