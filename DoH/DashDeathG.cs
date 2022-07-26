using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using JetBrains.Annotations;
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
            
           
            
        }
        IEnumerator Start()
        {
            yield return new WaitWhile(() => !gameObject.activeSelf);
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 10f);
            yield return new WaitForSeconds(1.5f);
            Destroy(gameObject);
        }
        
        private static void Log(object obj)
        {
            Logger.Log("[Fire Particle] " + obj);
        }
    }
}
