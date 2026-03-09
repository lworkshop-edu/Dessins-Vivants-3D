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


public class MainScript : MonoBehaviour
{

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
    public string filesLocation = @"C:\testt";
    public List<Texture2D> images = new List<Texture2D>();
    public int listcount = 0;
    int count = 0;
    public TextMeshProUGUI t2;
    string[] Arraypaths = new string[0];

    private bool isLoadingComplete = true;

    public float Waitingtime = 1f;


    int anotherconter = 0;

    public int depth = 100;
    public float spacing = 0.01f;

    public GameObject[] Roks;
    List<float> initplace;
    int countt = 0;
    //0.43429

    private void Start()
    {

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
        InvokeRepeating("LoadingCoroutine", 0, 3f);
        InvokeRepeating("Loadingcall", 0, 0.5f);
        //StartCoroutine(LoadingCoroutine());

    }
    private void LoadingCoroutine()
    {
        if (isLoadingComplete)
        {
            isLoadingComplete = false;
            loading();
        }
       

    }
    private void Loadingcall()
    {
        if (count != 0 && Roks[countt].transform.childCount != 0 && isLoadingComplete)
            StartCoroutine(call());
    }

    private IEnumerator call()
    {
        yield return new WaitForSeconds(0.5f);
        if (anotherconter < Arraypaths.Length && count > anotherconter)
        {
            GameObject g = Roks[countt].transform.parent.gameObject;
            LeanTween.cancel(g);
            LeanTween.moveY(g, initplace[countt], 2).setEase(LeanTweenType.easeOutCirc).setOnComplete(() =>
            {
                LeanTween.moveY(g, g.transform.position.y - 0.15f, 1.8f).setEase(LeanTweenType.easeInOutSine).setLoopPingPong();
            });
            anotherconter++;
            countt = (countt + 1) % 14;
        }
    }
    public async void loading()
    {

        images.Clear();
        var result = await PerformTaskAsyncloading(Arraypaths);
        Arraypaths = result.Item1;
        string[] allimages = result.Item2;

        try
        {
            if (allimages.Length > 0)
            {
                List<Texture2D> imgs = await PerformTaskAsyncLoadAll(allimages, images);
                images = imgs;
                print(imgs.Count + "           lenght");
                if (images.Count > 0)
                {
                    StartCoroutine(LaunchFishSequentially());
                }
            }
            else
                isLoadingComplete = true;
        }
        catch (DirectoryNotFoundException)
        {

            t2.text = "destination not found";
            isLoadingComplete = true;
        }
    }
    private async Task<(string[], string[])> PerformTaskAsyncloading(string[] Arraypaths)
    {
        string[] allimages = Directory.GetFiles(filesLocation, "*.*", SearchOption.AllDirectories)
                    .Where(file => new string[] { ".jpg", ".jpeg", ".png", ".bmp", ".webp" }
                    .Contains(Path.GetExtension(file).ToLower()))
                    .ToArray();

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
        return (Arraypaths, allimages);
    }
    private async Task<List<Texture2D>> PerformTaskAsyncLoadAll(string[] filePaths, List<Texture2D> images)
    {

        foreach (string filePath in filePaths)
        {
            using (UnityWebRequest www = UnityWebRequestTexture.GetTexture("file:///" + filePath))
            {

                www.SendWebRequest();
                await WaitForRequest(www);
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

        return images;
    }
    private async Task WaitForRequest(UnityWebRequest www)
    {
        while (!www.isDone)
        {
            await Task.Yield();
        }
    }

    IEnumerator LaunchFishSequentially()
    {
        List<Texture2D> imagesCopy = new List<Texture2D>(images);
        print(imagesCopy.Count + "           lenght");
        foreach (Texture2D texture in imagesCopy)
        {

            Texture2D t = duplicateTexture(texture);
            Color32[] pixels = t.GetPixels32();
            int h = texture.height;
            int w = texture.width;
            GameObject g = Roks[nextImageIndex].transform.parent.gameObject;
            if (g.transform.position.y != -3.59f)
            {
                LeanTween.cancel(g);
                LeanTween.moveY(g, -3.59f, 1.5f).setEase(LeanTweenType.easeOutCirc);
            }
            LaunchAFishAsync(t, pixels, h, w);
            yield return new WaitForSeconds(Waitingtime);
        }
        isLoadingComplete = true;
        AudioSource audioSource = transform.GetComponent<AudioSource>();
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

    private async void LaunchAFishAsync(Texture2D texture, Color32[] pixels, int height, int width)
    {
        await SetMaterialAsync(texture, pixels, height, width);
    }




    public async Task SetMaterialAsync(Texture2D texture, Color32[] pixels, int h, int w)
    {
        // Perform texture processing on a background thread
        var result = await Task.Run(() =>
        {
            int right = 0;
            int top = 0;
            int left = w;
            int bottom = h;
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    int index = y * w + x;
                    Color32 color = pixels[index];

                    if (color.r >= removewhite && color.g >= removewhite && color.b >= removewhite)
                    {
                        color.a = 0;
                    }
                    else if (color.a > 0)
                    {
                        if (x < left) left = x;
                        if (x > right) right = x;
                        if (y > top) top = y;
                        if (y < bottom) bottom = y;
                    }

                    pixels[index] = color;
                }
            }
            return Tuple.Create(pixels, left, right, top, bottom);
        });
        Color32[] pixels1 = result.Item1;
        int left = result.Item2;
        int right = result.Item3;
        int top = result.Item4;
        int bottom = result.Item5;

        texture.SetPixels32(pixels1);
        texture.Apply();

        int width = Math.Max(right - left + 1, 1);
        int height = Math.Max(top - bottom + 1, 1);
        Rect t = new Rect(left, bottom, width, height);
        Color[] croppedPixels = texture.GetPixels((int)t.x, (int)t.y, width, height);
        Texture2D croppedTexture = new Texture2D(width, height);
        croppedTexture.SetPixels(croppedPixels);
        croppedTexture.Apply();

        Transform firstChild = Roks[nextImageIndex].transform.GetChild(0);
        foreach (Transform child in Roks[nextImageIndex].transform)
        {

            if (child != firstChild)
            {
                Destroy(child.gameObject);
            }
        }
        croppedTexture.filterMode = FilterMode.Point;
        croppedTexture.wrapMode = TextureWrapMode.Clamp;
        StartCoroutine(EnterAsync(croppedTexture, Roks[nextImageIndex]));
        //StartCoroutine( EnterAsync(texture, Roks[nextImageIndex]));
        nextImageIndex = (nextImageIndex + 1) % 14;

    }

    IEnumerator EnterAsync(Texture2D sourceTexture, GameObject parentObject)
    {
        GameObject plane = parentObject.transform.GetChild(0).gameObject;
        plane.GetComponent<Renderer>().material = m;
        plane.transform.GetComponent<MeshRenderer>().material.mainTexture = sourceTexture;
        if (plane == null)
        {
            Debug.LogError("Failed to instantiate plane.");
            yield break; // Exit the coroutine early if plane instantiation failed
        }
        print("end creat textures");
        StartCoroutine(Create3DIllusionAsync(plane, parentObject));
        count++;
       
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
        }
        print("end creat 3d");
        yield return null;
    }





