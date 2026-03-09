using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

public class CreateCubesFromTexture : MonoBehaviour
{
 //   public Renderer cube;
 //   public Texture2D texture;   // Texture to read colors from
 //   public float alphaThreshold = 0.5f; // Threshold for alpha channel
 //   public float depth = 4f;   // Depth scale for cubes

 //   private GameObject cubesContainer; // Container for instantiated cubes

 //   private Dictionary<Color32, Material> palette = new Dictionary<Color32, Material>();

 //   void Start()
 //   {
 //       if (texture == null)
 //       {
 //           Debug.LogError("Texture not assigned!");
 //           return;
 //       }

 //       cubesContainer = new GameObject("GeneratedCubes");
 //       cubesContainer.transform.SetParent(transform);

 //       // Iterate through texture pixels to create cubes
 //       int textureWidth = texture.width;
 //       int textureHeight = texture.height;
 //       Color32[] pixels = texture.GetPixels32();

 //       int x = 0;
 //       int y = 0;

 //       for (int i = 0; i < pixels.Length; i++)
 //       {
 //           Color32 color = pixels[i];

 //           if (x >= textureWidth)
 //           {
 //               y++;
 //               x = 0;
 //           }

 //           if (color.a > alphaThreshold * 255) // Convert threshold to 0-255 range
 //           {
 //               Vector3 position = transform.position + transform.right * x + transform.up * y;

 //               // Instantiate cube prefab and set parent
 //               Renderer cubeInstance = Instantiate(cube, position, Quaternion.identity, cubesContainer.transform) as Renderer;

 //               // Adjust scale in the depth dimension
 //               Vector3 scale = cubeInstance.transform.localScale;
 //               scale.z = depth;
 //               cubeInstance.transform.localScale = scale;

 //               // Manage material instantiation and reuse
 //               if (!palette.ContainsKey(color))
 //               {
 //                   Material newMaterial = new Material(cubeInstance.sharedMaterial);
 //                   newMaterial.color = color;
 //                   cubeInstance.material = newMaterial;
 //                   palette.Add(color, newMaterial);
 //               }
 //               else
 //               {
 //                   cubeInstance.material = palette[color];
 //               }
 //           }

 //           x++;
 //       }

 //       // Combine all child meshes into a single mesh
 //       GameObject combinedMeshObject = CombineMeshes(cubesContainer);
 //   }

 //   GameObject CombineMeshes(GameObject parentObject)
 //   {
 //       MeshFilter[] meshFilters = parentObject.GetComponentsInChildren<MeshFilter>();
 //       CombineInstance[] combine = new CombineInstance[meshFilters.Length];

 //       for (int i = 0; i < meshFilters.Length; i++)
 //       {
 //           combine[i].mesh = meshFilters[i].sharedMesh;
 //           combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
 //           meshFilters[i].gameObject.SetActive(false); // Disable original GameObjects
 //       }

 //       // Create a new GameObject to hold the combined mesh
 //       GameObject combinedMeshObject = new GameObject("CombinedMesh");
 //       combinedMeshObject.transform.SetParent(parentObject.transform);

 //       // Add MeshFilter and MeshRenderer to the combined mesh GameObject
 //       MeshFilter meshFilter = combinedMeshObject.AddComponent<MeshFilter>();
 //       meshFilter.mesh = new Mesh();

 //       // Combine meshes
 //       meshFilter.mesh.CombineMeshes(combine, true);



 //       // Add MeshRenderer to display the combined mesh
 //       MeshRenderer meshRenderer = combinedMeshObject.AddComponent<MeshRenderer>();
 //       meshRenderer.material = meshFilters[0].GetComponent<Renderer>().sharedMaterial; // Use material from the first cube

 //       // Optionally, add Collider component if needed
 //       combinedMeshObject.AddComponent<MeshCollider>();

	//	        // Create a Empty Mesh
 //       var mesh = new Mesh();

 //       // Call targetMesh.CombineMeshes and pass in the array of CombineInstances.
 //       mesh.CombineMeshes(combine);

 //       // // Assign the target mesh to the mesh filter of the combination game object.
 //       // TargetMesh.mesh = mesh;

 //       // Save the Mesh automatically
 //       SaveMesh(mesh, "CombinedMeshPrefab.prefab", false, true);
        
 //       // Print Results
 //       Debug.Log("<color=#20E7B0>Combine Meshes was Successful!</color>");

 //       return combinedMeshObject; 
 //   }
	//public static void SaveMesh(Mesh mesh, string name, bool makeNewInstance, bool optimizeMesh)
 //   {
 //       // Define the path where you want to save the mesh
 //       string savePath = "Assets/" + name + ".asset";

 //       // Create a new instance of the mesh if needed
 //       Mesh meshToSave = (makeNewInstance) ? Object.Instantiate(mesh) as Mesh : mesh;

 //       // Optimize the mesh if required
 //       if (optimizeMesh)
 //           MeshUtility.Optimize(meshToSave);

 //       // Save the mesh asset to the specified path
 //       AssetDatabase.CreateAsset(meshToSave, savePath);
 //       AssetDatabase.SaveAssets();
 //   }
}