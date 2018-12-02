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
    internal class DashDeathG : MonoBehaviour
    {
        bool firstAct = false;
        bool secondAct = false;
        float timeBeforeDeath = 1.5f;
        float x, y;

        void OnTriggerEnter2D(Collider2D col)
        {
            if(col.name == "Knight")
            {
                HeroController.instance.TakeDamage(HeroController.instance.gameObject, GlobalEnums.CollisionSide.other, 1, 1);
            }
            
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 10f);
            
        }

        void Update()
        {
            try
            {
                if (this.gameObject.activeSelf && !firstAct)
                {
                    firstAct = true;
                    timeBeforeDeath = 1.5f;
                }
                if (firstAct)
                {
                    timeBeforeDeath -= Time.deltaTime;
                }
                if (timeBeforeDeath <= 0)
                {
                    firstAct = false;
                    Destroy(gameObject);
                }
            }
            catch(System.Exception e)
            {
                Log(e);
            }
        }
        private static void Log(object obj)
        {
            Logger.Log("[Fire Particle] " + obj);
        }
    }
}