    public void ChangeScene()
    {
        SceneManager.LoadScene(0);
    }
    public void Reset()
    {
        SceneManager.LoadScene(1);
    }


    Texture2D CropTexture(Texture2D texture, Rect rect)
    {
        Texture2D croppedTexture = new Texture2D((int)rect.width, (int)rect.height);
        for (int y = 0; y < rect.height; y++)
        {
            for (int x = 0; x < rect.width; x++)
            {
                croppedTexture.SetPixel(x, y, texture.GetPixel((int)rect.x + x, (int)rect.y + y));
            }
        }
        croppedTexture.Apply();
        return croppedTexture;
    }


}

//using System.Collections;
//using System.Collections.Generic;
//using System.IO;
//using UnityEngine;
//using UnityEngine.SceneManagement;
//using System;
//using TMPro;
//using System.Threading.Tasks;

//using Image = UnityEngine.UI.Image;
//using System.Xml.Linq;
//using UnityEngine.Networking;
//using UnityEngine.Rendering;


//public class MainScript : MonoBehaviour
//{

//    public GameObject loadui;

//    public float removewhite = 180f;
//    // Start is called before the first frame update
//    private int nextImageIndex = 0;
//    public GameObject[] ball;
//    public Image sprite;
//    public Texture2D _textureHolder;
//    public Color _colorHolder;
//    public Material m;
//    string path = "path/to/image";
//    byte[] bytes;
//    public string filesLocation = @"C:\testt";
//    public List<Texture2D> images = new List<Texture2D>();
//    public int listcount = 0;
//    int count = 0;
//    public TextMeshProUGUI t2;
//    string[] Arraypaths = new string[0];

