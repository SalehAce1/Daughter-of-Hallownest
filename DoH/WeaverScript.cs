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
    internal class WeaverScript : MonoBehaviour
    {
        bool firstAct = false;
        float timeBeforeDeath = 10f;
        void Update()
        {
            if (this.gameObject.activeSelf && !firstAct)
            {
                firstAct = true;
                timeBeforeDeath = 10f;
            }
            if (firstAct)
            {
                timeBeforeDeath -= Time.deltaTime;
            }
            if (timeBeforeDeath <= 0)
            {
                firstAct = false;
                this.gameObject.SetActive(false);
            }
        }
        private static void Log(object obj)
        {
            Logger.Log("[Daughter of Hallownest] " + obj);
        }
    }
}
