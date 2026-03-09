using UnityEngine;
using UnityEngine.Rendering;
using Unity.Collections;
using UnityEditor;


public class SaveTextureToFileUtility
{
   public enum SaveTextureFileFormat
   {
       EXR, JPG, PNG, TGA
   };
 
   /// <summary>
   /// Saves a Texture2D to disk with the specified filename and image format
   /// </summary>
   /// <param name="tex"></param>
   /// <param name="filePath"></param>
   /// <param name="fileFormat"></param>
   /// <param name="jpgQuality"></param>
   static public void SaveTexture2DToFile(Texture2D tex, string filePath, SaveTextureFileFormat fileFormat, int jpgQuality = 95)
   {
       switch (fileFormat)
       {
           case SaveTextureFileFormat.EXR:
               System.IO.File.WriteAllBytes(filePath + ".exr", tex.EncodeToEXR());
               break;
           case SaveTextureFileFormat.JPG:
               System.IO.File.WriteAllBytes(filePath + ".jpg", tex.EncodeToJPG(jpgQuality));
               break;
           case SaveTextureFileFormat.PNG:
               System.IO.File.WriteAllBytes(filePath + ".png", tex.EncodeToPNG());
               break;
           case SaveTextureFileFormat.TGA:
               System.IO.File.WriteAllBytes(filePath + ".tga", tex.EncodeToTGA());
               break;
       }
   }
 
 
   /// <summary>
   /// Saves a RenderTexture to disk with the specified filename and image format
   /// </summary>
   /// <param name="renderTexture"></param>
   /// <param name="filePath"></param>
   /// <param name="fileFormat"></param>
   /// <param name="jpgQuality"></param>
   static public void SaveRenderTextureToFile(RenderTexture renderTexture, string filePath, SaveTextureFileFormat fileFormat = SaveTextureFileFormat.PNG, int jpgQuality = 95)
   {
       Texture2D tex;
       if (fileFormat != SaveTextureFileFormat.EXR)
           tex = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false, false);
       else
           tex = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBAFloat, false, true);
       var oldRt = RenderTexture.active;
       RenderTexture.active = renderTexture;
       tex.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
       tex.Apply();
       RenderTexture.active = oldRt;
       SaveTexture2DToFile(tex, filePath, fileFormat, jpgQuality);
       if (Application.isPlaying)
           Object.Destroy(tex);
       else
           Object.DestroyImmediate(tex);
 
   }

   static public void SaveTextureToFile(Texture source,
                                         string filePath,
                                         int width,
                                         int height,
                                         SaveTextureFileFormat fileFormat = SaveTextureFileFormat.PNG,
                                         int jpgQuality = 95,
                                         bool asynchronous = true,
                                         System.Action<bool> done = null)
    {


      
        if (!(source is Texture2D || source is RenderTexture))
        {
            done?.Invoke(false);
            return;
        }

        // Use the original texture size if the input is negative
        if (width < 0 || height < 0)
        {
            width = source.width;
            height = source.height;
        }

        // Create a Texture2D from the source texture
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        RenderTexture rt = RenderTexture.GetTemporary(width, height, 0);
        Graphics.Blit(source, rt);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = rt;
        texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        texture.Apply();
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(rt);
        // Get the pixel colors from the texture
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
        
        // resize the original image:
        var resizeRT = RenderTexture.GetTemporary(width, height, 0);
        Graphics.Blit(texture, resizeRT);
 
        // create a native array to receive data from the GPU:
        var narray = new NativeArray<byte>(width * height * 4, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
 
        // request the texture data back from the GPU:
        var request = AsyncGPUReadback.RequestIntoNativeArray (ref narray, resizeRT, 0, (AsyncGPUReadbackRequest request) =>
        {
            // if the readback was successful, encode and write the results to disk
            if (!request.hasError)
            {
                NativeArray<byte> encoded;
 
                switch (fileFormat)
                {
                    case SaveTextureFileFormat.EXR:
                        encoded = ImageConversion.EncodeNativeArrayToEXR(narray, resizeRT.graphicsFormat, (uint)width, (uint)height);
                        break;
                    case SaveTextureFileFormat.JPG:
                        encoded = ImageConversion.EncodeNativeArrayToJPG(narray, resizeRT.graphicsFormat, (uint)width, (uint)height, 0, jpgQuality);
                        break;
                    case SaveTextureFileFormat.TGA:
                        encoded = ImageConversion.EncodeNativeArrayToTGA(narray, resizeRT.graphicsFormat, (uint)width, (uint)height);
                        break;
                    default:
                        encoded = ImageConversion.EncodeNativeArrayToPNG(narray, resizeRT.graphicsFormat, (uint)width, (uint)height);
                        break;
                }
 
                System.IO.File.WriteAllBytes(filePath, encoded.ToArray());
                encoded.Dispose();

           

            }



    narray.Dispose();
 
            // notify the user that the operation is done, and its outcome.
            done?.Invoke(!request.hasError);
        });
 
        if (!asynchronous)
            request.WaitForCompletion();
        
    }
    public static Sprite TextureToSprite(Texture2D texture) 
    {
     
       return Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 50f, 0, SpriteMeshType.FullRect);
    }
}

 