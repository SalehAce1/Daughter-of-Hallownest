using System.Collections;
using UnityEngine;
using HutongGames.PlayMaker.Actions;
using UnityEngine.SceneManagement;
using USceneManager = UnityEngine.SceneManagement.SceneManager;
using Logger = Modding.Logger;

namespace DoH
{
    internal class HornetFinder : MonoBehaviour
    {
        public static GameObject _mantis;
        public static GameObject _focus;
        public static GameObject _orb;
        public static GameObject _bee;
        public static GameObject _bat;

        private IEnumerator Start()
        {
            Logger.Log("[Daughter of Hallownest] Added HornetFinder MonoBehaviour");
            USceneManager.activeSceneChanged += SceneChanged;

            yield return new WaitWhile(() => !HeroController.instance);
            DoH.grubLPref = HeroController.instance.grubberFlyBeamPrefabL_fury;
            DoH.grubRPref = HeroController.instance.grubberFlyBeamPrefabR_fury;

            Destroy(DoH.preloadedGO["mantis"].LocateMyFSM("Control"));

            DoH.preloadedGO["orb"] = DoH.preloadedGO["rad"].LocateMyFSM("Attack Commands").GetAction<SpawnObjectFromGlobalPool>("Spawn Fireball", 1).gameObject.Value;
            Destroy(_orb.LocateMyFSM("Orb Control"));

            var dungo = DoH.preloadedGO["bee"].LocateMyFSM("Control");
            dungo.ChangeTransition("Init", "FINISHED", "Swarm Start");
            dungo.ChangeTransition("Swarm", "END", "Swarm");
            dungo.ChangeTransition("Swarm", "SPELL", "Swarm");

            DoH.preloadedGO["bat"] = DoH.preloadedGO["grimm"].LocateMyFSM("Control").GetAction<SpawnObjectFromGlobalPool>("Firebat 1", 2).gameObject.Value;
            var dungo2 = DoH.preloadedGO["bat"].LocateMyFSM("Control");
            dungo2.ChangeTransition("Init", "HIGH", "Fire");
            dungo2.ChangeTransition("Init", "MID", "Fire");
            dungo2.ChangeTransition("Fire", "DISSIPATE", "Fire");
            dungo2.ChangeTransition("Fire", "END", "Fire");
            dungo2.ChangeTransition("Fire", "ORBIT SHIELD", "Fire");

            _mantis = DoH.preloadedGO["mantis"];
            _orb = DoH.preloadedGO["orb"];
            _bee = DoH.preloadedGO["bee"];
            _focus = DoH.preloadedGO["blast"];
            _bat = DoH.preloadedGO["bat"];
        }
        private void SceneChanged(Scene arg0, Scene arg1)
        {
            if (arg1.name != "GG_Hornet_2") return;

            StartCoroutine(AddComponent());
        }
        private static IEnumerator AddComponent()
        {
            yield return null;

            GameObject.Find("Hornet Boss 2").AddComponent<Hornet>();
        }
        private void OnDestroy()
        {
            USceneManager.activeSceneChanged -= SceneChanged;
        }
    }
}