using System.Collections;
using UnityEngine;

namespace HughUtility
{
    public static class Vibration
    {
        /*
#if UNITY_ANDROID && !UNITY_EDITOR
    public static AndroidJavaClass AndroidPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
    public static AndroidJavaObject AndroidcurrentActivity = AndroidPlayer.GetStatic<AndroidJavaObject>("currentActivity");
    public static AndroidJavaObject AndroidVibrator = AndroidcurrentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
#endif
        public static void Vibrate()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidVibrator.Call("vibrate");
#else
            Handheld.Vibrate();
#endif
        }

        public static void Vibrate(long milliseconds)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidVibrator.Call("vibrate", milliseconds);
#else
            Handheld.Vibrate();
#endif
        }
        public static void Vibrate(long[] pattern, int repeat)
        {


#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidVibrator.Call("vibrate", pattern, repeat);
#else
            Handheld.Vibrate();
#endif
        }

        public static void Cancel()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            AndroidVibrator.Call("cancel");
#endif
        }
        */ 
    } 
}