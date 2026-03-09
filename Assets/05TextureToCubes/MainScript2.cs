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
using static UnityEngine.Rendering.DebugUI.Table;


public class MainScript2 : MonoBehaviour
{
    public float rouckup = 4;
    public float nextrouck = 2000000;
    public GameObject loadui;
    public float removewhite = 180f;
    // Start is called before the first frame update
    private int nextImageIndex = 0;
    public GameObject[] ball;
    public Image sprite;
    public Texture2D _textureHolder;
    public Color _colorHolder;
    public Material m;
    string path = "path/to/image";
    byte[] bytes;
    public string filesLocation = "Splash Ocean";
    public List<Texture2D> images = new List<Texture2D>();
    public int listcount = 0;
    int count = 0;
    public TextMeshProUGUI t2;
    string[] Arraypaths = new string[0];

        private bool isLoadingComplete = true;

    public float Waitingtime = 1f;


    int anotherconter = 0;


    public int depth = 30;
    public float spacing = 0.02f;

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

    public GameObject[] Roks;
    List<float> initplace;
    int countt = 0;
    //0.43429
    bool isreadytomove = false;

    public int chunkSize = 5000;
    private string[] allimages;
    private string[] allimagesorg;
    private int replaceIndex = 0;
    private void Start()
    {
        allimagesorg = GetImageFiles();
        allimages = allimagesorg.Take(14).ToArray();
        Debug.Log(allimagesorg.Length + " + " + allimages.Length);
        initplace = new List<float>();
        int i = 0;
        foreach (GameObject Rok in Roks)
        {
            float t = Rok.transform.parent.transform.position.y;
            initplace.Add(t);
            GameObject g = Rok.transform.parent.gameObject;
            g.transform.position = new Vector3(g.transform.position.x, -3.59f, g.transform.position.z);
            i++;
        }
        //InvokeRepeating("loading", 0, 0.5f);
        StartCoroutine(LoadingCoroutine());
        StartCoroutine(SwapImagesRoutine());
    }
    private IEnumerator SwapImagesRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(19f);

            if (allimagesorg.Length <= 14 || !isreadytomove) continue;

            // Refresh full list in case new files were added to folder
            allimagesorg = GetImageFiles();

            if (allimagesorg.Length <= 14) continue;

            // Find next new image (starting after current 14)
            int nextOrgIndex = (14 + replaceIndex) % allimagesorg.Length;
            string newImagePath = allimagesorg[nextOrgIndex];

            // Replace the oldest in current display (at replaceIndex)
            //allimages[replaceIndex] = newImagePath;

            // Load and apply the new image immediately to that rok
            StartCoroutine(ReplaceSingleImage(replaceIndex, newImagePath));

