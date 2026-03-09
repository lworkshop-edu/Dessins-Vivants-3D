using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using ZXing;
using ZXing.QrCode;
using ZXing.Common;
using Battlehub.SplineEditor;
using UnityEngine.SceneManagement;
using System;
using System.Linq;

//using Intel.RealSense;
using Image = UnityEngine.UI.Image;
using UnityEngine.Rendering;

public class NewBehaviourScript : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject [] ball;
    public Image sprite;
    public Texture2D _textureHolder;
    public Color _colorHolder;
    public Material m;
    string path = "path/to/image";
    byte[] bytes ;
    public string filesLocation = "Splash Ocean";
    public List<Texture2D> images = new List<Texture2D>();
    public int listcount = 0;
    int count = 0;
    public Text t2;
    public Spline [] sp;
    string[] Arraypaths = new string[0];
    private List<GameObject> activedraw = new List<GameObject>();
    private List<string> activeobjname= new List<string>();
    private List<string> allImages = new List<string>();
    private int currentIndex = 0;
    //0.43429
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

    public void Awake()
    {
        //InvokeRepeating("loading", 1, 10);
        //loading();


    }
    //private IEnumerator Start()
    //{

    //    while (true)
    //    {
    //        loading();
    //         yield return new WaitForSeconds(10f);
    //    }
    //}
    private IEnumerator Start()
    {
        allImages.AddRange(GetImageFiles());
        // Load first 5 images immediately
        yield return StartCoroutine(LoadInitialFive());
        currentIndex = 5;
        // Then start the 30-second loop
        InvokeRepeating(nameof(ProcessNextImage), 20f, 20f);
    }

    private IEnumerator LoadInitialFive()
    {
      
        if (allImages.Count > 0)
        {
            int count = Mathf.Min(5, allImages.Count);
            string[] firstFive = allImages.Take(count).ToArray();

            yield return StartCoroutine(LoadAll(firstFive));
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
    private void ProcessNextImage()
    {
     

        // Find next new file not yet loaded
        //string nextFile = allImages.FirstOrDefault(f =>
        //    !activedraw.Any(go => go.name == Path.GetFileNameWithoutExtension(f)));

        string nextFile = allImages[currentIndex];
        currentIndex = (currentIndex + 1) % allImages.Count;

        if (nextFile != null)
        {
            
            // Load and instantiate new one
            StartCoroutine(LoadAll(new[] { nextFile }));
            if (activedraw.Count > 8)
            {
                Debug.Log("heeeeeeeeere");
                Destroy(activedraw[0]);
                activedraw.RemoveAt(0);
                images.RemoveAt(0);

            }
            // Destroy oldest fish if more than 5 active

        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        //if (ball[0])
        //{
        //    t2.text = ball[0].transform.position.ToString();
        //}
        //if(ball[ball.Length - 1].GetComponent<SplineFollow>())
        //{
        //    //print("eeeeeeeeee" + ball[ball.Length - 1].GetComponent<SplineFollow>().Offset);
        //    //ball[ball.Length - 1].GetComponent<SplineFollow>().IsRunning = false;
        //}
        //loading();




    }
    public void loading()
    {

        print(Arraypaths.Length + "   Arraypaths");
        //listcount = images.Count;
        images.Clear();
        string fullPath = GetFullFilesLocation();
        // StartCoroutine("LoadAll",Directory.GetFiles(ppath, "*.JPG", SearchOption.TopDirectoryOnly));
        try
        {
            //string[] allimages = Directory.GetFiles(filesLocation, "*.JPG");
            string[] allimages = Directory.GetFiles(fullPath, "*.*", SearchOption.AllDirectories)
                    .Where(file => new string[] { ".jpg", ".jpeg", ".png", ".bmp", ".webp" }
                    .Contains(Path.GetExtension(file).ToLower()))
                    .ToArray();
            HashSet<string> currentFileNames = new HashSet<string>(
     allimages.Select(p => Path.GetFileNameWithoutExtension(p)));
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
            if(allimages.Length > 0)
                StartCoroutine("LoadAll", allimages);
            for (int i = 0; i < activedraw.Count; i++)
            {
               
                if (!currentFileNames.Contains(activedraw[i].name))
                {


                    Destroy(activedraw[i]);
                    activedraw.RemoveAt(i);
                   
                }
            }
        }
        catch (DirectoryNotFoundException)
        {

            t2.text = "destination not found";
        }




    }
    public int ReadTheBarcode(Texture2D texture)
    {
        BarcodeReader reader = new BarcodeReader();
        Color32[] pixells = texture.GetPixels32();
        byte[] data = new byte[pixells.Length];

        for (int i = 0; i < pixells.Length; i++)
        {
            Color32 pixel = pixells[i];
            // Pack RGBA values into a single byte value
            byte value = (byte)((pixel.r + pixel.g + pixel.b) / 3);
            data[i] = value;
        }
        Result result = reader.Decode(texture.GetRawTextureData(), texture.width, texture.height, RGBLuminanceSource.BitmapFormat.RGB32);

        // If the QR code was detected, print the result
        if (result != null)
        {
            Debug.Log("QR code detected: " + result.Text);
        }
        return int.Parse(result.Text);
    }
    public IEnumerator LoadAll(string[] filePaths)
    {

        foreach (string filePath in filePaths)
        {
            
            WWW load = new WWW(new Uri(filePath).AbsoluteUri);
            yield return load;
            if (!string.IsNullOrEmpty(load.error))
            {
                t2.text = filePath + " error";
                Debug.LogWarning(filePath + " error");
            }
            else
            {
                //t2.text = "detected";

                Texture2D t = load.texture;
               

                if (!images.Contains(t))
                {

                    images.Add(t);
                    launchafish(t, filePath);
                    count++;
                }
              

            }
            
        }
        //for (int i = 0; i < images.Count; i++)
        //{
        //    images[i]= duplicateTexture(images[i]);
        //    Color32[] pixels = images[i].GetPixels32();

        //    for (int j = 0; j < pixels.Length; j++)
        //    {
        //        if (pixels[j].r == 255 && pixels[j].g == 255 && pixels[j].b == 255)
        //        {
        //            pixels[j].a = 0;
        //        }
        //    }

        //    images[i].SetPixels32(pixels);
        //    images[i].Apply();
        //}


        //print(images.Count + "liiiiiiiist2");
        //count = images.Count;
        //if (images.Count > 0 )
        //{
        //    print("object added");
        //   // ReadTheBarcode(images[images.Count - 1]);
           
        //   foreach(Texture2D texture in images)
        //    {

        //        launchafish(texture, count);
        //        count++;
        //    }
        //}
    }
    private void RemoveDuplicate()
    {
        GetDistinctArrayList(images, 0);
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
    private void GetDistinctArrayList(List<Texture2D> arr, int idx)

    {

        int count = 0;

        if (idx >= arr.Count) return;

        Texture2D val = arr[idx];
        foreach (Texture2D s in arr)
        {
            if (s.Equals(arr[idx]))
            {
                count++;
            }
        }

        if (count > 1)
        {
            arr.Remove(val);
            GetDistinctArrayList(arr, idx);
        }
        else
        {
            idx += 1;
            GetDistinctArrayList(arr, idx);
        }
    }

    void launchafish(Texture2D texture, string filePath) {
        //if (SceneManager.GetActiveScene().buildIndex == 1)
        //{
        //    var fish = Instantiate(ball[ReadTheBarcode(images[images.Count - 1]) - 1], new Vector3(375, -35f, 163f), Quaternion.identity);
        //    setmaterial(images[images.Count - 1], fish);
        //}
        //if (SceneManager.GetActiveScene().buildIndex == 2)
        //{
        //    var fish = Instantiate(ball[ReadTheBarcode(images[images.Count - 1]) - 1], new Vector3(270.88f, 19.9f, 163f), Quaternion.identity);
        //    setmaterial(images[images.Count - 1], fish);
        //}
        try
        {
            //if (SceneManager.GetActiveScene().buildIndex == 1)
            //{
                var fish = Instantiate(ball[0], transform.position, Quaternion.identity);

            fish.name = Path.GetFileNameWithoutExtension(filePath) ;
            activedraw.Add(fish);
            activeobjname.Add(Path.GetFileNameWithoutExtension(filePath));
            //t2.text = fish.name + " " +  fish.transform.position;
            setmaterial(texture, fish);
            //}
            //Vector3(-31, 238.699997, 799)
            //if (SceneManager.GetActiveScene().buildIndex == 2)
            //{
            //    var fish = Instantiate(ball[0], transform.position, Quaternion.identity);
            //    fish.name = fish.name + count.ToString();
            //    setmaterial(texture, fish);
            //}
        }
        catch (Exception n)
        {

            t2.text = n.Message;
        }
        

        //fish.GetComponent<SplineFollow>().Spline = sp[0];


    }
    void setmaterial(Texture2D texture , GameObject fish)
    {
        //var material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        //material.SetFloat("_Mode", 2);
        //material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        //material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        //material.SetInt("_ZWrite", 0);
        //material.DisableKeyword("_ALPHATEST_ON");
        //material.EnableKeyword("_ALPHABLEND_ON");
        //material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        //material.renderQueue = 3000;

       
        //material.mainTexture = texture;
        //fish.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().material = material;
        
        Texture2D tt = duplicateTexture(texture);
        texture = duplicateTexture(texture);
   
        Color32[] pixels = texture.GetPixels32();

        for (int i = 0; i < pixels.Length; i++)
        {
            if (pixels[i].r == 255 && pixels[i].g == 255 && pixels[i].b == 255)
            {
                pixels[i].a = 0;
            }
            else
            {
                // Check if the pixel is close to white
                if (pixels[i].r >= 220 && pixels[i].g >= 220 && pixels[i].b >= 220)
                {
                    // Make the pixel half transparent
                    pixels[i].a = 0; // or you can set it to a fixed value like pixels[i].a = 128;
                }
            }
        }

        texture.SetPixels32(pixels);
        texture.Apply();
        foreach (var item in fish.GetComponentsInChildren<MeshRenderer>())
        {
            item.material.mainTexture = texture;
        }
        try
        {
            fish.transform.GetChild(0).GetComponent<MeshRenderer>().material.mainTexture = texture;
        }
        catch (Exception n)
        {

            t2.text = "n" + n.Message;

        }
      
    }
   
}
//public class FishManager : MonoBehaviour
//{
//    public GameObject fishPrefab;  // Drag your fish prefab to this field in the Unity Editor

//    private List<GameObject> fishes = new List<GameObject>();
//    private Dictionary<int, GameObject> handFishMapping = new Dictionary<int, GameObject>();

//    void Update()
//    {
//        // You can handle fish spawning logic here if needed
//    }

//    public GameObject SpawnFish(Vector3 position)
//    {
//        GameObject fish = Instantiate(fishPrefab, position, Quaternion.identity);
//        fishes.Add(fish);
//        return fish;
//    }

//    public void ProcessHandData(HandManager handData)
//    {
        
//        // Iterate through hands
//        for (int i = 0; i < handData.NumberOfHands; i++)
//        {
//            var hand = handData[i];
//            int handId = hand.Id;

//            // Get hand position
//            var handPosition = hand.PalmPosition.ToVector3();

//            // Find the closest fish to the hand
//            GameObject closestFish = FindClosestFish(handPosition);

//            // Update the position of the closest fish based on the hand position
//            closestFish.transform.position = handPosition;

//            // Associate the hand with the closest fish in the dictionary
//            handFishMapping[handId] = closestFish;
//        }
//    }

//    GameObject FindClosestFish(Vector3 handPosition)
//    {
//        GameObject closestFish = null;
//        float closestDistance = float.MaxValue;

//        // Iterate through all fishes to find the closest one
//        foreach (var fish in fishes)
//        {
//            float distance = Vector3.Distance(handPosition, fish.transform.position);

//            // Check if this fish is closer than the current closest fish
//            if (distance < closestDistance)
//            {
//                closestDistance = distance;
//                closestFish = fish;
//            }
//        }

//        return closestFish;
//    }

//    // Other fish-related functions can be added here
//}
