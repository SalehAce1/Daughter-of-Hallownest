using System.Collections;
using UnityEngine;
using HutongGames.PlayMaker.Actions;
using Logger = Modding.Logger;

namespace DoH
{
    internal class HornetFinder : MonoBehaviour
    {
        private GameObject _hornet;
        public static GameObject _mantis;
        public static GameObject _orb;
        public static GameObject _focus;
        public static GameObject _bee;
        public static GameObject _bat;
        public static GameObject _dashFire;
        public static GameObject _trap;

        private void Start()
        {
            Logger.Log("[Daughter of Hallownest] Added HornetFinder MonoBehaviour");
            DontDestroyOnLoad(this);
            StartCoroutine(loadingMantis());
        }
        private void Update()
        {
            if (!PlayerData.instance.hornetOutskirtsDefeated) return;

            if (_hornet != null) return;
            _hornet = GameObject.Find("Hornet Boss 2");
            if (_hornet == null) return;
            _hornet.AddComponent<Hornet>();
        }
        IEnumerator loadingMantis() 
        {
            //GameCameras.instance.cameraFadeFSM.Fsm.Event("FADE INSTANT");
            Logger.Log("Wait2");
            //GameCameras.instance.cameraFadeFSM.Fsm.Event("FADE INSTANT");
            //yield return null;
            GameManager.instance.LoadScene("GG_Mantis_Lords");
            yield return null;
            Resources.LoadAll<GameObject>("");
            foreach (var i in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                if (i.name == "Shot Mantis Lord")
                {
                    _mantis = Instantiate(i);
                }
            }
            if (_mantis == null)
            {
                Modding.Logger.Log("Not found.");
            }
            else
            {
                DontDestroyOnLoad(_mantis);
                Destroy(_mantis.LocateMyFSM("Control"));
                _mantis.SetActive(false);
                Modding.Logger.Log("Wow I actually found it?");
            }

            Logger.Log("Wait2");
            GameManager.instance.LoadScene("GG_Radiance");
            yield return null;
            Resources.LoadAll<GameObject>("");
            foreach (var i in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                if (i.name == "Radiant Orb")
                {
                    _orb = Instantiate(i);
                }
            }
            if (_orb == null)
            {
                Modding.Logger.Log("Not found.");
            }
            else
            {
                DontDestroyOnLoad(_orb);
                Destroy(_orb.LocateMyFSM("Orb Control"));
                _orb.SetActive(false);
                Modding.Logger.Log("Wow I actually found it 2?");
            }

            Logger.Log("Wait3");
            GameManager.instance.LoadScene("GG_Hollow_Knight");
            yield return null;
            Resources.LoadAll<GameObject>("");
            foreach (var i in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                //Logger.Log("PV " + i.name);
                if (i.name == "Blast")
                {
                    _focus = Instantiate(i);
                }
            }
            if (_focus == null)
            {
                Modding.Logger.Log("Not found.");
            }
            else
            {
                DontDestroyOnLoad(_focus);
                //Destroy(_focus.LocateMyFSM("Control"));
                _focus.SetActive(false);
                Modding.Logger.Log("Wow I actually found it 3?");
            }

            Logger.Log("Wait4");
            GameManager.instance.LoadScene("GG_Hive_Knight");
            yield return null;
            Resources.LoadAll<GameObject>("");
            foreach (var i in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                
                if (i.name == "Bee Dropper (1)")
                {
                    _bee = Instantiate(i);
                }
                else if (i.name == "Hive Knight Glob (1)")
                {
                    _trap = Instantiate(i);
                }
            }
            if (_bee == null)
            {
                Modding.Logger.Log("Not found.");
            }
            else
            {
                DontDestroyOnLoad(_bee);
                //ModCommon.GameObjectExtensions.PrintSceneHierarchyTree(_bee);
                try
                {
                    var dungo = _bee.LocateMyFSM("Control");
                    dungo.ChangeTransition("Init", "FINISHED", "Swarm Start");
                    dungo.ChangeTransition("Swarm", "END", "Swarm");
                    dungo.ChangeTransition("Swarm", "SPELL", "Swarm");
                }
                catch(System.Exception e)
                {
                    Logger.Log(e);
                }
                //Destroy(_bee.LocateMyFSM("Control"));
                _bee.SetActive(false);
                Modding.Logger.Log("Wow I actually found it 4?");
            }

            Logger.Log("Wait5");
            GameManager.instance.LoadScene("GG_Grimm_Nightmare");
            yield return null;
            Resources.LoadAll<GameObject>("");
            foreach (var i in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                if (i.name == "Nightmare Firebat(Clone)")
                {
                    _bat = Instantiate(i);
                }
                //else if (i.name == "Flame Trail(Clone)")
                //{
                //    _dashFire = Instantiate(i);
                //}
            }
            if (_bat == null)
            {
                Modding.Logger.Log("Not found.");
            }
            else
            {
                DontDestroyOnLoad(_bat);
                //ModCommon.GameObjectExtensions.PrintSceneHierarchyTree(_bat);
                var dungo = _bee.LocateMyFSM("Control");
                dungo.ChangeTransition("Init", "HIGH", "Fire");
                dungo.ChangeTransition("Init", "MID", "Fire");
                dungo.ChangeTransition("Fire", "DISSIPATE", "Fire");
                dungo.ChangeTransition("Fire", "END", "Fire");
                dungo.ChangeTransition("Fire", "ORBIT SHIELD", "Fire");
                _bat.SetActive(false);
                Modding.Logger.Log("Wow I actually found it 5?");
            }
            //yield return null;
            //GameCameras.instance.cameraFadeFSM.Fsm.SetState("FadeIn");
            /*if (_dashFire == null)
            {
                Modding.Logger.Log("Not found.");
            }
            else
            {
                DontDestroyOnLoad(_dashFire);
                ModCommon.GameObjectExtensions.PrintSceneHierarchyTree(_dashFire);
                _dashFire.SetActive(false);
                Modding.Logger.Log("Wow I actually found it 6?");
            }*/
        }
    }
}