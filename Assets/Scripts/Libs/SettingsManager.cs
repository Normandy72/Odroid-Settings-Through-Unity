using System;
using UnityEngine;

public static class SettingsManager
{
    private const string ANDROID_MAIN_CLASS = "com.unity3d.player.UnityPlayer";
    private const string CURRENT_ACTIVITY = "currentActivity";

    ///<summary>Opens Android Settings panel.</summary>
    public static void OpenSettings()
    {
        try
        {   
            if(Application.platform == RuntimePlatform.Android)
            {
                using(AndroidJavaClass unityPlayer = new(ANDROID_MAIN_CLASS))
                {
                    using(AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>(CURRENT_ACTIVITY))
                    {
                        using(AndroidJavaObject intent = new("android.content.Intent"))
                        {
                            intent.Call<AndroidJavaObject>("setAction", "android.settings.SETTINGS");
                            currentActivity.Call("startActivity", intent);
                        }
                    }
                }
            }
        }
        catch(Exception ex)
        {
            Debug.LogError("OPEN SETTINGS EXCEPTION: " + ex.Message);
        }            
    }

    ///<summary>Restarts the application (Android specific).</summary>
    public static void RestartApp()
    {
        try
        {
            if(Application.isEditor) return;                

            AndroidJavaObject activity = new AndroidJavaClass(ANDROID_MAIN_CLASS).GetStatic<AndroidJavaObject>(CURRENT_ACTIVITY);
            AndroidJavaClass pluginClass = new("com.example.usbserialwrapper.RebootApp");
            pluginClass?.CallStatic("restartApp", activity);
        }
        catch(Exception ex)
        {
            Debug.LogError("RESTART APP EXCEPTION: " + ex.Message);
        }
    }

    ///<summary>Restarts the device (Android specific) if it is rooted.</summary>
    ///<remarks>Requires REBOOT and ACCESS_SUPERUSER permission to be granted.</remarks>
    public static void RestartDevice()
    {
        try
        {
            if(Application.isEditor) return;

            AndroidJavaObject activity = new AndroidJavaClass(ANDROID_MAIN_CLASS).GetStatic<AndroidJavaObject>(CURRENT_ACTIVITY);
            AndroidJavaClass pluginClass = new("com.example.usbserialwrapper.RebootDevice");
            pluginClass?.CallStatic("reboot");
        }
        catch(Exception ex)
        {
            Debug.Log("RESTART DEVICE EXCEPTION: " + ex.Message);           
        }
    }

    ///<summary>Turns off the device (Android specific) if it is rooted.</summary>
    ///<remarks>Requires SHUTDOWN and ACCESS_SUPERUSER permission to be granted.</remarks>
    public static void ShutdownDevice()
    {
        try
        {
            if(Application.isEditor) return;

            AndroidJavaObject activity = new AndroidJavaClass(ANDROID_MAIN_CLASS).GetStatic<AndroidJavaObject>(CURRENT_ACTIVITY);
            AndroidJavaClass pluginClass = new("com.example.usbserialwrapper.ShutdownDevice");
            pluginClass?.CallStatic("shutdown");
        }
        catch(Exception ex)
        {
            Debug.Log("SHUTDOWN DEVICE EXCEPTION: " + ex.Message);
        }
    }

    ///<summary>Sets screen brightness using Android API. Use it for Odroid C4.</summary>
    ///<remarks>Requires WRITE_SETTINGS permission to be granted.</remarks>
    ///<param name="newValue">New value of brightness.</param>
    public static void SetScreenBrightness(float newValue)
    {
        try
        {
            if(Application.isEditor) return;

            AndroidJavaObject activity = new AndroidJavaClass(ANDROID_MAIN_CLASS).GetStatic<AndroidJavaObject>(CURRENT_ACTIVITY);

            activity.Call("runOnUiThread", new AndroidJavaRunnable(() => {
                AndroidJavaObject window = activity.Call<AndroidJavaObject>("getWindow");
                AndroidJavaObject attributes = window.Call<AndroidJavaObject>("getAttributes");
                attributes.Set("screenBrightness", newValue); 
                window.Call("setAttributes", attributes);
            }));
        }
        catch(Exception ex)
        {   
            Debug.Log("CHANGE SCREEN BRIGHTNESS EXCEPTION: " + ex.Message);
        }
    }

    ///<summary>Sets screen brightness using Odroid settings. Use it for Odroid N2.</summary>
    ///<remarks>Requires SUPERUSER permission to be granted.</remarks>
    ///<param name="newValue">New value of brightness.</param>
    public static void SetScreenBrightness(int newValue)
    {
        try
        {
            if(Application.isEditor) return;

            AndroidJavaObject activity = new AndroidJavaClass(ANDROID_MAIN_CLASS).GetStatic<AndroidJavaObject>(CURRENT_ACTIVITY);
            AndroidJavaClass pluginClass = new("com.example.usbserialwrapper.BrightnessControl");
            pluginClass?.CallStatic("setBrightness", newValue);
        }
        catch(Exception ex)
        {   
            Debug.Log("CHANGE SCREEN BRIGHTNESS EXCEPTION: " + ex.Message);
        }
    }

