using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using TMPro;


using Image = UnityEngine.UI.Image;
using System.Xml.Linq;
using UnityEngine.Networking;
using UnityEngine.Rendering.Universal;
using LowPolyUnderwaterPack;


public class MainReef : MonoBehaviour
{
    public float nextrouck = 1;
    public GameObject loadui;
    public float removewhite = 180f;
    private int nextImageIndex = 0;
    public Material m;
    public string filesLocation = "Splash Ocean";
    public List<Texture2D> images = new List<Texture2D>();
    public int listcount = 0;
    int count = 0;
    public TextMeshProUGUI t2;
    string[] Arraypaths = new string[0];

    private bool isLoadingComplete = true;

    public float Waitingtime = 1f;

    public bool isref = false;
    int anotherconter = 0;

    public GameObject[] Roks;
    int countt = 0;
    //0.43429
    private List<string> allImages = new List<string>();
    private int currentIndex = 0;
    public UniversalRendererData urpAsset;

    private string GetFullFilesLocation()
    {
        // Use runtime Desktop lookup which is robust on Windows (including OneDrive) and macOS
        string desktop = System.Environment.GetFolderPath(System.Environment.SpecialFolder.DesktopDirectory);

        if (string.IsNullOrEmpty(desktop))
        {
            desktop = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
        }

        if (string.IsNullOrEmpty(desktop))
        {
            desktop = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
        }

        if (string.IsNullOrEmpty(desktop))
        {
            desktop = System.Environment.CurrentDirectory;
        }

        return Path.Combine(desktop, filesLocation);
    }

    //private void Start()
    //{

    //    if (urpAsset != null)
    //    {
    //        var rendererFeatures = urpAsset.rendererFeatures;
    //    //    var feature = rendererFeatures.Find(f => f is Underwater) as Underwater;
    //    //    if (feature != null)
    //    //    {
    //    //        feature.SetActive(true);
    //    //    }
    //    }

    //    int i = 0;
    //    foreach (GameObject Rok in Roks)
    //    {
    //        //float t = Rok.transform.parent.transform.position.y;
    //        //initplace.Add(t);
    //        //GameObject g = Rok.transform.parent.gameObject;
    //        //g.transform.position = new Vector3(g.transform.position.x, -3.59f, g.transform.position.z);
    //        //i++;
    //    }

    //    //StartCoroutine(LoadingCoroutine());
    //    //StartCoroutine(call());
    //    InvokeRepeating("loading", 0, 3f);

    //}


    private IEnumerator Start()
    {
        if (urpAsset )
        {
            var rendererFeatures = urpAsset.rendererFeatures;
            var feature = rendererFeatures.Find(f => f is Underwater) as Underwater;
            if (feature != null && isref)
            {
                feature.SetActive(true);
            }
            else if(!isref)
            {
                feature.SetActive(false);

            }
        }
        allImages.AddRange(GetImageFiles());

        if (allImages.Count == 0) yield break;

        // Load all images once at start
        yield return StartCoroutine(LoadAllImages());

        // Assign first N to rocks immediately (N = Roks.Length)
        AssignToRocks(0, Roks.Length);

        currentIndex = Roks.Length; // next to use
        if(allImages.Count > Roks.Length)
        // Every 20 seconds: change one rock to next image (cycle forever)
        InvokeRepeating(nameof(ChangeNextRock), 20f, 20f);
    }

