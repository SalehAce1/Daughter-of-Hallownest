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
    internal class GoInCircle : MonoBehaviour
    {
        void OnCollisionEnter2D(Collider2D col)
        {
            Log(col.name);
        }
        private static void Log(object obj)
        {
            Logger.Log("[Fire Particle] " + obj);
        }
    }
}
