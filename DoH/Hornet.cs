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
using System.IO;

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
        private PlayMakerFSM _beamControlR;
        private PlayMakerFSM _beamControlL;
        private GameObject canvas;
        private GameObject[] needles = new GameObject[10];

        private Text textExample;

        private GameObject grubR;
        private GameObject grubL;
        private GameObject[] grubAll = new GameObject[10];
        private GameObject wave;
        private GameObject go1;
        private GameObject go2;

        float timeLeft;
        float height;
        bool dang;
        private bool finalPhase = false;
        private bool secondPhase = false;
        private bool firstPhase = true;

        public void grubberAttack()
        {
            if (secondPhase)
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
        }
        GameObject[] beamR = new GameObject[5];
        GameObject[] beamL = new GameObject[5];
        public IEnumerator grubberAttack2()
        {
            if (secondPhase)
            {
                _control.ChangeTransition("Move Choice A", "SPHERE A", "Set ADash");
                _control.ChangeTransition("Move Choice B", "SPHERE A", "Set ADash");
                _control.ChangeTransition("Move Choice A", "THROW", "GDash Antic");

                _control.ChangeTransition("G Sphere?", "SPHERE G", "Move Choice B");
                _control.ChangeTransition("Move Choice A", "THROW", "GDash Antic");

                if (gameObject.transform.localPosition.x < 0)//&& !dang)
                {
                    for (int i = 0; i < beamR.Length; i++)
                    {
                        beamR[i] = Instantiate(DoH.grubLPref);
                        _beamControlR = beamR[i].LocateMyFSM("Control");
                        _beamControlR.GetAction<Wait>("Active", 0).time = 5f;
                        _beamControlR.ChangeTransition("Active", "DEALT DAMAGE", "Active");
                        Destroy(beamR[i].LocateMyFSM("damages_enemy"));
                        Destroy(beamR[i].LocateMyFSM("Control"));
                        beamR[i].AddComponent<DamageHero>();
                        beamR[i].GetComponent<DamageHero>().damageDealt *= 2;
                        beamR[i].SetActive(true);
                        beamR[i].transform.SetPosition2D(gameObject.transform.GetPositionX(), gameObject.transform.GetPositionY());
                        beamR[i].GetComponent<Rigidbody2D>().velocity = new Vector2(30f, i * 4f);
                        beamR[i].GetComponent<Rigidbody2D>().rotation = (Mathf.Rad2Deg * (Mathf.Atan(beamR[i].GetComponent<Rigidbody2D>().velocity.y / beamR[i].GetComponent<Rigidbody2D>().velocity.x))) + 180f;
                        yield return new WaitForSeconds(0.01f);
                    }
                }
                else //if (!dang)
                {
                    for (int i = 0; i < beamL.Length; i++)
                    {
                        beamL[i] = Instantiate(DoH.grubRPref);
                        _beamControlL = beamL[i].LocateMyFSM("Control");
                        _beamControlL.GetAction<Wait>("Active", 0).time = 5f;
                        _beamControlL.ChangeTransition("Active", "DEALT DAMAGE", "Active");
                        Destroy(beamL[i].LocateMyFSM("damages_enemy"));
                        Destroy(beamL[i].LocateMyFSM("Control"));
                        beamL[i].AddComponent<DamageHero>();
                        beamL[i].GetComponent<DamageHero>().damageDealt *= 2;
                        beamL[i].SetActive(true);
                        beamL[i].transform.SetPosition2D(gameObject.transform.GetPositionX(), gameObject.transform.GetPositionY());
                        beamL[i].GetComponent<Rigidbody2D>().velocity = new Vector2(-30f, i * 4f);
                        beamL[i].GetComponent<Rigidbody2D>().rotation = (Mathf.Rad2Deg * (Mathf.Atan(beamL[i].GetComponent<Rigidbody2D>().velocity.y / beamL[i].GetComponent<Rigidbody2D>().velocity.x))) + 180f;
                        yield return new WaitForSeconds(0.01f);
                    }
                }
                yield return new WaitForSeconds(1f);

                _control.ChangeTransition("Move Choice A", "SPHERE A", "Set Sphere A");
                _control.ChangeTransition("Move Choice B", "SPHERE A", "Set Sphere A");
                _control.ChangeTransition("Move Choice A", "THROW", "Throw Antic");

                _control.ChangeTransition("G Sphere?", "SPHERE G", "Sphere Antic G");
                _control.ChangeTransition("Move Choice A", "THROW", "Throw Antic");
            }

        }
        bool trick;
        IEnumerator tricky()
        {
            //_control.ChangeTransition("In Air", "AIRDASH", "In Air");
            yield return new WaitForSeconds(3f);
            //_control.ChangeTransition("In Air", "AIRDASH", "ADash Antic");
            _control.GetAction<WaitRandom>("Idle", 7).timeMax = 0.2f;
            _control.GetAction<WaitRandom>("Idle", 7).timeMin = 0.2f;
            _control.GetAction<Tk2dPlayAnimation>("Idle", 4).clipName = "Idle";
            _control.SetState("CA Antic");
            trick = false;
            HeroController.instance.takeNoDamage = false;
        }

        IEnumerator needleSpread()
        {
            if (firstPhase)
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
                        needles[i].SetActive(true);
                        needles[i].transform.SetPosition2D(gameObject.transform.GetPositionX(), gameObject.transform.GetPositionY());
                        needles[i].GetComponent<Rigidbody2D>().velocity = new Vector2(-30f, i * 4f);
                        needles[i].GetComponent<Rigidbody2D>().rotation = Mathf.Rad2Deg * (Mathf.Atan(needles[i].GetComponent<Rigidbody2D>().velocity.y / needles[i].GetComponent<Rigidbody2D>().velocity.x));
                        yield return new WaitForSeconds(0.01f);
                    }
                }
                dang = true;
            }
        }

        List<GameObject> allParticles = new List<GameObject>();
        IEnumerator FireDash()
        {
            allParticles = new List<GameObject>();
            int i = 0;
            float timer = 0.5f;
            while (_anim.CurrentClip.name == "G Dash" && timer >= 0)
            {
                try
                {
                    allParticles.Add(Instantiate(DoH.wavePref));
                    allParticles[i].SetActive(true); //Make it active
                    Rigidbody2D rb1 = allParticles[i].AddComponent<Rigidbody2D>(); //Add a rigidbody to it
                    rb1.gravityScale = 0f;
                    BoxCollider2D bc1 = allParticles[i].AddComponent<BoxCollider2D>(); //Add a boxcollider2d
                    bc1.isTrigger = true; //Set the boxcollider's isTrigger to true
                    bc1.size = new Vector2(bc1.size.x - 1.2f, bc1.size.y - 1.2f);
                    allParticles[i].AddComponent<DashDeathG>(); //This script destroys the particle after 4 seconds
                    allParticles[i].transform.SetPosition2D(gameObject.transform.GetPositionX(), gameObject.transform.GetPositionY() - 0.8f); //Places the particle
                    i++;
                }
                catch (System.Exception e)
                {
                    Log(e);
                }
                timer -= Time.fixedDeltaTime;
                yield return null;
            }
        }
        //bool beeHere;

        //Try nail arts again except remove fsm this time?
        public void SetMantisThrow()
        {
            for (int i = 0; i < 4; i++)
            {
                var _mantisReal = Instantiate(HornetFinder._mantis);
                _mantisReal.SetActive(true);
                Destroy(_mantisReal.LocateMyFSM("Control"));
                _mantisReal.AddComponent<MantisThrow>();
                _mantisReal.transform.SetPosition2D(12f, gameObject.transform.GetPositionY() + i * 1.5f);
                _mantisReal = Instantiate(HornetFinder._mantis);
                _mantisReal.SetActive(true);
                Destroy(_mantisReal.LocateMyFSM("Control"));
                _mantisReal.AddComponent<MantisThrow>();
                _mantisReal.transform.SetPosition2D(42f, gameObject.transform.GetPositionY() + i * 1.5f);

            }
        }
        bool test = false;
        GameObject testgo;
        RenderTexture testing;
        Texture2D test2d;


    

        IEnumerator randFocus()
        {
            for (int i = 0; i < 4; i++)
            {
                var randX = Random.Range(20f, 35f);
                var randY = Random.Range(23.5f, 35f);
                _focusReal = Instantiate(HornetFinder._focus);
                _focusReal.SetActive(true);
                _focusReal.transform.SetPosition2D(randX, randY);
                
                HeroController.instance.spellControl.gameObject.GetComponent<AudioSource>().PlayOneShot(PVSound.LoadAssets.orbSound);

                try
                {
                    // _focusReal.GetComponentsInChildren<ParticleSystemRenderer>()[0].
                    for (int j = 0; j < _focusReal.GetComponentsInChildren<SpriteRenderer>(true).Length-1; j++) //length-1
                    {
                        
                        //Log(_focusReal.GetComponentsInChildren<SpriteRenderer>()[j].name);
                        _focusReal.GetComponentsInChildren<SpriteRenderer>()[j].sprite = DoH.SPRITES[j];
                        /*var a = _focusReal.GetComponentsInChildren<SpriteRenderer>(true)[j].sprite.texture;
                        Log("1");
                        RenderTexture tmp = RenderTexture.GetTemporary(
                            a.width,
                            a.height,
                            0,
                            RenderTextureFormat.Default,
                            RenderTextureReadWrite.Linear
                            );
                        Log("2");
                        Graphics.Blit(a, tmp);
                        Log(tmp == null);
                        DumpRenderTexture(tmp, "C:/Users/ghaem/Desktop/SavedScreen" + j + ".png");*/

                        // C:/Users/egsha/SavedScreen.png
                    }
                    for (int j = 0; j < _focusReal.GetComponentsInChildren<ParticleSystem>(true).Length; j++)
                    {
                        ParticleSystem.MainModule settings = _focusReal.GetComponentsInChildren<ParticleSystem>(true)[j].main;
                        Log("2 " + _focusReal.GetComponentsInChildren<ParticleSystem>(true)[j].name);
                        settings.startColor = new ParticleSystem.MinMaxGradient(Color.red);
                        Log("3");
                    }
                }
                catch (System.Exception e)
                {
                    Log(e);
                }


                yield return new WaitForSeconds(0.5f);
               
            }
        }



        public void SaveTexture()
        {
            
        }

        public void DumpRenderTexture(RenderTexture rt, string pngOutPath)
        {
            var oldRT = RenderTexture.active;
            Log("3");
            var tex = new Texture2D(rt.width, rt.height);
            Log("4");
            RenderTexture.active = rt;
            tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            tex.Apply();
            Log("5");
            File.WriteAllBytes(pngOutPath, tex.EncodeToPNG());
            RenderTexture.active = oldRT;
            Log("6");
        }

        IEnumerator orbThrow()
        {
            _control.ChangeTransition("Move Choice A", "SPHERE A", "Set ADash");
            _control.ChangeTransition("Move Choice B", "SPHERE A", "Set ADash");
            _control.ChangeTransition("Move Choice A", "THROW", "GDash Antic");

            _control.ChangeTransition("G Sphere?", "SPHERE G", "Move Choice B");
            _control.ChangeTransition("Move Choice A", "THROW", "GDash Antic");

            for (int i = 0; i < 5; i++)
            {
                _orbReal = Instantiate(HornetFinder._orb);
                _orbReal.SetActive(true);
                orbNum = i;
                if (gameObject.transform.localPosition.x < 0)
                {
                    orbDirec = 1;
                }
                else
                {
                    orbDirec = -1;
                }
                Destroy(_orbReal.LocateMyFSM("Orb Control"));
                _orbReal.transform.SetPosition2D(gameObject.transform.GetPositionX(), gameObject.transform.GetPositionY());
                _orbReal.AddComponent<OrbThrow>();
                yield return 5;
            }

            yield return new WaitForSeconds(1f);

            _control.ChangeTransition("Move Choice A", "SPHERE A", "Set Sphere A");
            _control.ChangeTransition("Move Choice B", "SPHERE A", "Set Sphere A");
            _control.ChangeTransition("Move Choice A", "THROW", "Throw Antic");

            _control.ChangeTransition("G Sphere?", "SPHERE G", "Sphere Antic G");
            _control.ChangeTransition("Move Choice A", "THROW", "Throw Antic");
        }
       
        IEnumerator DestroyBeeBrain()
        {
            for (int i = 0; i < 3; i++)
            {
                _beeReal = Instantiate(HornetFinder._bee);
                _beeReal.SetActive(true);
                var dungo = _beeReal.LocateMyFSM("Control");
                dungo.ChangeTransition("Init", "FINISHED", "Swarm Start");
                dungo.ChangeTransition("Swarm", "END", "Swarm");
                dungo.ChangeTransition("Swarm", "SPELL", "Swarm");
                yield return null;
                Destroy(_beeReal.LocateMyFSM("Control"));
                _beeReal.transform.SetPosition2D(17f + i * 8f, 40f);
                _beeReal.AddComponent<BeeControl>();
            }
        }

        IEnumerator FireBatThrow()
        {
            _batReal = Instantiate(HornetFinder._bat);
            var dungo = _batReal.LocateMyFSM("Control");
            dungo.ChangeTransition("Init", "HIGH", "Fire");
            dungo.ChangeTransition("Init", "MID", "Fire");
            dungo.ChangeTransition("Fire", "DISSIPATE", "Fire");
            dungo.ChangeTransition("Fire", "END", "Fire");
            dungo.ChangeTransition("Fire", "ORBIT SHIELD", "Fire");
            dungo.GetAction<SetIsKinematic2d>("Fire", 2).isKinematic = true;
            _batReal.SetActive(true);

            _batReal2 = Instantiate(HornetFinder._bat);
            dungo = _batReal2.LocateMyFSM("Control");
            dungo.ChangeTransition("Init", "HIGH", "Fire");
            dungo.ChangeTransition("Init", "MID", "Fire");
            dungo.ChangeTransition("Fire", "DISSIPATE", "Fire");
            dungo.ChangeTransition("Fire", "END", "Fire");
            dungo.ChangeTransition("Fire", "ORBIT SHIELD", "Fire");
            dungo.GetAction<SetIsKinematic2d>("Fire", 2).isKinematic = true;
            _batReal2.SetActive(true);
            yield return null;
            Destroy(_batReal.LocateMyFSM("Control"));
            Destroy(_batReal2.LocateMyFSM("Control"));
            _batReal.transform.SetPosition2D(gameObject.transform.GetPositionX(), gameObject.transform.GetPositionY());
            _batReal2.transform.SetPosition2D(gameObject.transform.GetPositionX(), gameObject.transform.GetPositionY());
            if (gameObject.transform.localScale.x > 0)
            {
                batDirec = -1;
            }
            else
            {
                batDirec = 1;
            }
            _batReal.AddComponent<FireBatThrow>();
            _batReal2.AddComponent<FireBatThrow2>();
        }

        PlayMakerFSM _sceneFSM;
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
            UnityEngine.Object.DontDestroyOnLoad(canvas); //600 50
            textExample = CanvasUtil.CreateTextPanel(canvas, "", 30, TextAnchor.MiddleLeft, new CanvasUtil.RectData(new Vector2(700, 100), new Vector2(-560, 805), new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0.5f, 0.5f)), true).GetComponent<Text>();
            textExample.color = new Color(1f, 1f, 1f, 1f);

            timeLeft = 2f;

            // No stunning
            Destroy(_stunControl);

            // 1500hp
            _hm.hp = 1800;

            // Disable Knockback
            _recoil.enabled = false;

            // Speed up some attacks.
            try
            {
                foreach (KeyValuePair<string, float> i in _fpsDict)
                {
                    _anim.GetClipByName(i.Key).fps = i.Value;
                }

                Log("attempt at music was made");
                //_sceneFSM.ChangeTransition("Statue Level", "1", "Boss Statue Alt");
                Log("attempt at music was made2");
                //_sceneFSM.RemoveAction("Boss Statue Alt", 0);
                
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
            go1 = _control.GetAction<ActivateGameObject>("Sphere Recover A", 1).gameObject.GameObject.Value;
            go2 = _control.GetAction<ActivateGameObject>("Sphere Recover", 1).gameObject.GameObject.Value;
            IEnumerator ActivateSphereA()
            {
                _control.ChangeTransition("In Air", "AIRDASH", "Land");
                if (firstPhase)
                {
                    Log("Hornet: Activate Air turbo sphere mode, DIE little Ghost.");
                    go1.SetActive(true);
                    _control.ChangeTransition("Move Choice A", "SPHERE A", "GDash Antic");
                    _control.ChangeTransition("Move Choice B", "SPHERE A", "CA Antic");
                    _control.ChangeTransition("Move Choice A", "THROW", "GDash Antic");
                    yield return new WaitForSeconds(3f);
                    go1.SetActive(false); 
                    _control.ChangeTransition("Move Choice A", "SPHERE A", "Set Sphere A");
                    _control.ChangeTransition("Move Choice B", "SPHERE A", "Set Sphere A");
                    _control.ChangeTransition("Move Choice A", "THROW", "Throw Antic");
                }
                else if (secondPhase)
                {
                    //This might cause problems in the future
                    _control.ChangeTransition("Move Choice A", "SPHERE A", "Set ADash");
                    _control.ChangeTransition("Move Choice B", "SPHERE A", "Set ADash");
                    _control.ChangeTransition("Move Choice A", "THROW", "GDash Antic");
                    go1.SetActive(false);
                    yield return new WaitForSeconds(1f);
                    _control.ChangeTransition("Move Choice A", "THROW", "Throw Antic");
                    _control.ChangeTransition("Move Choice A", "SPHERE A", "Set Sphere A");
                    _control.ChangeTransition("Move Choice B", "SPHERE A", "Set Sphere A");
                }
                else if (finalPhase)
                {
                    //This might cause problems in the future
                    _control.ChangeTransition("Move Choice A", "SPHERE A", "Set ADash");
                    _control.ChangeTransition("Move Choice B", "SPHERE A", "Set ADash");
                    _control.ChangeTransition("Move Choice A", "THROW", "GDash Antic");
                    _control.GetAction<WaitRandom>("Idle", 7).timeMax = 1.2f;
                    _control.GetAction<WaitRandom>("Idle", 7).timeMin = 1.2f;
                    go1.SetActive(false);
                    yield return new WaitForSeconds(1.2f);
                    _control.ChangeTransition("Move Choice A", "THROW", "Throw Antic");
                    _control.ChangeTransition("Move Choice A", "SPHERE A", "Set Sphere A");
                    _control.ChangeTransition("Move Choice B", "SPHERE A", "Set Sphere A");
                    _control.GetAction<WaitRandom>("Idle", 7).timeMax = 0.2f;
                    _control.GetAction<WaitRandom>("Idle", 7).timeMin = 0.2f;
                }
                _control.ChangeTransition("In Air", "AIRDASH", "ADash Antic");
                yield return new WaitForSeconds(1f);
                canSphere = false;
            }
            IEnumerator ActivateSphereG()
            {
                if (firstPhase)
                {
                    Log("Hornet: Activate Ground turbo sphere mode, DIE little Ghost.");
                    go2.SetActive(true);
                    _control.ChangeTransition("G Sphere?", "SPHERE G", "Move Choice B");
                    _control.ChangeTransition("Move Choice A", "THROW", "GDash Antic");
                    yield return new WaitForSeconds(3f);
                    go2.SetActive(false);
                    _control.ChangeTransition("G Sphere?", "SPHERE G", "Sphere Antic G");
                    _control.ChangeTransition("Move Choice A", "THROW", "Throw Antic");
                }
                else if (secondPhase)
                {
                    //This might cause problems in the future
                    _control.ChangeTransition("Move Choice A", "THROW", "GDash Antic");
                    _control.ChangeTransition("G Sphere?", "SPHERE G", "Move Choice B");
                    go2.SetActive(false);
                    yield return new WaitForSeconds(1f);
                    _control.ChangeTransition("G Sphere?", "SPHERE G", "Sphere Antic G");
                    _control.ChangeTransition("Move Choice A", "THROW", "Throw Antic");
                }
                else if (finalPhase)
                {
                    //This might cause problems in the future
                    _control.ChangeTransition("Move Choice A", "THROW", "GDash Antic");
                    _control.ChangeTransition("G Sphere?", "SPHERE G", "Move Choice B");
                    go2.SetActive(false);
                    _control.GetAction<WaitRandom>("Idle", 7).timeMax = 1.2f;
                    _control.GetAction<WaitRandom>("Idle", 7).timeMin = 1.2f;
                    
                    yield return new WaitForSeconds(1.2f);

                    _control.GetAction<WaitRandom>("Idle", 7).timeMax = 0.2f;
                    _control.GetAction<WaitRandom>("Idle", 7).timeMin = 0.2f;
                    _control.ChangeTransition("G Sphere?", "SPHERE G", "Sphere Antic G");
                    _control.ChangeTransition("Move Choice A", "THROW", "Throw Antic");
                }
            }
            _control.CopyState("Sphere Recover", "Sphere Recover Old");
            _control.CopyState("Sphere Recover A", "Sphere Recover A Old");
            _control.RemoveAction("Sphere Recover A", 1);
            _control.RemoveAction("Sphere Recover", 1);
            _control.InsertCoroutine("Sphere Recover A", 1, ActivateSphereA);
            _control.InsertCoroutine("Sphere Recover", 1, ActivateSphereG);

            //Make Hornet Dash after needle throw
            var removeNeedle = _control.GetAction<ActivateGameObject>("Throw Recover", 0);
            try
            {
                Log("is throw working?");
                _control.GetAction<SetVelocityAsAngle>("Throw", 7).speed = 30f;
            }
            catch(System.Exception e)
            {
                Log(e);
            }
           // _control.InsertAction("Throw Antic", removeNeedle, 0);
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

            Log("Remove Evade when hit because it's dumb and also makes it so when hit hornet has a higher chance of either attacking or jumping");
            _control.GetAction<SendRandomEvent>("Dmg Response", 0).weights[0] = 0f;
            _control.GetAction<SendRandomEvent>("Dmg Response", 0).weights[1] = 0.5f;
            _control.GetAction<SendRandomEvent>("Dmg Response", 0).weights[2] = 0f;
            _control.GetAction<SendRandomEvent>("Dmg Response", 0).weights[3] = 0.5f;
            _control.ChangeTransition("Dmg Response", "JUMP", "Jump Antic");
            _control.ChangeTransition("Dmg Response", "Counter", "CA Antic");
            _control.RemoveAction("Set Jump Only", 1);
            _control.RemoveAction("Set Jump Only", 0);


            Log("Skip waiting for player to hit her counter and never do the dumb evade move");
            _control.GetAction<Wait>("Counter Stance", 1).time = 0f;
            _control.ChangeTransition("Counter Stance", "FINISHED", "CA Antic");

            Log("Choose Counter over Evade");
            _control.GetAction<SendRandomEvent>("Ev Or Counter", 0).weights[0] = 0f;
            _control.GetAction<SendRandomEvent>("Ev Or Counter", 0).weights[1] = 1f;

            Log("Choose GDash over Evade");
            _control.ChangeTransition("Run", "EVADE", "GDash Antic");

            //Removing useless barbs
            _control.RemoveAction("Barb?", 0);
            _control.ChangeTransition("Barb?", "BARB", "Barb Throw");
            _control.ChangeTransition("Barb Throw", "FINISHED", "Can Throw?");
            _control.GetAction<Wait>("Barb Throw", 2).time = 0.4f;

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
            
            _control.InsertCoroutine("Throw", 0, needleSpread);

            Log("fin.");
            try
            {
                var ball = _control.GetAction<ActivateGameObject>("Sphere A",0).gameObject.GameObject.Value;
                Log("2");
                var ballfsm = ball.LocateMyFSM("Grow");
                Log("3");
                ballfsm.RemoveAction("Grow",0);
            }
            catch(System.Exception e)
            {
                Log(e);
            }

            _control.InsertMethod("Fire", 0,AirDashCounter);
            //_control.InsertMethod("Land Y", 0, ResetADash);
            _control.InsertMethod("In Air", 0, InAirChecker); //Had to increase num by 1
            _control.ChangeTransition("Jump R", "FINISHED", "Land");
            _control.ChangeTransition("Jump L", "FINISHED", "Land");
        }

        

        public void AirDashCounter()
        {
            //aDashC++;
        }


        public void InAirChecker()
        {
            //Log("Air D " + aDashC);
            /*if (aDashC > 1)
            {
                _control.ChangeTransition("In Air", "AIRDASH", "Land");
            }
            else
            {
                _control.ChangeTransition("In Air", "AIRDASH", "ADash Antic");
            }*/
        }

        IEnumerator SphereCheck()
        {
            while (true)
            {
               
                if (gameObject.transform.GetPositionY() < 33.8f)
                {
                   
                    _control.SetState("Sphere Antic A");
                    break;
                }
                yield return null;
            }
        }
        
        GameObject _orbReal;
        GameObject _focusReal;
        GameObject _beeReal;
        GameObject _beeReal2;
        GameObject _batReal;
        GameObject _batReal2;
        GameObject _dashReal;
        public static int aDashC = 0;
        public static int orbNum = 0;
        public static int orbDirec = 1;
        public static int batDirec = 0;
        public static int focusAmount = 0;
        public static int gndOrAir;
        public static float HornetDirect;
        float timeFocusing = 10f;  
        bool isDashing = false;
        bool firstFinal = true;

        //make orb spawn when air dash
        int num = 0;
        bool canSphere;
        float canStart = 2f;
        bool firstFinal2 = false;

        private void FixedUpdate()
        {

            if (canStart > 0)
            {
                canStart -= Time.fixedDeltaTime;
            }
            else if (canStart > -10f)
            {
                canStart = -999f;
                _control.RemoveAction("In Air", 1);
                _control.RemoveAction("In Air", 1);
                _control.GetAction<Wait>("In Air", 3).time = 0.2f;
                _control.ChangeTransition("Do Sphere?", "CANCEL", "ADash Antic");
                _control.RemoveAction("Do Sphere?", 0);
                //_control.RemoveAction("Do Sphere?", 0);
                try
                {
                    _control.GetAction<BoolTest>("Do Sphere?", 0).isTrue = null;
                    _control.InsertCoroutine("Do Sphere?", 1, SphereCheck);
                }
                catch (System.Exception e)
                {
                    Log(e);
                }
            }
            else if (canStart == -999f)
            {
                if (_control.ActiveStateName == "In Air" && !canSphere)
                {
                    num++;
                    // Log("Dont Sphere");
                }
                if (num > 60)
                {
                    canSphere = true;
                    //Log("can sphere");
                    num = 0;
                    _control.ChangeTransition("In Air", "AIRDASH", "Do Sphere?");
                }
            }

            if (finalPhase)
            {
                HeroController.instance.ClearMP();
                if (_hm.hp <= 300 && !firstFinal2)
                {
                    firstFinal2 = true;
                    textExample.text = "I pity you no more...";
                    try
                    {
                        _control.GetAction<FireAtTarget>("Fire", 3).speed = 42f;
                        _control.GetAction<SetVelocityAsAngle>("Fire", 4).speed = 42f;
                        _control.GetAction<SetVelocity2d>("G Dash", 5).x = 30f;
                    }
                    catch (System.Exception e)
                    {
                        Log(e);
                    }
                    _anim.GetClipByName("Sphere Antic A Q").fps = 14;
                    _anim.GetClipByName("Wall Impact").fps = 35;
                    _anim.GetClipByName("A Dash Antic Q").fps = 40;
                    _anim.GetClipByName("G Dash Antic Q").fps = 40;
                }
                if (trick)
                {
                    gameObject.transform.SetPosition2D(29f, 28.6f);
                    HeroController.instance.transform.SetPosition2D(20f, 28.4f);
                }
                else
                {
                    if (firstFinal)
                    {
                        textExample.text = "Stop those worthless spells ghost";
                        _control.ChangeTransition("Throw", "FINISHED", "Throw Recover");
                        //_control.RemoveAction("Throw Recover", 3);
                        _control.ChangeTransition("Throw Recover", "FINISHED", "Jump");
                        try
                        {
                            // _control.GetAction<Wait>("Sphere A", 5).time = 1f;
                            // _control.GetAction<Wait>("Sphere", 5).time = 1f;
                            _control.GetAction<FireAtTarget>("Fire", 3).speed = 36f;
                            _control.GetAction<SetVelocityAsAngle>("Fire", 4).speed = 36f;
                            _control.GetAction<SetVelocity2d>("G Dash", 5).x = 30f;
                        }
                        catch(System.Exception e)
                        {
                            Log(e);
                        }
                        _anim.GetClipByName("Sphere Antic A Q").fps = 14;
                        _anim.GetClipByName("Wall Impact").fps = 35;
                        _anim.GetClipByName("A Dash Antic Q").fps = 34;
                        _anim.GetClipByName("G Dash Antic Q").fps = 34;
                        

                        _control.InsertMethod("Sphere", 0, SetMantisThrow);
                        _control.InsertMethod("Sphere A", 0, SetMantisThrow);

                        _control.InsertCoroutine("Throw", 0, orbThrow);

                        _control.InsertCoroutine("CA Recover", 0, FireBatThrow);
                        _control.AddAction("CA Antic", new FaceObject
                        {
                            objectA = gameObject,
                            objectB = HeroController.instance.gameObject,
                            spriteFacesRight = false,
                            playNewAnimation = false,
                            resetFrame = false,
                            everyFrame = false
                        });

                        try
                        {
                            _control.GetAction<Wait>("G Dash", 9).time = 0.5f;
                           // _control.GetAction<DecelerateXY>("GDash Recover1", 3).decelerationX = 0.0f;
                            //_control.GetAction<DecelerateXY>("GDash Recover2", 2).decelerationX = 0.0f;
                        }
                        catch(System.Exception e)
                        {
                            Log(e);
                        }

                        _control.InsertCoroutine("G Dash", 1, FireDash);
                        var lookAtKnight = _control.GetAction<FaceObject>("GDash Antic", 2);
                        _control.InsertAction("G Dash", new FaceObject
                        {
                            objectA = lookAtKnight.objectA,
                            objectB = lookAtKnight.objectB,
                            spriteFacesRight = lookAtKnight.spriteFacesRight,
                            playNewAnimation = lookAtKnight.playNewAnimation,
                            newAnimationClip = lookAtKnight.newAnimationClip,
                            resetFrame = lookAtKnight.resetFrame,
                            everyFrame = lookAtKnight.everyFrame
                        }, 0);
                        _control.ChangeTransition("GDash Recover2", "FINISHED", "CA Antic");
                        
                        firstFinal = false;
                    }
                    if (timeFocusing <= 0f)
                    {
                        var rand = Random.Range(0, 2);
                        timeFocusing = 12f; 
                        if (rand == 0)
                        {
                            StartCoroutine(randFocus());
                        }
                        else
                        {
                            StartCoroutine(DestroyBeeBrain());
                        }
                    }
                    else
                    {
                        timeFocusing -= Time.fixedDeltaTime;
                    }
                }
            }
            if (_hm.hp <= 1800 && _hm.hp > 1300)
            {
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
            if (_hm.hp <= 1300 && _hm.hp > 800)
            {
                if (!secondPhase)
                {
                    try
                    {
                        // _control.GetAction<Wait>("Sphere A", 5).time = 1f;
                        // _control.GetAction<Wait>("Sphere", 5).time = 1f;
                        _control.GetAction<FireAtTarget>("Fire", 3).speed = 35f;
                        _control.GetAction<SetVelocityAsAngle>("Fire", 4).speed = 35f;
                    }
                    catch (System.Exception e)
                    {
                        Log(e);
                    }
                    _anim.GetClipByName("Wall Impact").fps = 25;
                    _anim.GetClipByName("A Dash Antic Q").fps = 30;

                    _control.GetAction<SetPosition>("Throw", 5).vector = new Vector3(50f, 50f, 0f);
                    _control.RemoveAction("Throw", 5);
                    _control.GetAction<ActivateGameObject>("Throw", 5).activate = false;

                    firstPhase = false;
                    _control.ChangeTransition("Barb?", "BARB", "Can Throw?");
                    wave = Instantiate(DoH.wavePref);
                    wave.SetActive(true);

                    Log("Do da grubber throw boiu");
                    _control.InsertMethod("CA Recover", 0, grubberAttack);
                    _control.ChangeTransition("Move Choice B", "G DASH", "CA Antic");

                    _control.InsertCoroutine("Throw", 0, grubberAttack2);

                    IEnumerator GrubFill()
                    {
                        if (secondPhase)
                        {
                            height = gameObject.transform.GetPositionY() + 5f;
                            for (int i = 0, d = 5, a = 0; i < 5; i++, d++, a += 2)
                            {
                                try
                                {
                                    Log("E for everyone");
                                    
                                    
                                    
                                    grubAll[i] = Instantiate(DoH.grubRPref);
                                    Destroy(grubAll[i].LocateMyFSM("Control"));

                                    Destroy(grubAll[i].LocateMyFSM("damages_enemy"));
                                    grubAll[i].transform.SetPosition2D(gameObject.transform.GetPositionX(), height - a);
                                    grubAll[i].AddComponent<DamageHero>();
                                    grubAll[i].GetComponent<DamageHero>().damageDealt *= 2;
                                    grubAll[i].SetActive(true);
                                    grubAll[i].GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 0f);



                                    height = gameObject.transform.GetPositionY() + 5f;

                                    
                                    
                                    
                                    grubAll[d] = Instantiate(DoH.grubLPref);
                                    Destroy(grubAll[d].LocateMyFSM("Control"));

                                    Destroy(grubAll[d].LocateMyFSM("damages_enemy"));
                                    grubAll[d].transform.SetPosition2D(gameObject.transform.GetPositionX(), height - a);
                                    grubAll[d].AddComponent<DamageHero>();
                                    grubAll[d].GetComponent<DamageHero>().damageDealt *= 2;
                                    grubAll[d].SetActive(true);
                                    grubAll[d].GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 0f);
                                    
                                }
                                catch (System.Exception e)
                                {
                                    Log(e);
                                }
                                yield return null;
                            }
                            
                        }
                        yield return new WaitForSeconds(0.2f);

                        for (int i = 0, a = 5; i < 5; i++, a++)
                        {
                            grubAll[i].GetComponent<Rigidbody2D>().velocity = new Vector2(20f, -5f);
                            grubAll[a].GetComponent<Rigidbody2D>().velocity = new Vector2(-20f, -5f);
                            yield return null;
                        }
                        
                    }

                    _control.InsertCoroutine("Sphere", 0, GrubFill);
                    _control.InsertCoroutine("Sphere A", 0, GrubFill);
                    textExample.text = "I can't let you win ghost.";
                    secondPhase = true;
                }
                //Make lava particle follow her
                wave.transform.SetPosition2D(gameObject.transform.GetPositionX(), gameObject.transform.GetPositionY());
            }

            if (_hm.hp <= 800 )
            {
                if (!finalPhase && _control.ActiveStateName != "Idle")
                {
                    
                    _control.GetAction<WaitRandom>("Idle", 9).timeMax = 3f;
                    _control.GetAction<WaitRandom>("Idle", 9).timeMin = 3f;
                    _control.GetAction<Tk2dPlayAnimation>("Idle", 4).clipName = "Stun";
                    _control.RemoveAction("Idle", 8);
                    _control.RemoveAction("Idle", 7);
                    //_control.SetState("Idle");
                }
                else if (!finalPhase && _control.ActiveStateName == "Idle")
                {
                    HeroController.instance.takeNoDamage = true;
                    secondPhase = false;
                    trick = true;
                    Destroy(wave);
                    textExample.text = "Queens of Hallownest, give me strength";
                    finalPhase = true;

                    gameObject.transform.SetPosition2D(29f, 28.6f);
                    if (gameObject.transform.localScale.x > 0)
                    {
                        gameObject.transform.localScale = new Vector3(-1f * gameObject.transform.localScale.x, gameObject.transform.localScale.y, gameObject.transform.localScale.z);
                    }
                    try
                    {
                        go1.SetActive(false);
                        go2.SetActive(false);
                    }
                    catch (System.Exception e)
                    {
                        Log(e);
                    }
                    StartCoroutine(tricky());
                }
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
        private static void Log(object obj)
        {
            Logger.Log("[Daughter of Hallownest] " + obj);
        }
    }
}