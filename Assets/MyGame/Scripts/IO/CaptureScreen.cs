﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CaptureScreen : MonoBehaviour
{
    public GameObject captureObj;

    public float waitTime = 1f;
    float currTime = 0f;
    bool wait;

    private void Update()
    {
        currTime += Time.deltaTime;

        if(currTime > waitTime)
        {
            wait = false;
        }
    }

    public void Capture()
    {
        if (wait)
            return;
        currTime = 0f;
        wait = true;
        StartCoroutine(CaptureCor(false));
    }
    public void Share()
    {
        if (wait)
            return;
        currTime = 0f;
        wait = true;
        StartCoroutine(CaptureCor(true));
    }

    IEnumerator CaptureCor(bool share)
    {
        yield return new WaitForEndOfFrame();

        byte[] imageByte;
        Texture2D tex = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, true);
        //RenderTexture rt = new RenderTexture(Screen.width, Screen.height, 24);

        //foreach (Camera cam in cams)
        //{
        //    RenderTexture prev = cam.targetTexture;
        //    cam.targetTexture = rt;
        //    cam.Render();
        //    cam.targetTexture = prev;
        //}
        //RenderTexture.active = rt;

        tex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);

        tex.Apply();

        imageByte = tex.EncodeToPNG();

        DestroyImmediate(tex);

        if (share)
        {
            try
            {
                string filePath = Path.Combine(Application.temporaryCachePath, "shared img.png");
                File.WriteAllBytes(filePath, imageByte);

                new NativeShare().AddFile(filePath).SetSubject("소녀전선 제대 시뮬레이터").SetText("- Made By Cosmos0").Share();
                captureObj.SetActive(true);
            }
            catch
            {
                Debug.Log("Share Error");
            }

        }
        else
        {
            try
            {
                var permission = NativeGallery.SaveImageToGallery(imageByte, "GFSIM", "HOXY {0}.png");

                if (permission == NativeGallery.Permission.ShouldAsk)
                {
                    NativeGallery.RequestPermission();
                }
                else if (permission == NativeGallery.Permission.Denied)
                {

                }
            }
            catch
            {
                Debug.Log("Capture Error");
            }
        }


    }

    //public void GalleryRefresh(string fileToRefresh)
    //{
    //    if (Application.platform == RuntimePlatform.Android)
    //    {
    //        try
    //        {
    //            Debug.Log("file://" + fileToRefresh);
    //            AndroidJavaClass classPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
    //            AndroidJavaObject objActivity = classPlayer.GetStatic<AndroidJavaObject>("currentActivity");
    //            AndroidJavaClass classUri = new AndroidJavaClass("android.net.Uri");
    //            AndroidJavaObject objIntent = new AndroidJavaObject("android.content.Intent", new object[2] {
    //                "android.intent.action.MEDIA_SCANNER_SCAN_FILE",
    //                classUri.CallStatic<AndroidJavaObject> ("parse", "file://" + fileToRefresh)
    //            });

    //            objActivity.Call("sendBroadcast", objIntent);
    //        }
    //        catch
    //        {

    //            //GameObject.Find("console").GetComponent<Text>().text = "Exception: ";
    //            //GameObject.Find("console").GetComponent<Text>().text += e.Message;

    //        }
    //    }
    //}

}