//    private bool isLoadingComplete = true;

//    public float Waitingtime = 1f;


//    int anotherconter = 0;

//    public int depth = 100;
//    public float spacing = 0.01f;

//    public GameObject[] Roks;
//    List<float> initplace;
//    int countt = 0;
//    //0.43429

//    private void Start()
//    {

//        initplace = new List<float>();
//        int i = 0;
//        foreach (GameObject Rok in Roks)
//        {
//            float t = Rok.transform.parent.transform.position.y;
//            initplace.Add(t);
//            GameObject g = Rok.transform.parent.gameObject;
//            g.transform.position = new Vector3(g.transform.position.x, -3.59f, g.transform.position.z);
//            i++;
//        }
//        InvokeRepeating("LoadingCoroutine", 0, 3f);
//        //StartCoroutine(LoadingCoroutine());

//    }
//    private void LoadingCoroutine()
//    {

//            if (isLoadingComplete)
//            {
//                isLoadingComplete = false;
//                loading();
//            }
//            if (count != 0 && Roks[countt].transform.childCount != 0 && isLoadingComplete)
//                StartCoroutine(call());

//    }

//    private IEnumerator call()
//    {

//        yield return new WaitForSeconds(0.5f);
//        if (anotherconter < Arraypaths.Length && count > anotherconter)
//        {
//            GameObject g = Roks[countt].transform.parent.gameObject;
//            //if (g.transform.position.y != -3.59f)
//            //{
//            //    LeanTween.cancel(g);

//            //    LeanTween.moveY(g, -3.59f, 1.5f).setEase(LeanTweenType.easeOutCirc).setOnComplete(() =>
//            //    {
//            //        LeanTween.moveY(g, initplace[countt], 1.5f).setEase(LeanTweenType.easeOutCirc).setOnComplete(() =>
//            //        {
//            //            LeanTween.moveY(g, g.transform.position.y - 0.15f, 1.8f).setEase(LeanTweenType.easeInOutSine).setLoopPingPong();
//            //        });
//            //    });
//            //}
//            //else
//            //{
//            LeanTween.cancel(g);
//            LeanTween.moveY(g, initplace[countt], 2).setEase(LeanTweenType.easeOutCirc).setOnComplete(() =>
//            {
//                LeanTween.moveY(g, g.transform.position.y - 0.15f, 1.8f).setEase(LeanTweenType.easeInOutSine).setLoopPingPong();
//            });
//            //}
//            anotherconter++;
//            countt = (countt + 1) % 14;
//        }

//    }
//    public void loading()
//    {

//        images.Clear();
//        try
//        {
//            string[] allimages = Directory.GetFiles(filesLocation, "*.JPG");

//            if (Arraypaths.Length == 0)
//            {
//                Arraypaths = new string[allimages.Length];
//                Array.Copy(allimages, Arraypaths, allimages.Length);

//            }
//            else
//            {
//                for (int i = 0; i < allimages.Length; i++)
//                {

//                    string element = allimages[i];
//                    if (!Array.Exists(Arraypaths, e => e == element))
//                    {
//                        Array.Resize(ref Arraypaths, Arraypaths.Length + 1);
//                        Arraypaths[Arraypaths.Length - 1] = element;
//                    }
//                    else
//                    {

