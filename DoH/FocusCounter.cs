using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using JetBrains.Annotations;
using ModCommon.Util;
using ModCommon;
using Modding;
using UnityEngine;
using UnityEngine.UI;
using Logger = Modding.Logger;
using USceneManager = UnityEngine.SceneManagement.SceneManager;

namespace DoH
{
    internal class FocusCounter : MonoBehaviour
    {
        float time = 0.8f;
        void Update()
        {
            time -= Time.deltaTime;
            if (time <= 0f)
            {
                //Log("DESTZROYED");
                //Hornet.focusAmount++;
                //Destroy(gameObject);
            }
        }
        private static void Log(object obj)
        {
            Logger.Log("[FocusCount] " + obj);
        }
    }
}
