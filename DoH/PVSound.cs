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
using System.Reflection;
using System.IO;

namespace DoH
{
    internal class PVSound : MonoBehaviour
    {
        public static class LoadAssets
        {
            public static AudioClip orbSound;
            public static AudioClip airStrikeSoundFX;

            public static Texture2D[] gunSprites = new Texture2D[5];

            public static void LoadOrbSound()
            {
                Log("Starting");
                foreach (string res in Assembly.GetExecutingAssembly().GetManifestResourceNames())
                {
                    
                    if (res.EndsWith(".wav"))
                    {
                        Modding.Logger.Log("Found sound effect! Saving it.");
                        Stream audioStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(res);
                        if (audioStream != null)
                        {
                            Log("bullet");
                            byte[] buffer = new byte[audioStream.Length];
                            audioStream.Read(buffer, 0, buffer.Length);
                            audioStream.Dispose();
                            orbSound = WavUtility.ToAudioClip(buffer);
                        }
                    }
                }
            }


        }

        private static void Log(object obj)
        {
            Logger.Log("[Audio PV] " + obj);
        }
    }
}