//                        for (int i1 = i; i1 < allimages.Length - 1; i1++)
//                        {
//                            allimages[i1] = allimages[i1 + 1];
//                        }
//                        Array.Resize(ref allimages, allimages.Length - 1);
//                        i--;
//                    }

//                }
//            }
//            if (allimages.Length > 0)
//                StartCoroutine(LoadAll(allimages));
//            else
//                isLoadingComplete = true;
//        }
//        catch (DirectoryNotFoundException)
//        {

//            t2.text = "destination not found";
//            isLoadingComplete = true;
//        }
//    }


//    public IEnumerator LoadAll(string[] filePaths)
//    {

//        foreach (string filePath in filePaths)
//        {
//            using (UnityWebRequest www = UnityWebRequestTexture.GetTexture("file:///" + filePath))
//            {
//                yield return www.SendWebRequest();
//                if (www.result != UnityWebRequest.Result.Success)
//                {
//                    t2.text = filePath + " error";
//                    Debug.LogWarning(filePath + " error");
//                }
//                else
//                {
//                    Texture2D t = DownloadHandlerTexture.GetContent(www);
//                    if (!images.Contains(t))
//                    {
//                        images.Add(t);
//                    }
//                }
//            }

//        }
//        if (images.Count > 0)
//        {
//            StartCoroutine(LaunchFishSequentially());
//        }

//    }
//    IEnumerator LaunchFishSequentially()
//    {
//        List<Texture2D> imagesCopy = new List<Texture2D>(images);
//        print(imagesCopy.Count + "           lenght");
//        foreach (Texture2D texture in imagesCopy)
//        {

//            Texture2D t = duplicateTexture(texture);
//            Color32[] pixels = t.GetPixels32();
//            int h = texture.height;
//            int w = texture.width;
//            GameObject g = Roks[nextImageIndex].transform.parent.gameObject;
//            if (g.transform.position.y != -3.59f)
//            {
//                LeanTween.cancel(g);
//                LeanTween.moveY(g, -3.59f, 1.5f).setEase(LeanTweenType.easeOutCirc);
//            }
//            LaunchAFishAsync(t, pixels, h, w);

//            yield return new WaitForSeconds(Waitingtime);
//        }
//        isLoadingComplete = true;
//        AudioSource audioSource = transform.GetComponent<AudioSource>();
//        if (audioSource.mute)
//        {
//            loadui.SetActive(false);
//            audioSource.mute = false;
//        }
//    }
//    Texture2D duplicateTexture(Texture2D source)
//    {
//        RenderTexture renderTex = RenderTexture.GetTemporary(
//                 source.width,
//                 source.height,
//                 0,
//                 RenderTextureFormat.Default,
//                 RenderTextureReadWrite.Linear);

//        Graphics.Blit(source, renderTex);
//        RenderTexture previous = RenderTexture.active;
//        RenderTexture.active = renderTex;
//        Texture2D readableText = new Texture2D(source.width, source.height);
//        readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
//        readableText.Apply();
//        RenderTexture.active = previous;
//        RenderTexture.ReleaseTemporary(renderTex);
//        return readableText;
//    }

//    async void LaunchAFishAsync(Texture2D texture, Color32[] pixels, int height, int width)
//    {
//        await SetMaterialAsync(texture, pixels, height, width);
//    }




//    public async Task SetMaterialAsync(Texture2D texture, Color32[] pixels, int h, int w)
//    {
//        // Perform texture processing on a background thread
//        var result = await Task.Run(() =>
//        {
//            int right = 0;
//            int top = 0;
//            int left = w;
//            int bottom = h;
//            //for (int i = 0; i < pixels.Length; i++)
//            //{
//            //    if (pixels[i].r == 255 && pixels[i].g == 255 && pixels[i].b == 255)
//            //    {
//            //        pixels[i].a = 0;
//            //        hasTransparency = true;
//            //    }
//            //    else if (pixels[i].r >= removewhite && pixels[i].g >= removewhite && pixels[i].b >= removewhite)
//            //    {
//            //        pixels[i].a = 0;
//            //        hasTransparency = true;
//            //    }
//            //}

