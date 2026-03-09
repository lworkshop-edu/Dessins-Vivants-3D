using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using TMPro;
using System.Threading.Tasks;

using Image = UnityEngine.UI.Image;
using System.Xml.Linq;
using UnityEngine.Networking;
using UnityEngine.Rendering;
using System.Linq;

public class Create_Carpets : MonoBehaviour
{
    public AudioSource Sound;
    public bool mouvecarpet = false;
    public float Nextiterator = 3;
    public GameObject loadui;
    public float removewhite = 180f;
    public Material m;
    public GameObject carpetModel;
    public string filesLocation = "Splash Ocean";
    public List<Texture2D> images = new List<Texture2D>();
    public int listcount = 0;
    public TextMeshProUGUI t2;
    string[] Arraypaths = new string[0];

    public bool isLoadingComplete = true;

    public float Waitingtime = 1f;
    public static Create_Carpets init;

    int anotherconter = 0;

    int countt = 0;
    //0.43429

    private string GetFullFilesLocation()
    {
        // Prefer System's Desktop directory which works across Windows/macOS/Linux
        string desktop = System.Environment.GetFolderPath(System.Environment.SpecialFolder.DesktopDirectory);

        if (string.IsNullOrEmpty(desktop))
        {
            // fallback options
            desktop = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
        }

        if (string.IsNullOrEmpty(desktop))
        {
            desktop = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
        }

        // Final fallback: current directory
        if (string.IsNullOrEmpty(desktop))
        {
            desktop = System.Environment.CurrentDirectory;
        }

        return Path.Combine(desktop, filesLocation);
    }

    private void Start()
    {
        Sound.mute = true;
        if (init != null)
            init = this;
        InvokeRepeating("loading", 0, Nextiterator);
        loadui.SetActive(true);
    }

    public void loading()
    {

        images.Clear();
        try
        {
            string fullPath = GetFullFilesLocation();
            string[] allimages = Directory.GetFiles(fullPath, "*.*", SearchOption.AllDirectories)
                     .Where(file => new string[] { ".jpg", ".jpeg", ".png", ".bmp", ".webp" }
                     .Contains(Path.GetExtension(file).ToLower()))
                     .ToArray();
            GameObject[] allCarpets = FindObjectsOfType<GameObject>()
             .Where(go => go.GetComponent<Renderer>() != null && go.name == Path.GetFileNameWithoutExtension(go.name))
             .ToArray();

            foreach (GameObject carpet in allCarpets)
            {
                bool fileExists = allimages.Any(f => Path.GetFileNameWithoutExtension(f) == carpet.name);
                if (!fileExists)
                {
                    Destroy(carpet);
                }
            }
            if (Arraypaths.Length == 0)
            {
                Arraypaths = new string[allimages.Length];
                Array.Copy(allimages, Arraypaths, allimages.Length);

            }
            else
            {
                for (int i = 0; i < allimages.Length; i++)
                {

                    string element = allimages[i];
                    if (!Array.Exists(Arraypaths, e => e == element))
                    {
                        Array.Resize(ref Arraypaths, Arraypaths.Length + 1);
                        Arraypaths[Arraypaths.Length - 1] = element;
                    }
                    else
                    {

                        for (int i1 = i; i1 < allimages.Length - 1; i1++)
                        {
                            allimages[i1] = allimages[i1 + 1];
                        }
                        Array.Resize(ref allimages, allimages.Length - 1);
                        i--;
                    }

                }
            }
            if (allimages.Length > 0)
                StartCoroutine(LoadAll(allimages));
        }
        catch (DirectoryNotFoundException)
        {

            t2.text = "destination not found";
            StartCoroutine(delayofui());
        }
    }



    public IEnumerator LoadAll(string[] filePaths)
    {
        List<Texture2D> imagesCopy = new List<Texture2D>();
        foreach (string filePath in filePaths)
        {
            using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(new Uri(filePath).AbsoluteUri))
            {
                yield return www.SendWebRequest();
                if (www.result != UnityWebRequest.Result.Success)
                {
                    t2.text = filePath + " error";
                    Debug.LogWarning(filePath + " error");
                }
                else
                {
                    Texture2D t = DownloadHandlerTexture.GetContent(www);
                    if (!images.Contains(t))
                    {
                        imagesCopy.Add(t);
                        SetMaterialAsync(t, filePath);
                        images.Add(t);
                    }
                }
            }

        }
        //if (images.Count > 0)
        //{
        //    LaunchFishSequentially(imagesCopy);
        //}
        StartCoroutine(delayofui());
    }
    private void LaunchFishSequentially(List<Texture2D> imagesCopy)
    {
        foreach (Texture2D texture in imagesCopy)
        {
            LaunchAFishAsync(texture);
        }

        StartCoroutine(delayofui());
    }
    IEnumerator delayofui()
    {
        yield return new WaitForSeconds(1.5f);
        isLoadingComplete = true;
        loadui.SetActive(false);
        Sound.mute = false;
        Sound.Play();
        mouvecarpet = true;
    }
    Texture2D duplicateTexture(Texture2D source)
    {
        RenderTexture renderTex = RenderTexture.GetTemporary(
                 source.width,
                 source.height,
                 0,
                 RenderTextureFormat.Default,
                 RenderTextureReadWrite.Linear);

        Graphics.Blit(source, renderTex);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTex;
        Texture2D readableText = new Texture2D(source.width, source.height);
        readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
        readableText.Apply();
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTex);
        return readableText;
    }


    private void LaunchAFishAsync(Texture2D texture)
    {
        SetMaterialAsync(texture);
    }

    public void SetMaterialAsync(Texture2D texture, string filePath = "")
    {

        texture = duplicateTexture(texture);
        Color32[] pixels = texture.GetPixels32();
        for (int i = 0; i < pixels.Length; i++)
        {
            if (pixels[i].r >= removewhite && pixels[i].g >= removewhite && pixels[i].b >= removewhite)
            {
                pixels[i].a = 0;
            }
        }
        texture.SetPixels32(pixels);
        texture.Apply();
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        EnterAsync(texture, filePath);
    }


    void EnterAsync(Texture2D sourceTexture, string filePath)
    {
        GameObject plane = Instantiate(carpetModel, new Vector3(-80f, 0, 0), carpetModel.transform.rotation);
        plane.name = Path.GetFileNameWithoutExtension(filePath);
        countt++;
        plane.GetComponent<Renderer>().material = m;
        plane.GetComponent<Renderer>().material.mainTexture = sourceTexture;
        if (plane == null)
        {
            Debug.LogError("Failed to instantiate plane.");
            return;
        }
    }


    public void ChangeScene()
    {
        SceneManager.LoadScene(0);
    }
    public void Reset()
    {
        SceneManager.LoadScene(2);
    }

    public void Resetspace()
    {
        SceneManager.LoadScene(3);
    }

    public void ResetCqrpet()
    {
        SceneManager.LoadScene(4);
    }
}
