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
    internal class BeeControl : MonoBehaviour
    {
        GameObject target;
        Rigidbody2D rb2d;
        float acceleration;
        float speedMax;
        float turnRange;
        float angleOffset;
        bool everyFrame;
        bool worldSpace;
        float timeStart;

        void Start()
        {
            target = HeroController.instance.gameObject;
            acceleration = 0.185f;
            speedMax = 9999f;
            rb2d = gameObject.GetComponent<Rigidbody2D>();
            turnRange = 0f;
            angleOffset = 90f;
            everyFrame = true;
            worldSpace = true;

            timeStart = 0.8f;
        }

        void FixedUpdate()
        {
            if (timeStart <= 0)
            {
                if (this.rb2d == null)
                {
                    return;
                }
                Vector2 velocity = this.rb2d.velocity;
                if (gameObject.transform.position.x < target.transform.position.x - turnRange || gameObject.transform.position.x > target.transform.position.x + turnRange)
                {
                    if (gameObject.transform.position.x < target.transform.position.x)
                    {
                        velocity.x += this.acceleration;
                    }
                    else
                    {
                        velocity.x -= acceleration;
                    }
                    if (velocity.x > speedMax)
                    {
                        velocity.x = speedMax;
                    }
                    if (velocity.x < -speedMax)
                    {
                        velocity.x = -speedMax;
                    }
                    this.rb2d.velocity = velocity;
                }
                rb2d.velocity = new Vector2(rb2d.velocity.x, -13f);

                Vector2 velocity2 = rb2d.velocity;
                float z = Mathf.Atan2(velocity.y, velocity.x) * 57.2957764f + this.angleOffset;
                if (this.worldSpace)
                {
                    this.gameObject.transform.eulerAngles = new Vector3(0f, 0f, z);
                }
                else
                {
                    this.gameObject.transform.localEulerAngles = new Vector3(0f, 0f, z);
                }
                if (gameObject.transform.GetPositionY() < 20f)
                {
                    Destroy(gameObject);
                }
            }
            else
            {
                gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 0f);
                timeStart -= Time.fixedDeltaTime;
            }
        }

        private static void Log(object obj)
        {
            Logger.Log("[Bee Mind] " + obj);
        }
    }
}
