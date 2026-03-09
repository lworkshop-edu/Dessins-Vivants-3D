using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZXing;
using ZXing.QrCode;
using UnityEngine.UI;
using ZXing.Common;

public class removebackground : MonoBehaviour
{
    public Texture2D texture;
    public Material mat;
    //only for QrCode generation
    public RawImage qrCodeImage;
    public string dataToEncode;
    private Texture2D qrCodeTexture;
    void Start()
    {
        //only for QrCode generation
        //GenerateQrCode();

        //BarcodeReader reader = new BarcodeReader();
        //Color32[] pixells = texture.GetPixels32();
        //byte[] data = new byte[pixells.Length];

        //for (int i = 0; i < pixells.Length; i++)
        //{
        //    Color32 pixel = pixells[i];
        //    // Pack RGBA values into a single byte value
        //    byte value = (byte)((pixel.r + pixel.g + pixel.b) / 3);
        //    data[i] = value;
        //}
        //Result result = reader.Decode(texture.GetRawTextureData() , texture.width, texture.height,RGBLuminanceSource.BitmapFormat.RGB32);

        //// If the QR code was detected, print the result
        //if (result != null)
        //{
        //    Debug.Log("QR code detected: " + result.Text);
        //}
        //Debug.Log("QR code detected: " + result.Text.ToString());
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
        mat.mainTexture = texture;
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
    //    Texture2D duplicateTexture(Texture2D source)
    //    {
    //        RenderTexture renderTex = RenderTexture.GetTemporary(
    //                    source.width,
    //                    source.height,
    //                    0,
    //                    RenderTextureFormat.Default,
    //                    RenderTextureReadWrite.Linear);

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

    //only for QrCode generation
    private void GenerateQrCode()
    {
        BarcodeWriter barcodeWriter = new BarcodeWriter();
        barcodeWriter.Format = BarcodeFormat.QR_CODE;

        EncodingOptions encodingOptions = new EncodingOptions();
        encodingOptions.Height = 256;
        encodingOptions.Width = 256;
        encodingOptions.Margin = 1;

        barcodeWriter.Options = encodingOptions;

        Color32[] color32 = barcodeWriter.Write(dataToEncode);
        qrCodeTexture = new Texture2D(256, 256);
        qrCodeTexture.SetPixels32(color32);
        qrCodeTexture.Apply();

        qrCodeImage.texture = qrCodeTexture;
    }
}
