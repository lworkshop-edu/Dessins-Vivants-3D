//using System.Drawing;
//using UnityEngine;
//using UnityEngine.UIElements;

//public class TransparentMeshFromTexture : MonoBehaviour
//{
//    public Texture2D sourceTexture;
//    public GameObject parentObject;
//    public Material planeMaterial;
//    public float scale = 5;

//    void Start()
//    {
//        if (parentObject == null)
//        {
//            Debug.LogError("Parent Object is not assigned.");
//            return;
//        }

//        if (planeMaterial == null)
//        {
//            Debug.LogError("Plane Material is not assigned.");
//            return;
//        }

//        Collider parentCollider = parentObject.GetComponent<Collider>();
//        if (parentCollider == null)
//        {
//            Debug.LogError("Parent Object does not have a Collider component.");
//            return;
//        }

//        Rect nonTransparentRect = GetNonTransparentRect(sourceTexture);
//        Texture2D croppedTexture = CropTexture(sourceTexture, nonTransparentRect);
//        CreatePlaneWithTexture(croppedTexture, nonTransparentRect.size, parentCollider);
//    }

//    Rect GetNonTransparentRect(Texture2D texture)
//    {
//        int left = texture.width;
//        int right = 0;
//        int top = 0;
//        int bottom = texture.height;

//        for (int y = 0; y < texture.height; y++)
//        {
//            for (int x = 0; x < texture.width; x++)
//            {
//                if (texture.GetPixel(x, y).a > 0)
//                {
//                    if (x < left) left = x;
//                    if (x > right) right = x;
//                    if (y > top) top = y;
//                    if (y < bottom) bottom = y;
//                }
//            }
//        }

//        int width = right - left + 1;
//        int height = top - bottom + 1;

//        return new Rect(left, bottom, width, height);
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

//    void CreatePlaneWithTexture(Texture2D texture, Vector2 size, Collider parentCollider)
//    {
//        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
//    plane.transform.SetParent(parentObject.transform);

//        // Scale plane to fit inside parent collider
//        Bounds bounds = parentCollider.bounds;
//    float colliderWidth = bounds.size.x;
//    float colliderHeight = bounds.size.z;

//        // Determine scale based on width
//    float scaleX = colliderWidth / scale;
//    float scaleY = scaleX * (size.y / size.x);

//    plane.transform.localScale = new Vector3(scaleX, 1, scaleY);

//    // Apply the given material to the plane
//    planeMaterial.mainTexture = texture;
//         plane.GetComponent<Renderer>().material = planeMaterial;

//         // Align plane's rotation and position with the parent object
//         plane.transform.localRotation = Quaternion.Euler(180, 0, -90);
//         plane.transform.localPosition = Vector3.zero;
//         plane.transform.position = parentCollider.bounds.center;
//     }




//}
using UnityEngine;

public class TransparentMeshFromTexture : MonoBehaviour
{
    public Texture2D sourceTexture;
    public GameObject parentObject;
    public Material planeMaterial;
    public float planeWidth = 40.0f;
    public float planeHeight = 35.0f;
    void Start()
    {
        if (parentObject == null)
        {
            Debug.LogError("Parent Object is not assigned.");
            return;
        }

        if (planeMaterial == null)
        {
            Debug.LogError("Plane Material is not assigned.");
            return;
        }

        BoxCollider parentCollider = parentObject.GetComponent<BoxCollider>();
        if (parentCollider == null)
        {
            Debug.LogError("Parent Object does not have a BoxCollider component.");
            return;
        }

        Rect nonTransparentRect = GetNonTransparentRect(sourceTexture);
        Texture2D croppedTexture = CropTexture(sourceTexture, nonTransparentRect);
        CreatePlaneWithTexture(croppedTexture, nonTransparentRect.size, parentCollider);
    }

    Rect GetNonTransparentRect(Texture2D texture)
    {
        int left = texture.width;
        int right = 0;
        int top = 0;
        int bottom = texture.height;

        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                if (texture.GetPixel(x, y).a > 0)
                {
                    if (x < left) left = x;
                    if (x > right) right = x;
                    if (y > top) top = y;
                    if (y < bottom) bottom = y;
                }
            }
        }

        int width = right - left + 1;
        int height = top - bottom + 1;

        return new Rect(left, bottom, width, height);
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

    void CreatePlaneWithTexture(Texture2D texture, Vector2 size, BoxCollider parentCollider)
    {
        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.transform.SetParent(parentObject.transform);

        // Calculate scale factors
        Bounds bounds = parentCollider.bounds;
        float colliderWidth = bounds.size.x;
        float colliderHeight = bounds.size.z;



        // Calculate scale factors to fit the plane within the parent collider
        float scaleX = colliderWidth / planeWidth;
        float scaleY = colliderHeight / planeHeight;

        // Adjust the scale based on the texture size
        float textureAspectRatio = size.x / size.y;
        float colliderAspectRatio = colliderWidth / colliderHeight;

        if (textureAspectRatio > colliderAspectRatio)
        {
            scaleY = scaleX / textureAspectRatio;
        }
        else
        {
            scaleX = scaleY * textureAspectRatio;
        }

        // Apply scaling to the plane
        plane.transform.localScale = new Vector3(scaleX, 1, scaleY);

        // Apply the given material to the plane
        planeMaterial.mainTexture = texture;
        plane.GetComponent<Renderer>().material = planeMaterial;

        // Position the plane to align with the center of the collider
        plane.transform.position = parentCollider.bounds.center;

        // Align the plane's rotation to ensure correct orientation
        plane.transform.localRotation = Quaternion.Euler(180, 0, -90);

        // Offset to align with the bottom of the collider
        plane.transform.localPosition = new Vector3(0, 0, -0.19f);
    }
}