//            //if (hasTransparency)
//            //{
//            //    texture.SetPixels32(pixels);
//            //    texture.Apply();
//            //}


//            for (int y = 0; y < h; y++)
//            {
//                for (int x = 0; x < w; x++)
//                {
//                    int index = y * w + x;
//                    Color32 color = pixels[index];

//                    if (color.r >= removewhite && color.g >= removewhite && color.b >= removewhite)
//                    {
//                        color.a = 0;
//                    }
//                    else if (color.a > 0)
//                    {
//                        if (x < left) left = x;
//                        if (x > right) right = x;
//                        if (y > top) top = y;
//                        if (y < bottom) bottom = y;
//                    }

//                    pixels[index] = color;
//                }
//            }
//            return Tuple.Create(pixels, left, right, top, bottom);
//        });
//        Color32[] pixels1 = result.Item1;
//        int left = result.Item2;
//        int right = result.Item3;
//        int top = result.Item4;
//        int bottom = result.Item5;

//        texture.SetPixels32(pixels1);
//        texture.Apply();

//        int width = Math.Max(right - left + 1, 1);
//        int height = Math.Max(top - bottom + 1, 1);
//        Rect t = new Rect(left, bottom, width, height);
//        Color[] croppedPixels = texture.GetPixels((int)t.x, (int)t.y, width, height);
//        Texture2D croppedTexture = new Texture2D(width, height);
//        croppedTexture.SetPixels(croppedPixels);
//        croppedTexture.Apply();

//        Transform firstChild = Roks[nextImageIndex].transform.GetChild(0);
//        foreach (Transform child in Roks[nextImageIndex].transform)
//        {

//            if (child != firstChild)
//            {
//                Destroy(child.gameObject);
//            }
//        }
//        croppedTexture.filterMode = FilterMode.Point;
//        croppedTexture.wrapMode = TextureWrapMode.Clamp;
//        StartCoroutine(EnterAsync(croppedTexture, Roks[nextImageIndex]));
//        //StartCoroutine( EnterAsync(texture, Roks[nextImageIndex]));
//        nextImageIndex = (nextImageIndex + 1) % 14;

//    }




//    IEnumerator EnterAsync(Texture2D sourceTexture, GameObject parentObject)
//    {
//        GameObject plane = parentObject.transform.GetChild(0).gameObject;
//        plane.GetComponent<Renderer>().material = m;
//        plane.transform.GetComponent<MeshRenderer>().material.mainTexture = sourceTexture;
//        if (plane == null)
//        {
//            Debug.LogError("Failed to instantiate plane.");
//            yield break; // Exit the coroutine early if plane instantiation failed
//        }
//        StartCoroutine(Create3DIllusionAsync(plane, parentObject));
//        count++;
//        yield return null;
//    }

//    IEnumerator Create3DIllusionAsync(GameObject plane, GameObject container)
//    {
//        if (plane == null)
//        {
//            Debug.LogError("Sprite or Parent Object is not assigned.");
//            yield break;
//        }
//        for (int i = 0; i < depth; i++)
//        {
//            GameObject planeClone = Instantiate(plane, plane.transform.position, plane.transform.rotation, container.transform);
//            planeClone.name = "PlaneLayer_" + i;
//            float zPosition = i * spacing;
//            planeClone.transform.localPosition = new Vector3(-zPosition, 0, -0.19f);
//        }
//        yield return null;
//    }





//    public void ChangeScene()
//    {
//        SceneManager.LoadScene(0);
//    }
//    public void Reset()
//    {
//        SceneManager.LoadScene(1);
//    }


//    Texture2D CropTexture(Texture2D texture, Rect rect)
//    {
//        Texture2D croppedTexture = new Texture2D((int)rect.width, (int)rect.height);
//        for (int y = 0; y < rect.height; y++)
//        {
//            for (int x = 0; x < rect.width; x++)
//            {
//                croppedTexture.SetPixel(x, y, texture.GetPixel((int)rect.x + x, (int)rect.y + y));
//            }
//        }
//        croppedTexture.Apply();
//        return croppedTexture;
//    }


//}