            replaceIndex = (replaceIndex + 1) % 14;
        }
    }

    private IEnumerator ReplaceSingleImage(int rokIndex, string filePath)
    {
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(new Uri(filePath).AbsoluteUri))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success) yield break;

            Texture2D tex = DownloadHandlerTexture.GetContent(www);

            // Clear old layers (keep base plane)
            GameObject rok = Roks[rokIndex];
            //for (int i = rok.transform.childCount - 1; i > 0; i--)
            //{
            //    Destroy(rok.transform.GetChild(i).gameObject);
            //}

            // Apply new texture to this rok only
            yield return StartCoroutine(ApplyTextureOnly(tex, rokIndex));

            MeshRenderer baseMR = rok.transform.GetChild(0).GetComponent<MeshRenderer>();
            Texture newTex = baseMR.material.mainTexture;

            for (int i = 1; i < rok.transform.childCount; i++)
            {
                MeshRenderer mr = rok.transform.GetChild(i).GetComponent<MeshRenderer>();
                if (mr != null) mr.material.mainTexture = newTex;
            }

        }
    }
    private string[] GetImageFiles()
    {
        string fullPath = GetFullFilesLocation();
        return Directory.GetFiles(fullPath, "*.*", SearchOption.AllDirectories)
            .Where(f => new[] { ".jpg", ".jpeg", ".png", ".bmp", ".webp" }
            .Contains(Path.GetExtension(f).ToLowerInvariant()))
            .ToArray();
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
            GameObject g = Roks[countt].transform.parent.gameObject;
            LeanTween.cancel(g);
            LeanTween.moveY(g, initplace[countt], rouckup).setEase(LeanTweenType.easeOutCirc).setOnComplete(() =>
            {
                if (anotherconter + 1 == count)
                    isreadytomove = true;
                LeanTween.moveY(g, g.transform.position.y - 0.15f, 1.8f).setEase(LeanTweenType.easeInOutSine).setLoopPingPong();
            });
            anotherconter++;
            countt = (countt + 1) % 14;
        }

    }
    public void loading()
    {

        images.Clear();
        try
        {


            //string[] allimages = Directory.GetFiles(filesLocation, "*.JPG");

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
        isLoadingComplete = true;
        if (images.Count > 0)
        {
            StartCoroutine(ApplyTexturesThenBuild3D());
        }
    }
    private IEnumerator ApplyTexturesThenBuild3D()
    {
        List<Texture2D> imagesCopy = new List<Texture2D>(images);
        int index = 0;
        foreach (Texture2D texture in imagesCopy)
        {
            yield return StartCoroutine(ApplyTextureOnly(texture, index));
            index++;
            if (index % 10 == 0) yield return null;
        }
        yield return StartCoroutine(BuildAll3DIllusions());
        loadui.SetActive(false);
        AudioSource audioSource = GetComponent<AudioSource>();
        if (audioSource != null && audioSource.mute) audioSource.mute = false;
    }
    private IEnumerator ApplyTextureOnly(Texture2D tex, int idx)
    {
        Texture2D texture = duplicateTexture(tex);
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

        int rokIndex = idx % 14;
        GameObject plane = Roks[rokIndex].transform.GetChild(0).gameObject;
        plane.GetComponent<Renderer>().material = m;
        plane.GetComponent<MeshRenderer>().material.mainTexture = texture;

        count++;
        nextImageIndex = (nextImageIndex + 1) % 14;
        yield return null;
    }
    private IEnumerator BuildAll3DIllusions()
{
    foreach (GameObject rok in Roks)
    {
        if (rok.transform.childCount > 0)
        {
            GameObject plane = rok.transform.GetChild(0).gameObject;
            for (int i = 1; i < depth; i++) // Start from 1 to skip original
            {
                GameObject planeClone = Instantiate(plane, plane.transform.position, plane.transform.rotation, rok.transform);
                planeClone.name = "PlaneLayer_" + i;
                float zPosition = i * spacing;
                planeClone.transform.localPosition = new Vector3(-zPosition, 0, -0.19f);
                if (i % 10 == 0) yield return null;
            }
        }
    }
    anotherconter = 0;
    countt = 0;
    StartCoroutine(call());
}
    private IEnumerator LaunchFishSequentially()
    {
        List<Texture2D> imagesCopy = new List<Texture2D>(images);
        print(imagesCopy.Count + "           lenght");
        foreach (Texture2D texture in imagesCopy)
        {
            GameObject g = Roks[nextImageIndex].transform.parent.gameObject;
            if (g.transform.position.y != -3.59f)
            {
                LeanTween.cancel(g);
                LeanTween.moveY(g, -3.59f, 1.5f).setEase(LeanTweenType.easeOutCirc);
            }
            StartCoroutine(LaunchAFishCoroutine(texture));
            yield return new WaitForSeconds(Waitingtime);
        }
        isLoadingComplete = true;
        AudioSource audioSource = transform.GetComponent<AudioSource>();
        print("***start***");
        if (audioSource.mute)
        {
            loadui.SetActive(false);
            audioSource.mute = false;
        }
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
        yield return new WaitForSeconds(0.2f);
        Transform firstChild = Roks[nextImageIndex].transform.GetChild(0);
        foreach (Transform child in Roks[nextImageIndex].transform)
        {

            if (child != firstChild)
            {
                Destroy(child.gameObject);
            }
        }
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
            //if (i % chunkSize == 0)
            //{
            //    yield return null;
            //}
        }
        print("after loop ");
        texture.SetPixels32(pixels);
        texture.Apply();
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        print("aply trance");
        StartCoroutine(EnterAsync(texture, Roks[nextImageIndex]));
        count++;
        nextImageIndex = (nextImageIndex + 1) % 14;
        yield return null;
    }


    IEnumerator EnterAsync(Texture2D sourceTexture, GameObject parentObject)
    {
        GameObject plane = parentObject.transform.GetChild(0).gameObject;
        print(plane.name);
        plane.GetComponent<Renderer>().material = m;
        plane.transform.GetComponent<MeshRenderer>().material.mainTexture = sourceTexture;
        if (plane == null)
        {
            Debug.LogError("Failed to instantiate plane.");
            yield break; 
        }
        StartCoroutine(Create3DIllusionAsync(plane, parentObject));
        yield return null;
    }
    IEnumerator Create3DIllusionAsync(GameObject plane, GameObject container)
    {
        if (plane == null)
        {
            Debug.LogError("Sprite or Parent Object is not assigned.");
            yield break;
        }
        for (int i = 0; i < depth; i++)
        {
            GameObject planeClone = Instantiate(plane, plane.transform.position, plane.transform.rotation, container.transform);
            planeClone.name = "PlaneLayer_" + i;
            float zPosition = i * spacing;
            planeClone.transform.localPosition = new Vector3(-zPosition, 0, -0.19f);

            if (i % 10 == 0) // Adjust to yield after a certain number of clones
            {
                yield return null;
            }
        }
        print("end3d");
    }

    public void ChangeScene()
    {
        SceneManager.LoadScene(0);
    }
    public void Reset()
    {
        SceneManager.LoadScene(1);
    }



}
