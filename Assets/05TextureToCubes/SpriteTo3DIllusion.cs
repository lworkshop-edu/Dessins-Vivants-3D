using UnityEngine;

public class SpriteTo3DIllusion : MonoBehaviour
{
    public Sprite sprite;         // The sprite to create the illusion
    public GameObject parentObject; // The parent object to attach the sprite renderers to
    public int depth = 10;        // The depth (number of sprite layers)
    public float spacing = 0.1f;  // The spacing between each sprite layer

    void Start()
    {
        Create3DIllusion();
    }

    void Create3DIllusion()
    {
        if (sprite == null || parentObject == null)
        {
            Debug.LogError("Sprite or Parent Object is not assigned.");
            return;
        }

        for (int i = 0; i < depth; i++)
        {
            GameObject spriteObject = new GameObject("SpriteLayer_" + i);
            spriteObject.transform.SetParent(parentObject.transform);

            float zPosition = i * spacing;
            spriteObject.transform.localPosition = new Vector3(0, 0, -zPosition);

            SpriteRenderer spriteRenderer = spriteObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = sprite;
        }
    }
}


// using UnityEngine;

// public class SpriteTo3DIllusionAndCombine : MonoBehaviour
// {
//     public Sprite sprite;         // The sprite to create the illusion
//     public GameObject parentObject; // The parent object to attach the sprite layers to
//     public int depth = 10;        // The depth (number of sprite layers)
//     public float spacing = 0.1f;  // The spacing between each sprite layer

//     void Start()
//     {
//         Create3DIllusionAndCombine();
//     }

//     void Create3DIllusionAndCombine()
//     {
//         if (sprite == null || parentObject == null)
//         {
//             Debug.LogError("Sprite or Parent Object is not assigned.");
//             return;
//         }

//         // Array to hold CombineInstances for mesh combining
//         CombineInstance[] combine = new CombineInstance[depth];

//         for (int i = 0; i < depth; i++)
//         {
//             GameObject spriteObject = new GameObject("SpriteLayer_" + i);
//             spriteObject.transform.SetParent(parentObject.transform);

//             float zPosition = i * spacing;
//             spriteObject.transform.localPosition = new Vector3(0, 0, -zPosition);

//             // Create a MeshFilter and MeshRenderer for each sprite layer
//             MeshFilter meshFilter = spriteObject.AddComponent<MeshFilter>();
//             MeshRenderer meshRenderer = spriteObject.AddComponent<MeshRenderer>();

//             // Create a new mesh for this layer
//             Mesh mesh = new Mesh();
//             Vector3[] vertices = new Vector3[4];
//             Vector2[] uv = new Vector2[4];
//             int[] triangles = new int[6];

//             // Define vertices and triangles for a simple quad
//             vertices[0] = new Vector3(-0.5f, -0.5f, 0);
//             vertices[1] = new Vector3(0.5f, -0.5f, 0);
//             vertices[2] = new Vector3(-0.5f, 0.5f, 0);
//             vertices[3] = new Vector3(0.5f, 0.5f, 0);

//             triangles[0] = 0;
//             triangles[1] = 2;
//             triangles[2] = 1;
//             triangles[3] = 2;
//             triangles[4] = 3;
//             triangles[5] = 1;

//             // Assign vertices and triangles to the mesh
//             mesh.vertices = vertices;
//             mesh.uv = uv;
//             mesh.triangles = triangles;

//             // Apply sprite texture to the material
//             Material material = new Material(Shader.Find("Unlit/Texture"));
//             material.mainTexture = sprite.texture;

//             // Set material to the MeshRenderer
//             meshRenderer.material = material;

//             // Set the mesh to the MeshFilter
//             meshFilter.mesh = mesh;

//             // Setup CombineInstance for mesh combining
//             combine[i].mesh = meshFilter.sharedMesh;
//             combine[i].transform = meshFilter.transform.localToWorldMatrix;

//             // Optionally, add a MeshCollider if needed
//             spriteObject.AddComponent<MeshCollider>();
//         }

//         // Combine meshes into a single GameObject
//         GameObject combinedMeshObject = new GameObject("CombinedMesh");
//         combinedMeshObject.transform.SetParent(transform);

//         MeshFilter combinedMeshFilter = combinedMeshObject.AddComponent<MeshFilter>();
//         combinedMeshFilter.mesh = new Mesh();
//         combinedMeshFilter.mesh.CombineMeshes(combine, true);

//         MeshRenderer combinedMeshRenderer = combinedMeshObject.AddComponent<MeshRenderer>();
//         combinedMeshRenderer.sharedMaterial = combine[0].meshRenderer.sharedMaterial; // Use material from the first sprite layer

//         // Optionally, add a MeshCollider to the combined mesh object
//         combinedMeshObject.AddComponent<MeshCollider>();

//         Debug.Log("<color=#20E7B0>Sprite 3D illusion and mesh combining was successful!</color>");
//     }


// }