    ///<summary>Sets device volume using Android API.</summary>
    ///<remarks>Requires MODIFY_AUDIO_SETTINGS permission to be granted.</remarks>
    ///<param name="newValue">New value of volume.</param>
    public static void SetDeviceVolume(float newValue)
    {
        try
        {
            if(Application.isEditor) return;

            AndroidJavaObject activity = new AndroidJavaClass(ANDROID_MAIN_CLASS).GetStatic<AndroidJavaObject>(CURRENT_ACTIVITY);
            AndroidJavaObject context = activity.Call<AndroidJavaObject>("getApplicationContext");

            using(AndroidJavaClass audioManagerClass = new("android.media.AudioManager"))
            {
                using(AndroidJavaObject audioManager = context.Call<AndroidJavaObject>("getSystemService", context.GetStatic<string>("AUDIO_SERVICE")))
                {
                    int maxVolume = audioManager.Call<int>("getStreamMaxVolume", audioManagerClass.GetStatic<int>("STREAM_MUSIC"));
                    int newVolume = Mathf.Clamp(Mathf.RoundToInt(newValue * maxVolume), 0, maxVolume);
                    audioManager.Call("setStreamVolume", audioManagerClass.GetStatic<int>("STREAM_MUSIC"), newVolume, 0);
                }
            }
        }
        catch(Exception ex)
        {
            Debug.Log("CHANGE DEVICE VOLUME EXCEPTION: " + ex.Message);
        }
    }

    ///<summary>Gets screen brightness using Android API. Use it for Odroid C4.</summary>
    ///<remarks>Requires WRITE_SETTINGS permission to be granted.</remarks>
    ///<returns>Current screen brightness (float value).</returns>
    public static float GetScreenBrightnessFloat()
    {
        float currentBrightness = 0f;

        try
        {
            if(Application.isEditor) return currentBrightness;

            AndroidJavaObject activity = new AndroidJavaClass(ANDROID_MAIN_CLASS).GetStatic<AndroidJavaObject>(CURRENT_ACTIVITY);
            AndroidJavaObject contentResolver = activity.Call<AndroidJavaObject>("getContentResolver");
            AndroidJavaClass system = new("android.provider.Settings$System");
            currentBrightness = system.CallStatic<int>("getInt", contentResolver, "screen_brightness") / 255f;
        }
        catch(Exception ex)
        {   
            Debug.Log("GET SCREEN BRIGHTNESS EXCEPTION: " + ex.Message);
        }

        return currentBrightness;
    }

    ///<summary>Gets screen brightness using Android API. Use it for Odroid N2.</summary>
    ///<remarks>Requires SUPERUSER permission to be granted.</remarks>
    ///<returns>Current screen brightness (int value).</returns>
    public static int GetScreenBrightnessInt()
    {
        int currentBrightness = 0;

        try
        {
            if(Application.isEditor) return currentBrightness;

            AndroidJavaObject activity = new AndroidJavaClass(ANDROID_MAIN_CLASS).GetStatic<AndroidJavaObject>(CURRENT_ACTIVITY);
            AndroidJavaClass pluginClass = new("com.example.usbserialwrapper.BrightnessControl");
            currentBrightness = int.Parse(pluginClass?.CallStatic<string>("readBrightness"));
        }
        catch(Exception ex)
        {   
            Debug.Log("GET SCREEN BRIGHTNESS EXCEPTION: " + ex.Message);
        }

        return currentBrightness;
    }

    ///<summary>Gets device volume using Android API.</summary>
    ///<remarks>Requires MODIFY_AUDIO_SETTINGS permission to be granted.</remarks>
    ///<returns>Current device volume.</returns>
    public static float GetDeviceVolume()
    {
        float currentVolumeNormalized = 0f;

        try
        {
            if(Application.isEditor) return currentVolumeNormalized;

            AndroidJavaObject activity = new AndroidJavaClass(ANDROID_MAIN_CLASS).GetStatic<AndroidJavaObject>(CURRENT_ACTIVITY);
            AndroidJavaObject context = activity.Call<AndroidJavaObject>("getApplicationContext");

            using(AndroidJavaClass audioManagerClass = new("android.media.AudioManager"))
            {
                using(AndroidJavaObject audioManager = context.Call<AndroidJavaObject>("getSystemService", context.GetStatic<string>("AUDIO_SERVICE")))
                {
                    int currentVolume = audioManager.Call<int>("getStreamVolume", audioManagerClass.GetStatic<int>("STREAM_MUSIC"));
                    int maxVolume = audioManager.Call<int>("getStreamMaxVolume", audioManagerClass.GetStatic<int>("STREAM_MUSIC"));

                    currentVolumeNormalized = (float)currentVolume / maxVolume;
                }
            }
        }
        catch(Exception ex)
        {
            Debug.Log("GET DEVICE VOLUME EXCEPTION: " + ex.Message);
        }

        return currentVolumeNormalized;
    }    
}