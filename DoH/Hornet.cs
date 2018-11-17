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
    internal class Hornet : MonoBehaviour
    {
        private readonly Dictionary<string, float> _fpsDict = new Dictionary<string, float> //Use to change animation speed
        {
            //["Run"] = 100
            ["Sphere Antic A Q"] = 20,
            ["Sphere Recover A"] = 20,
            ["Sphere Antic G"] = 20,
            ["Sphere Recover G"] = 20
        };
        public static HealthManager _hm;
        private tk2dSpriteAnimator _anim;
        private Recoil _recoil;
        private PlayMakerFSM _stunControl;
        private PlayMakerFSM _control;
        private PlayMakerFSM _needleControl;
        private PlayMakerFSM _needleControl2;
        private PlayMakerFSM _weaverControl;
        private PlayMakerFSM _beamControlR;
        private PlayMakerFSM _beamControlL;
        private GameObject needle;
        private GameObject needle2;
        private GameObject canvas;
        private GameObject weaver;
        private GameObject[] needles = new GameObject[10];

        private Text textExample;

        private GameObject grubR;
        private GameObject grubL;
        private GameObject[] grubAll = new GameObject[10];
        private GameObject wave;
        private GameObject greatSlash;


        float timeLeft;
        float heightNeedle1;
        float heightNeedle2;
        float needleVelocity;
        float height;
        float timer = 3f;
        bool dang;
        float angle = 0;
        private bool finalPhase = false;
        private bool secondPhase = false;

        private void Awake()
        {
            Log("Added Hornet Mono");

            if (!PlayerData.instance.hornetOutskirtsDefeated) return;
            if (!DoH.Instance.IsInHall) return;
            _hm = gameObject.GetComponent<HealthManager>();
            _stunControl = gameObject.LocateMyFSM("Stun Control");
            _control = gameObject.LocateMyFSM("Control");
            _recoil = gameObject.GetComponent<Recoil>();
            _anim = gameObject.GetComponent<tk2dSpriteAnimator>();
        }

        private void Start()
        {
            if (!PlayerData.instance.hornetOutskirtsDefeated) return;
            if (!DoH.Instance.IsInHall) return;

            CanvasUtil.CreateFonts();
            canvas = CanvasUtil.CreateCanvas(RenderMode.ScreenSpaceOverlay, new Vector2(1920, 1080));
            UnityEngine.Object.DontDestroyOnLoad(canvas);
            textExample = CanvasUtil.CreateTextPanel(canvas, "", 30, TextAnchor.MiddleLeft, new CanvasUtil.RectData(new Vector2(600, 50), new Vector2(-560, 805), new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0.5f, 0.5f)), true).GetComponent<Text>();
            textExample.color = new Color(1f, 1f, 1f, 1f);

            timeLeft = 2f;

            // Refill MP
            HeroController.instance.AddMPChargeSpa(999);

            // No stunning
            Destroy(_stunControl);

            // 1500hp
            _hm.hp = 1500;

            // Disable Knockback
            _recoil.enabled = false;

            // Speed up some attacks.
            try
            {
                foreach (KeyValuePair<string, float> i in _fpsDict)
                {
                    _anim.GetClipByName(i.Key).fps = i.Value;
                }
            }
            catch(System.Exception e)
            {
                Log(e);
            }
            

            // Stop pointless standing in place
            _control.GetAction<WaitRandom>("Idle", 9).timeMax = 0f;
            _control.GetAction<WaitRandom>("Idle", 9).timeMin = 0f;

            // Stop pointless running
            _control.GetAction<WaitRandom>("Run", 6).timeMax = 0f;
            _control.GetAction<WaitRandom>("Run", 6).timeMin = 0f;

            //Make Hornet hold her sphere for 5 seconds
            var go = _control.GetAction<ActivateGameObject>("Sphere Recover A", 1).gameObject.GameObject.Value;
            var go2 = _control.GetAction<ActivateGameObject>("Sphere Recover", 1).gameObject.GameObject.Value;
            IEnumerator ActivateSphereA()
            {
                Log("Hornet: Activate Air turbo sphere mode, DIE little Ghost.");
                go.SetActive(true);
                yield return new WaitForSeconds(3f);
                go.SetActive(false);
            }
            IEnumerator ActivateSphereG()
            {
                Log("Hornet: Activate Ground turbo sphere mode, DIE little Ghost.");
                go2.SetActive(true);
                yield return new WaitForSeconds(3f);
                go2.SetActive(false);
            }
            _control.CopyState("Sphere Recover", "Sphere Recover Old");
            _control.CopyState("Sphere Recover A", "Sphere Recover A Old");
            _control.RemoveAction("Sphere Recover A", 1);
            _control.RemoveAction("Sphere Recover", 1);
            _control.InsertCoroutine("Sphere Recover A", 1, ActivateSphereA);
            _control.InsertCoroutine("Sphere Recover", 1, ActivateSphereG);

            //Make Hornet Dash after needle throw
            var removeNeedle = _control.GetAction<ActivateGameObject>("Throw Recover", 0);
            _control.InsertAction("Throw Antic", removeNeedle, 0);
            removeNeedle.gameObject.GameObject.Value.LocateMyFSM("Control").ChangeTransition("Out", "FINISHED", "Notify");
            _control.CopyState("Jump", "Jump2");
            _control.CopyState("ADash Antic", "ADash Antic 2");
            _control.GetAction<GetAngleToTarget2D>("ADash Antic 2", 1).offsetY.Value += 3f;
            _control.ChangeTransition("Throw", "FINISHED", "Jump2");
            _control.ChangeTransition("Jump2", "FINISHED", "ADash Antic 2");

            //Make a better G Dash for attacking you while you heal
            var lookAtKnight = _control.GetAction<FaceObject>("GDash Antic", 2);
            _control.CopyState("G Dash", "G Dash 2");
            _control.InsertAction("G Dash 2", new FaceObject
            {
                objectA = lookAtKnight.objectA,
                objectB = lookAtKnight.objectB,
                spriteFacesRight = lookAtKnight.spriteFacesRight,
                playNewAnimation = lookAtKnight.playNewAnimation,
                newAnimationClip = lookAtKnight.newAnimationClip,
                resetFrame = lookAtKnight.resetFrame,
                everyFrame = lookAtKnight.everyFrame
            }, 0);
            _control.ChangeTransition("G Dash 2", "FINISHED", "CA Antic");

            try
            {
                Log("Create a copy of the needle and use it as a horizontal attack");
                needle = Instantiate(_control.GetAction<SetPosition>("Throw", 4).gameObject.GameObject.Value);
                Destroy(needle.LocateMyFSM("Control"));
                needle.SetActive(true);
                this.needle.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 0f);
                needle.transform.SetPosition2D(12, 35);
                this.needle.AddComponent<TinkEffect>();
                Hornet.Log("Error was here right?");
                UnityEngine.Object.Destroy(this.needle.GetComponent<NonBouncer>());
                var tink = UnityEngine.Object.Instantiate(GameObject.Find("Needle Tink")).AddComponent<ModCommon.NeedleTink>();
                tink.SetParent(needle.transform);
                heightNeedle1 = 30;
                heightNeedle2 = 35;
                needleVelocity = 15;

                Log("Create a copy of the needle and use it as a horizontal attack");
                needle2 = Instantiate(_control.GetAction<SetPosition>("Throw", 4).gameObject.GameObject.Value);
                Destroy(needle2.LocateMyFSM("Control"));
                needle2.SetActive(true);
                this.needle2.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 0f);
                needle2.transform.SetPosition2D(40, 35);
                this.needle2.AddComponent<TinkEffect>();
                Hornet.Log("Error was here right?");
                UnityEngine.Object.Destroy(this.needle2.GetComponent<NonBouncer>());
                var tink2 = UnityEngine.Object.Instantiate(GameObject.Find("Needle Tink")).AddComponent<ModCommon.NeedleTink>();
                tink2.SetParent(needle2.transform);

                Log("Remove Evade when hit because it's dumb and also makes it so when hit hornet has a higher chance of either attacking or jumping");
                _control.GetAction<SendRandomEvent>("Dmg Response", 0).weights[0] = 0f;
                _control.GetAction<SendRandomEvent>("Dmg Response", 0).weights[1] = 0.4f;
                _control.GetAction<SendRandomEvent>("Dmg Response", 0).weights[2] = 0.5f;
                _control.GetAction<SendRandomEvent>("Dmg Response", 0).weights[3] = 0.1f;


                Log("When she gets hit she does not only jump");
                _control.ChangeTransition("Dmg Response", "JUMP", "Jump Antic");

                Log("Skip waiting for player to hit her counter and never do the dumb evade move");
                _control.GetAction<Wait>("Counter Stance", 1).time = 0f;
                _control.ChangeTransition("Counter Stance", "FINISHED", "CA Antic");

                Log("Choose Counter over Evade");
                _control.GetAction<SendRandomEvent>("Ev Or Counter", 0).weights[0] = 0f;
                _control.GetAction<SendRandomEvent>("Ev Or Counter", 0).weights[1] = 1f;

                Log("Choose GDash over Evade");
                _control.ChangeTransition("Run", "EVADE", "GDash Antic");
            }
            catch (System.Exception e)
            {
                Log(e);
            }

            //Removing useless barbs
            _control.ChangeTransition("Barb?", "BARB", "Can Throw?");
            

            Log("Added health recovery with Weavers.");
            weaver = Instantiate(DoH.weaverPref);
            weaver.transform.SetPosition2D(gameObject.transform.GetPositionX(), gameObject.transform.GetPositionY());
            var warpDelete = weaver.LocateMyFSM("Warp To Hero");
            _weaverControl = weaver.LocateMyFSM("Control");
            warpDelete.ChangeTransition("Check", "WARP", "Idle");
            weaver.AddComponent<HealthManager>().hp = 1;
            weaver.AddComponent<DamageEnemies>().damageDealt = 0;
            weaver.AddComponent<WeaverScript>();
            weaver.SetActive(false);

            //Stops the dumb freeze effect when the counter occurs
            _control.RemoveAction("CA Antic", 1);

            _control.GetAction<Wait>("Sphere", 4).time = 0.3f;
            _control.GetAction<Wait>("Sphere A", 4).time = 0.3f;

            for (int i = 0; i < needles.Length; i++)
            {
                needles[i] = Instantiate(_control.GetAction<SetPosition>("Throw", 4).gameObject.GameObject.Value);
                Destroy(needles[i].LocateMyFSM("Control"));
                needles[i].AddComponent<TinkEffect>();
                UnityEngine.Object.Destroy(needles[i].GetComponent<NonBouncer>());
                var tink = UnityEngine.Object.Instantiate(GameObject.Find("Needle Tink")).AddComponent<ModCommon.NeedleTink>();
                tink.SetParent(needles[i].transform);
                needles[i].transform.SetPosition2D(gameObject.transform.GetPositionX(), gameObject.transform.GetPositionY());
                needles[i].GetComponent<Rigidbody2D>().velocity = new Vector2(-15f, i * 5f);
                needles[i].SetActive(false);
            }

            Log("fin.");
        }
        public void grubberAttack()
        {
      
            grubR = Instantiate(DoH.grubRPref);
            _beamControlR = grubR.LocateMyFSM("Control");
            _beamControlR.GetAction<Wait>("Active", 0).time = 5f;
            _beamControlR.ChangeTransition("Active", "DEALT DAMAGE", "Active");
            Destroy(grubR.LocateMyFSM("damages_enemy"));
            grubR.transform.SetPosition2D(gameObject.transform.GetPositionX(), gameObject.transform.GetPositionY());
            grubR.AddComponent<DamageHero>();
            grubR.GetComponent<DamageHero>().damageDealt *= 2;
            grubR.SetActive(true);

            grubL = Instantiate(DoH.grubLPref);
            _beamControlL = grubL.LocateMyFSM("Control");
            _beamControlL.GetAction<Wait>("Active", 0).time = 5f;
            _beamControlL.ChangeTransition("Active", "DEALT DAMAGE", "Active");
            Destroy(grubL.LocateMyFSM("damages_enemy"));
            grubL.transform.SetPosition2D(gameObject.transform.GetPositionX(), gameObject.transform.GetPositionY());
            grubL.AddComponent<DamageHero>();
            grubL.GetComponent<DamageHero>().damageDealt *= 2;
            grubL.SetActive(true);
            
        }
        public void grubberAttack2()
        {
            if (gameObject.transform.localPosition.x < 0)
            {
                grubR = Instantiate(DoH.grubRPref);
                _beamControlR = grubR.LocateMyFSM("Control");
                _beamControlR.GetAction<Wait>("Active", 0).time = 5f;
                _beamControlR.ChangeTransition("Active", "DEALT DAMAGE", "Active");
                Destroy(grubR.LocateMyFSM("damages_enemy"));
                grubR.transform.SetPosition2D(gameObject.transform.GetPositionX(), HeroController.instance.gameObject.transform.GetPositionY());
                grubR.AddComponent<DamageHero>();
                grubR.GetComponent<DamageHero>().damageDealt *= 2;
                grubR.SetActive(true);
            }
            else
            {
                grubL = Instantiate(DoH.grubLPref);
                _beamControlL = grubL.LocateMyFSM("Control");
                _beamControlL.GetAction<Wait>("Active", 0).time = 5f;
                _beamControlL.ChangeTransition("Active", "DEALT DAMAGE", "Active");
                Destroy(grubL.LocateMyFSM("damages_enemy"));
                grubL.transform.SetPosition2D(gameObject.transform.GetPositionX(), HeroController.instance.gameObject.transform.GetPositionY());
                grubL.AddComponent<DamageHero>();
                grubL.GetComponent<DamageHero>().damageDealt *= 2;
                grubL.SetActive(true);
            }
            
        }
        private void Update()
        {
            if (_hm.hp <= 1150)
            {
                var wX = weaver.transform.GetPositionX();
                var wY = weaver.transform.GetPositionY();
                var hX = gameObject.transform.GetPositionX();
                var hY = gameObject.transform.GetPositionY();
                if (!secondPhase)
                {
                    Log("Add Weaver Boios");
                    _control.InsertMethod("Sphere", 0, createWeaver);
                    _control.InsertMethod("Sphere A", 0, createWeaver);
                    textExample.text = "Mother forgive my inaction.";
                    secondPhase = true;
                    IEnumerator needleSpread()
                    {
                        if (gameObject.transform.localPosition.x < 0 && !dang)
                        {
                            for (int i = 0; i < needles.Length; i++)
                            {
                                needles[i].SetActive(true);
                                needles[i].transform.SetPosition2D(gameObject.transform.GetPositionX(), gameObject.transform.GetPositionY());
                                needles[i].GetComponent<Rigidbody2D>().velocity = new Vector2(30f, i * 4f);
                                needles[i].GetComponent<Rigidbody2D>().rotation = (Mathf.Rad2Deg * (Mathf.Atan(needles[i].GetComponent<Rigidbody2D>().velocity.y / needles[i].GetComponent<Rigidbody2D>().velocity.x))) + 180f;
                                yield return new WaitForSeconds(0.01f);
                            }
                        }
                        else if (!dang)
                        {
                            for (int i = 0; i < needles.Length; i++)
                            {
                                Log("1What happened?");
                                needles[i].SetActive(true);
                                needles[i].transform.SetPosition2D(gameObject.transform.GetPositionX(), gameObject.transform.GetPositionY());
                                needles[i].GetComponent<Rigidbody2D>().velocity = new Vector2(-30f, i * 4f);
                                needles[i].GetComponent<Rigidbody2D>().rotation = Mathf.Rad2Deg * (Mathf.Atan(needles[i].GetComponent<Rigidbody2D>().velocity.y / needles[i].GetComponent<Rigidbody2D>().velocity.x));
                                yield return new WaitForSeconds(0.01f);
                            }
                        }
                        dang = true;
                    }
                    _control.InsertCoroutine("Throw", 0, needleSpread);
                }
                if (Mathf.Abs(wX - hX) >= 0 && Mathf.Abs(wX - hX) <= 1 && Mathf.Abs(wY - hY) >= 0 && Mathf.Abs(wY - hY) <= 1 && _hm.hp <= 1400)
                {
                    _hm.hp += 5;
                }
                if ((needles[9].transform.GetPositionX() > 40 || needles[9].transform.GetPositionX() < 12) && dang)
                {
                    dang = false;
                    for (int i = 0; i < needles.Length; i++)
                    {
                        needles[i].transform.SetPosition2D(i * 2.6f + 15f, 45);
                        needles[i].GetComponent<Rigidbody2D>().rotation = 90f;
                        needles[i].GetComponent<Rigidbody2D>().velocity = new Vector2(0, -30f);
                    }
                }
            }
            if (_hm.hp <= 800)
            {
                if (!finalPhase)
                {
                    wave = Instantiate(DoH.wavePref);
                    wave.SetActive(true);

                    needle.SetActive(false);
                    needle2.SetActive(false);
                    Log("Do da grubber throw boiu");
                    _control.InsertMethod("CA Recover", 0, grubberAttack);
                    _control.ChangeTransition("Move Choice B", "G DASH", "CA Antic");

                    IEnumerator GrubBoiThrow()
                    {
                        yield return new WaitForSeconds(0.1f);
                        grubberAttack2();
                        yield return new WaitForSeconds(0.1f);
                        grubberAttack2();
                        yield return new WaitForSeconds(0.1f);
                        grubberAttack2();
                        yield return new WaitForSeconds(0.1f);
                        grubberAttack2();
                    }
                    _control.InsertCoroutine("Jump2", 0, GrubBoiThrow);
                    IEnumerator GrubFill()
                    {
                        height = gameObject.transform.GetPositionY() + 5f;
                        for (int i = 0, a = 0; i < 5; i++, a += 2)
                        {
                            grubAll[i] = Instantiate(DoH.grubRPref);
                            _beamControlR = grubAll[i].LocateMyFSM("Control");
                            _beamControlR.GetAction<Wait>("Active", 0).time = 5f;
                            _beamControlR.ChangeTransition("Active", "DEALT DAMAGE", "Active");
                            var initFsm = _beamControlR.GetAction<SetVelocity2d>("Init", 7);
                            _beamControlR.AddAction("Active", new SetVelocity2d
                            {
                                gameObject = initFsm.gameObject,
                                vector = initFsm.vector,
                                x = 20,
                                y = -5,
                                everyFrame = false
                            });
                            Destroy(grubAll[i].LocateMyFSM("damages_enemy"));
                            grubAll[i].transform.SetPosition2D(gameObject.transform.GetPositionX(), height - a);
                            grubAll[i].AddComponent<DamageHero>();
                            grubAll[i].GetComponent<DamageHero>().damageDealt *= 2;
                            grubAll[i].SetActive(true);
                            grubAll[i].GetComponent<Rigidbody2D>().velocity = new Vector2(15f, 0);
                            yield return null;
                        }
                        height = gameObject.transform.GetPositionY() + 5f;
                        for (int i = 5, a = 0; i < 10; i++, a += 2)
                        {
                            grubAll[i] = Instantiate(DoH.grubLPref);
                            _beamControlL = grubAll[i].LocateMyFSM("Control");
                            _beamControlL.GetAction<Wait>("Active", 0).time = 5f;
                            _beamControlL.ChangeTransition("Active", "DEALT DAMAGE", "Active");
                            var initFsm = _beamControlL.GetAction<SetVelocity2d>("Init", 7);
                            _beamControlL.AddAction("Active", new SetVelocity2d
                            {
                                gameObject = initFsm.gameObject,
                                vector = initFsm.vector,
                                x = -20,
                                y = -5,
                                everyFrame = false
                            });
                            Destroy(grubAll[i].LocateMyFSM("damages_enemy"));
                            grubAll[i].transform.SetPosition2D(gameObject.transform.GetPositionX(), height - a);
                            grubAll[i].AddComponent<DamageHero>();
                            grubAll[i].GetComponent<DamageHero>().damageDealt *= 2;
                            grubAll[i].SetActive(true);
                            grubAll[i].GetComponent<Rigidbody2D>().velocity = new Vector2(15f, 0);
                            yield return null;
                        }
                    }
                    _control.InsertCoroutine("Sphere Recover", 0, GrubFill);
                    _control.InsertCoroutine("Sphere Recover A", 0, GrubFill);
                    textExample.text = "I can't let you win ghost.";
                    finalPhase = true;
                }
                //Make lava particle follow her
                wave.transform.SetPosition2D(gameObject.transform.GetPositionX(), gameObject.transform.GetPositionY());
            }
            if (needle.transform.GetPositionX() <= 12)
            {
                needle.transform.SetPosition2D(12, heightNeedle1);
                needle.GetComponent<Rigidbody2D>().rotation = 180;
                needle.GetComponent<Rigidbody2D>().velocity = new Vector2(needleVelocity, 0);
            }
            else if (needle.transform.GetPositionX() >= 40)
            {
                needle.GetComponent<Rigidbody2D>().rotation = 0;
                needle.GetComponent<Rigidbody2D>().velocity = new Vector2(-1 * needleVelocity, 0);
            }
            if (needle2.transform.GetPositionX() <= 12)
            {
                needle2.transform.SetPosition2D(12, heightNeedle2);
                needle2.GetComponent<Rigidbody2D>().rotation = 180f;
                needle2.GetComponent<Rigidbody2D>().velocity = new Vector2(needleVelocity, 0);
            }
            else if (needle2.transform.GetPositionX() >= 40)
            {
                needle2.GetComponent<Rigidbody2D>().rotation = 0f;
                needle2.GetComponent<Rigidbody2D>().velocity = new Vector2(-1 * needleVelocity, 0);
            }

            if (HeroController.instance.cState.focusing && _control.transform.GetPositionY() < 29)
            {
                textExample.text = "Healing is for the weak ghost";
                _control.SetState("G Dash 2");
            }

            if (!textExample.text.Equals(""))
            {
                timeLeft -= Time.deltaTime;
            }
            if (timeLeft <= 0f)
            {
                textExample.text = "";
                timeLeft = 2f;
            }
        }

        private void OnDestroy()
        {
            Destroy(canvas);
        }
        public void createWeaver()
        {
            Log("Getting weavers to spawn");
            if (!weaver.activeSelf)
            {
                weaver.transform.SetPosition2D(gameObject.transform.GetPositionX(), gameObject.transform.GetPositionY());
                weaver.AddComponent<DamageEnemies>().damageDealt = 0;
                weaver.SetActive(true);
            }
        }
        private static void Log(object obj)
        {
            Logger.Log("[Daughter of Hallownest] " + obj);
        }
    }
}