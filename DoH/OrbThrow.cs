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
    internal class OrbThrow : MonoBehaviour
    {
        float time = 3f;
        Vector2 oldDirect;
        public GameObject target = HeroController.instance.gameObject;
        public float angle = 170f;
        public float speed = 13f;
        bool first;

        void OnTriggerEnter2D(Collider2D col)
        {
            Log(col.name);
            if (col.name == "Floor Saver" || col.name == "Roof Collider" || col.name == "Terrain Saver")
            {
                Destroy(gameObject);
            }
        }

        void Start()
        {
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(Hornet.orbDirec * 20f, Hornet.orbNum * 4f);
            if (Hornet.orbDirec == 1)
                gameObject.GetComponent<Rigidbody2D>().rotation = (Mathf.Rad2Deg * (Mathf.Atan(gameObject.GetComponent<Rigidbody2D>().velocity.y / gameObject.GetComponent<Rigidbody2D>().velocity.x))) + 180f;
            else
            {
                gameObject.GetComponent<Rigidbody2D>().rotation = (Mathf.Rad2Deg * (Mathf.Atan(gameObject.GetComponent<Rigidbody2D>().velocity.y / gameObject.GetComponent<Rigidbody2D>().velocity.x)));
            }
            time = 3f;
        }

        void Update()
        {
            if (!first && time <= 2.5f)
            {
                first = true;
                gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 0f);
                LookTowards(360);
            }

            if (time <= 2.5f)
            {
                LookTowards(angle * Time.deltaTime);
                //speed += 0.1f;
                gameObject.transform.position += (gameObject.transform.right * speed * Time.deltaTime);
            }
            time -= Time.deltaTime;
            if (time <= 0f)
            {
                Destroy(gameObject);
            }
        }
        public void LookTowards(float rotationSpeed)
        {
            Vector3 targetDir = target.transform.position - gameObject.transform.position;

            targetDir.Normalize();
            float r = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;
            gameObject.transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.AngleAxis(r, Vector3.forward), rotationSpeed);
        }
        private static void Log(object obj)
        {
            Logger.Log("[Orb Throw] " + obj);
        }
    }
}
