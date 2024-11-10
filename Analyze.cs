using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class Analyze : MonoBehaviour
{
    public GameObject loadingMenu;
    public GameObject errorMenu;
    public GameObject errorMenuLoading;
    public static string data;
    public GameObject startGameMenu;
    UnityWebRequest res;
    public GameObject tips;
    bool loaded;
    public int floorCount = 2;
    public List<Texture2D> floorImages;  // List to store 2D floor images
    public List<string> floorData = new List<string>();


    //public void sendToServer()
    //{
    //    StartCoroutine(Upload(false, [], 0));
    //}

    public void sendToServerLoadedImage()
    {
        data = "";
        for (int i = 0; i < floorCount; i++)
        {
            StartCoroutine(Upload(true, "", i));
        }
    }

    //IEnumerator Upload(bool isLoaded, Texture2D image, int floorIndex)
    IEnumerator Upload(bool isLoaded, string test, int floorIndex)
    {
        loaded = isLoaded;
        loadingMenu.SetActive(true);
        tips.SetActive(true);

        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        Texture2D snap;

        //if (!isLoaded)
        //{
        //    WebCamTexture snappedImage = SnappingImage.webCamTexture;
        //    snap = new Texture2D(snappedImage.width, snappedImage.height, TextureFormat.RGB24, false);
        //    snap.SetPixels(snappedImage.GetPixels());
        //    snap.Apply();
        //}
        //else
        //{
        Texture2D texture = Kakera.ImageLoader.tex;
        snap = new Texture2D(texture.width, texture.height, TextureFormat.RGB24, false);
        snap.SetPixels(texture.GetPixels());
        snap.Apply();
        //}

        byte[] bytes = snap.EncodeToPNG();
        formData.Add(new MultipartFormFileSection("image", bytes, "F1_original.png", "image/png"));

        UnityWebRequest www = UnityWebRequest.Post("https://172.20.10.5:5000", formData);

        // Attach the custom certificate handler to bypass SSL verification
        www.certificateHandler = new BypassCertificateHandler();

        yield return www.SendWebRequest();
        res = www;
        if (!res.isNetworkError && !res.isHttpError)
        {
            Debug.Log($"Floor {floorIndex} data received.");
            floorData.Insert(floorIndex, www.downloadHandler.text);  // Store data for each floor
            if (floorData.Count == floorCount)
            {
                BuildMultiFloorBuilding();
            }
        }
        else
        {
            Debug.LogError("Error uploading floor image: " + www.error);
        }
    }

    //private void Update()
    //{
    //    if (res != null && MenuFunc.tipsDone)
    //    {
    //        if (res.isNetworkError || res.isHttpError)
    //        {
    //            loadingMenu.SetActive(false);
    //            if (loaded)
    //            {
    //                errorMenuLoading.SetActive(true);
    //            }
    //            else
    //            {
    //                errorMenu.SetActive(true);
    //            }
    //        }
    //        else
    //        {
    //            Debug.Log("Form upload complete!");
    //            data = res.downloadHandler.text;
    //            data = res.downloadHandler.text;
    //            startGameMenu.SetActive(true);
    //            gameObject.SetActive(false);
    //            tips.SetActive(false);
    //        }
    //        res = null;
    //    }
    //}

    //public void LoadScene()
    //{
    //    GameObject builder = new GameObject("Ya3m ana 3omdaaaaaaa");
    //    builder.AddComponent<Builder>();
    //    //for (int i = 0; i < floorData.Count; i++)
    //    //{
    //    //    GameObject builder = new GameObject($"Floor_{i}_Builder");
    //    //    Builder builderComponent = builder.AddComponent<Builder>();
    //    //    builderComponent.data = floorData[i];
    //    //    builderComponent.floorIndex = i;
    //    //    builderComponent.BuildFloor();
    //    //}
    //}

    public void LoadScene()
    {
        BuildMultiFloorBuilding();
    }

    private void BuildMultiFloorBuilding()
    {
        for (int i = 0; i < floorData.Count; i++)
        {
            GameObject builder = new GameObject($"Floor_{i}_Builder");
            Builder builderComponent = builder.AddComponent<Builder>();
            builderComponent.data = floorData[i];
            builderComponent.floorIndex = i;
            builderComponent.BuildFloor();
        }
    }
}

// Custom CertificateHandler to bypass SSL verification (for development only)
public class BypassCertificateHandler : CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        // Always return true to bypass certificate validation
        return true;
    }
}