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
    public class DoH : Mod, ITogglableMod
    {
        public static DoH Instance;
        private string _lastScene;
        public static Dictionary<string, GameObject> preloadedGO = new Dictionary<string, GameObject>();
        internal bool IsInHall => _lastScene == "GG_Workshop";
        public static readonly IList<Sprite> SPRITES = new List<Sprite>();

        public static GameObject weaverPref;
        public static GameObject grubRPref;
        public static GameObject grubLPref;
        public static GameObject wavePref;
        public static GameObject backgroundPart;

        public override string GetVersion()
        {
            return "2.0.0.0.0-1.5";
        }

        public override List<(string, string)> GetPreloadNames()
        {
            return new List<(string, string)>
            {
                ("GG_Mantis_Lords", "Shot Mantis Lord"),
                ("GG_Radiance", "Boss Control/Absolute Radiance"),
                ("GG_Hollow_Knight", "Battle Scene/Focus Blasts/HK Prime Blast (4)/Blast"),
                ("GG_Hive_Knight", "Battle Scene/Droppers/Bee Dropper (1)"),
                ("Grimm_Nightmare","Grimm Control/Nightmare Grimm Boss"),
            };
        }

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            preloadedGO.Add("mantis", preloadedObjects["GG_Mantis_Lords"]["Shot Mantis Lord"]);
            preloadedGO.Add("rad", preloadedObjects["GG_Radiance"]["Boss Control/Absolute Radiance"]);
            preloadedGO.Add("blast", preloadedObjects["GG_Hollow_Knight"]["Battle Scene/Focus Blasts/HK Prime Blast (4)/Blast"]);
            preloadedGO.Add("bee", preloadedObjects["GG_Hive_Knight"]["Battle Scene/Droppers/Bee Dropper (1)"]);
            preloadedGO.Add("grimm", preloadedObjects["Grimm_Nightmare"]["Grimm Control/Nightmare Grimm Boss"]);
            preloadedGO.Add("bat", null);

            Instance = this;
            Unload();
            Log("Initalizing.");
            ModHooks.AfterSavegameLoadHook += AfterSaveGameLoad;
            ModHooks.NewGameHook += AddComponent;
            ModHooks.LanguageGetHook += LangGet;
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
                else if (i.name == "lava_particles_03")
                {
                    wavePref = i;
                }
                else if (i.name == "outskirts_particles")
                {
                    backgroundPart = i;
                }
            }
            PVSound.LoadAssets.LoadOrbSound();


            foreach (string res in asm.GetManifestResourceNames())
            {
                if (!res.EndsWith(".png"))
                {
                    Log("Unknown resource: " + res);

                    continue;
                }

                using (Stream s = asm.GetManifestResourceStream(res))
                {
                    if (s == null) continue;

                    byte[] buffer = new byte[s.Length];
                    s.Read(buffer, 0, buffer.Length);
                    s.Dispose();

                    // Create texture from bytes
                    var tex = new Texture2D(1, 1);
                    tex.LoadImage(buffer,true);
                    // Create sprite from texture
                    SPRITES.Add(Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f)));

                    Log("Created sprite from embedded image: " + res + " at ind " + ++ind);
                }
            }
        }

        private void LastScene(Scene arg0, Scene arg1) => _lastScene = arg0.name;

        private string LangGet(string key, string sheettitle,string orig)
        {
            switch (key)
            {
                case "HORNET_MAIN": return "Daughter";
                case "HORNET_SUB": return "of Hallownest";
                case "NAME_HORNET_2": return "Daughter of Hallownest";
                case "GG_S_HORNET": return "Protector God, birthed, raised, and trained by three great Queens.";
                default: return orig;
            }
        }

        private void AfterSaveGameLoad(SaveGameData data) => AddComponent();

        private void AddComponent()
        {
            GameManager.instance.gameObject.AddComponent<HornetFinder>();
        }

        public void Unload()
        {
            ModHooks.AfterSavegameLoadHook -= AfterSaveGameLoad;
            ModHooks.NewGameHook -= AddComponent;
            ModHooks.LanguageGetHook -= LangGet;
            USceneManager.activeSceneChanged -= LastScene;

            // ReSharper disable once Unity.NoNullPropogation
            var x = GameManager.instance?.gameObject.GetComponent<HornetFinder>();
            if (x == null) return;
            UObject.Destroy(x);
        }
    }
}