using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using JetBrains.Annotations;
using Modding;
using UnityEngine;
using UnityEngine.SceneManagement;
using UObject = UnityEngine.Object;
using USceneManager = UnityEngine.SceneManagement.SceneManager;

namespace DoH
{
    [UsedImplicitly]
    public class DoH : Mod<VoidModSettings>, ITogglableMod
    {
        public static DoH Instance;

        private string _lastScene;

        internal bool IsInHall => _lastScene == "GG_Workshop";

        public static GameObject weaverPref;
        public static GameObject grubRPref;
        public static GameObject grubLPref;
        public static GameObject wavePref;
        public static GameObject backgroundPart;

        public override string GetVersion()
        {
            return FileVersionInfo.GetVersionInfo(Assembly.GetAssembly(typeof(DoH)).Location).FileVersion;
        }

        public override void Initialize()
        {
            Instance = this;

            Log("Initalizing.");
            ModHooks.Instance.AfterSavegameLoadHook += AfterSaveGameLoad;
            ModHooks.Instance.NewGameHook += AddComponent;
            ModHooks.Instance.LanguageGetHook += LangGet;
            USceneManager.activeSceneChanged += LastScene;

            int ind = 0;
            Assembly asm = Assembly.GetExecutingAssembly();

            Resources.LoadAll<GameObject>("");
            foreach (var i in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                if (i.name == "Weaverling")
                {
                    weaverPref = i;
                }
                else if (i.name == "Grubberfly BeamR R")
                {
                    grubRPref = i;
                }
                else if (i.name == "Grubberfly BeamL R")
                {
                    grubLPref = i;
                }
                else if (i.name == "lava_particles_03")//grimm_flame_particle")//lava_particles_03") 
                {
                    wavePref = i;
                    //ModCommon.GameObjectExtensions.PrintSceneHierarchyTree(wavePref);
                }
                else if (i.name == "outskirts_particles")
                {
                    backgroundPart = i;
                }
                //Particle Wave: Void particle that emites radially 
                //grimm_flame_particle: fire particle
                //lava_particles_03: 4 red dots stuck together
            }

        }

        private void LastScene(Scene arg0, Scene arg1) => _lastScene = arg0.name;

        private string LangGet(string key, string sheettitle)
        {
            switch (key)
            {
                case "HORNET_MAIN": return "Daughter";
                case "HORNET_SUB": return "of Hallownest";
                case "NAME_HORNET_2": return "Daughter of Hallownest";
                case "GG_S_HORNET": return "Protector God, birthed, raised, and trained by three great Queens.";
                default: return Language.Language.GetInternal(key, sheettitle);
            }
        }

        private void AfterSaveGameLoad(SaveGameData data) => AddComponent();

        private void AddComponent()
        {
            GameManager.instance.gameObject.AddComponent<HornetFinder>();
        }

        public void Unload()
        {
            ModHooks.Instance.AfterSavegameLoadHook -= AfterSaveGameLoad;
            ModHooks.Instance.NewGameHook -= AddComponent;
            ModHooks.Instance.LanguageGetHook -= LangGet;
            USceneManager.activeSceneChanged -= LastScene;

            // ReSharper disable once Unity.NoNullPropogation
            var x = GameManager.instance?.gameObject.GetComponent<HornetFinder>();
            if (x == null) return;
            UObject.Destroy(x);
        }
    }
}