using System.Collections;
using UnityEngine;
using Logger = Modding.Logger;

namespace DoH
{
    internal class HornetFinder : MonoBehaviour
    {
        private GameObject _hornet;
        //public static GameObject _mantis;

        private void Start()
        {
            Logger.Log("[Daughter of Hallownest] Added HornetFinder MonoBehaviour");
            //DontDestroyOnLoad(this);
            //StartCoroutine(loadingMantis());
        }
        private void Update()
        {
            if (!PlayerData.instance.hornetOutskirtsDefeated) return;

            if (_hornet != null) return;
            _hornet = GameObject.Find("Hornet Boss 2");
            if (_hornet == null) return;
            _hornet.AddComponent<Hornet>();
        }
        /*IEnumerator loadingMantis() 
        {
            Logger.Log("Wait1");
            //GameCameras.instance.cameraFadeFSM.Fsm.Event("FADE INSTANT");
            Logger.Log("Wait2");
            GameManager.instance.LoadScene("GG_Mantis_Lords");
            yield return null;
            Resources.LoadAll<GameObject>("");
            foreach (var i in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                if (i.name == "Shot Mantis Lord")
                {
                    Modding.Logger.Log("Shot Mantis Lord");
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
                Modding.Logger.Log("Wow I actually found it?");
            }
        }*/
    }
}