    private string[] GetImageFiles()
    {
        string fullPath = GetFullFilesLocation();
        return Directory.GetFiles(fullPath, "*.*", SearchOption.AllDirectories)
            .Where(f => new[] { ".jpg", ".jpeg", ".png", ".bmp", ".webp" }
            .Contains(Path.GetExtension(f).ToLowerInvariant()))
            .ToArray();
    }
    private IEnumerator LoadAllImages()
    {
        string[] paths = allImages.ToArray();
        foreach (string path in paths)
        {
            using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(new Uri(path).AbsoluteUri))
            {
                yield return www.SendWebRequest();
                if (www.result == UnityWebRequest.Result.Success)
                {
                    Texture2D tex = DownloadHandlerTexture.GetContent(www);
                    images.Add(ProcessTexture(tex)); // apply transparency
                }
            }
        }
    }

    private Texture2D ProcessTexture(Texture2D tex)
    {
        tex = duplicateTexture(tex);
        Color32[] pixels = tex.GetPixels32();
        for (int i = 0; i < pixels.Length; i++)
        {
            if (pixels[i].r >= removewhite && pixels[i].g >= removewhite && pixels[i].b >= removewhite)
                pixels[i].a = 0;
        }
        tex.SetPixels32(pixels);
        tex.Apply();
        tex.filterMode = FilterMode.Point;
        tex.wrapMode = TextureWrapMode.Clamp;
        return tex;
    }

    private void AssignToRocks(int startIdx, int count)
    {
        for (int i = 0; i < count && i < Roks.Length; i++)
        {
            int imgIdx = (startIdx + i) % images.Count;
            SetRockTexture(Roks[i], images[imgIdx]);
        }
    }

    private void ChangeNextRock()
    {
        if (images.Count == 0 || Roks.Length == 0) return;

        int rockIdx = currentIndex % Roks.Length;
        int imgIdx = currentIndex % images.Count;

        // Log before change
        Texture2D oldTex = null;
        Renderer rend = Roks[rockIdx].GetComponent<Renderer>();
        if (rend.material.HasProperty("_Image"))
            oldTex = rend.material.GetTexture("_Image") as Texture2D;

        string oldName = oldTex != null ? Path.GetFileNameWithoutExtension(allImages[images.IndexOf(oldTex)]) : "None";
        string newName = Path.GetFileNameWithoutExtension(allImages[imgIdx]);

        Debug.Log($"Rock {rockIdx} (old image: {oldName} at index {images.IndexOf(oldTex)}) replaced with {newName} at index {imgIdx}");

        SetRockTexture(Roks[rockIdx], images[imgIdx]);
        currentIndex++;
    }

    private void SetRockTexture(GameObject rock, Texture2D tex)
    {
        if (!rock.activeSelf) rock.SetActive(true);
        Renderer rend = rock.GetComponent<Renderer>();
        rend.material = m;
        if (rend.material.HasProperty("_Image"))
            rend.material.SetTexture("_Image", tex);
    }

    private IEnumerator LoadingCoroutine()
    {
        while (true)
        {
            if (isLoadingComplete)
            {
                isLoadingComplete = false;
                loading();
            }
            if (count != 0 && Roks[countt].transform.childCount != 0 && isLoadingComplete)
                StartCoroutine(call());
            yield return new WaitForSeconds(nextrouck);
        }
    }
    private IEnumerator call()
    {

        yield return new WaitForSeconds(0.5f);
        if (anotherconter < Arraypaths.Length && count > anotherconter)
        {
            
            anotherconter++;
            countt = (countt + 1) % Roks.Length;
        }

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
            int imageCount = allimages.Length;

            // If number of rocks != number of images, reset everything
           

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
            else
                isLoadingComplete = true;
            if (Arraypaths.Length > imageCount)
            {
                Reset();
                
            }
        }
        catch (DirectoryNotFoundException)
        {

            t2.text = "destination not found";
            isLoadingComplete = true;
        }

    }



    public IEnumerator LoadAll(string[] filePaths)
    {
        print("loadall **************");
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
                        images.Add(t);
                    }
                }
            }

        }
        print("finish loodall **************");
        if (images.Count > 0)
        {
            StartCoroutine(LaunchFishSequentially());
        }
    }
    private IEnumerator LaunchFishSequentially()
    {
        List<Texture2D> imagesCopy = new List<Texture2D>(images);
        print(imagesCopy.Count + "           lenght");
        foreach (Texture2D texture in imagesCopy)
        {
            //GameObject g = Roks[nextImageIndex].transform.parent.gameObject;
            //if (g.transform.position.y != -3.59f)
            //{
            //    LeanTween.cancel(g);
            //    LeanTween.moveY(g, -3.59f, 1.5f).setEase(LeanTweenType.easeOutCirc);
            //}
            StartCoroutine(LaunchAFishCoroutine(texture));
            yield return new WaitForSeconds(Waitingtime);
        }
        isLoadingComplete = true;
        //AudioSource audioSource = transform.GetComponent<AudioSource>();
        print("***start***");
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






    IEnumerator LaunchAFishCoroutine(Texture2D texture)
    {
        yield return new WaitForSeconds(0f);
        print("start mat");
        StartCoroutine(SetMaterialAsync(texture));
    }


    IEnumerator SetMaterialAsync(Texture2D texture)
    {
        print("enter mat");
        texture = duplicateTexture(texture);
        Color32[] pixels = texture.GetPixels32();

        print("befor loop ");

        for (int i = 0; i < pixels.Length; i++)
        {
            if (pixels[i].r >= removewhite && pixels[i].g >= removewhite && pixels[i].b >= removewhite)
            {
                pixels[i].a = 0;
            }
        }
        print("after loop ");
        texture.SetPixels32(pixels);
        texture.Apply();
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        print("aply trance");
        StartCoroutine(EnterAsync(texture, Roks[nextImageIndex]));
        count++;
        nextImageIndex = (nextImageIndex + 1) % Roks.Length;
        yield return null;
    }


    IEnumerator EnterAsync(Texture2D sourceTexture, GameObject parentObject)
    {
       GameObject plane = parentObject;
        if(!parentObject.active)
            parentObject.SetActive(true);
       //GameObject plane = parentObject.transform.GetChild(0).gameObject;
        print(plane.name);
        plane.GetComponent<Renderer>().material = m;
        if (plane.GetComponent<Renderer>().material.HasProperty("_Image"))
        {
            plane.GetComponent<Renderer>().material.SetTexture("_Image", sourceTexture);
        }
        if (plane == null)
        {
            Debug.LogError("Failed to instantiate plane.");
            yield break;
        }
        yield return null;
    }


    public void ChangeScene()
    {
        SceneManager.LoadScene(0);
    }
    public void Reset()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Resetspace()
    {
        SceneManager.LoadScene(3);
    }
}
