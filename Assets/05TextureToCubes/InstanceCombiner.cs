using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

public class InstanceCombiner : MonoBehaviour
{
    //// Source Meshes you want to combine
    //[SerializeField] private List<MeshFilter> listMeshFilter;

    //// Make a new mesh to be the target of the combine operation
    //[SerializeField] private MeshFilter TargetMesh;

    //[ContextMenu("Combine Meshes")]
    //private void CombineMesh()
    //{
    //    // Make an array of CombineInstance.
    //    var combine = new CombineInstance[listMeshFilter.Count];

    //    // Set Mesh and their Transform to the CombineInstance
    //    for (int i = 0; i < listMeshFilter.Count; i++)
    //    {
    //        combine[i].mesh = listMeshFilter[i].sharedMesh;
    //        combine[i].transform = listMeshFilter[i].transform.localToWorldMatrix;
    //    }

    //    // Create a Empty Mesh
    //    var mesh = new Mesh();

    //    // Call targetMesh.CombineMeshes and pass in the array of CombineInstances.
    //    mesh.CombineMeshes(combine);

    //    // Assign the target mesh to the mesh filter of the combination game object.
    //    TargetMesh.mesh = mesh;

    //    // Save the Mesh automatically
    //    SaveMesh(mesh, gameObject.name, false, true);
        
    //    // Print Results
    //    Debug.Log("<color=#20E7B0>Combine Meshes was Successful!</color>");
    //}

    //public static void SaveMesh(Mesh mesh, string name, bool makeNewInstance, bool optimizeMesh)
    //{
    //    // Define the path where you want to save the mesh
    //    string savePath = "Assets/" + name + ".asset";

    //    // Create a new instance of the mesh if needed
    //    Mesh meshToSave = (makeNewInstance) ? Object.Instantiate(mesh) as Mesh : mesh;

    //    // Optimize the mesh if required
    //    if (optimizeMesh)
    //        MeshUtility.Optimize(meshToSave);

    //    // Save the mesh asset to the specified path
    //    AssetDatabase.CreateAsset(meshToSave, savePath);
    //    AssetDatabase.SaveAssets();
    //}

    //void Start()
    //{
    //    Invoke("PerformActionWithLerp", 2f); 
    //}

    //void PerformActionWithLerp()
    //{
    //    CombineMesh();
    //}
}
