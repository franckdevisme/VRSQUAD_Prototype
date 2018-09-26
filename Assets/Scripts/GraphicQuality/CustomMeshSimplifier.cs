using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityMeshSimplifier;
//using UnityEditor;

[ExecuteInEditMode]
public class CustomMeshSimplifier : MonoBehaviour
{
    public float meshQualityDivider = 0.5f;
    [HideInInspector]
    public int vertexCount;
    [HideInInspector]
    public int triangleCount;
    [HideInInspector]
    public int submeshCount;

    private MeshFilter myMeshFilter;
    public Mesh baseMesh;

    // Use this for initialization
    void OnEnable()

    {
       
        myMeshFilter = GetComponent<MeshFilter>();
        if(myMeshFilter != null)
        {
            baseMesh = myMeshFilter.sharedMesh;
            MeshInfo();
        }
    }

    public void SimplifyMesh()
    {

        myMeshFilter = GetComponent<MeshFilter>();

        var meshSimplifier = new UnityMeshSimplifier.MeshSimplifier();
        meshSimplifier.Initialize(myMeshFilter.sharedMesh);
        meshSimplifier.SimplifyMesh(meshQualityDivider);
        var destMesh = meshSimplifier.ToMesh();

        myMeshFilter.sharedMesh = destMesh;
        myMeshFilter.sharedMesh.RecalculateBounds();

        /* 
         * 
         * 
         * 
         * 
        destMesh.name = baseMesh + "_optimized";

        string myPath = AssetDatabase.GetAssetPath(PrefabUtility.FindPrefabRoot(gameObject));
        Debug.Log(myPath);

        AssetDatabase.CreateAsset(destMesh, myPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        

        var emptyPrefab = PrefabUtility.CreateEmptyPrefab(myPath + PrefabUtility.FindPrefabRoot(gameObject).name);

        PrefabUtility.ReplacePrefab(PrefabUtility.FindPrefabRoot(gameObject), emptyPrefab);
        */

        MeshInfo();
        Debug.Log("Mesh Simplified");
    }




    public void RevertMesh()
    {

        GetComponent<MeshFilter>().mesh = baseMesh;
        MeshInfo();
        Debug.Log("Mesh Reverted");
    }

    public void MeshInfo()
    {
        if(GetComponent<MeshFilter>().sharedMesh != null)
        {
            vertexCount = GetComponent<MeshFilter>().sharedMesh.vertexCount;
            triangleCount = GetComponent<MeshFilter>().sharedMesh.triangles.Length / 3;
            submeshCount = GetComponent<MeshFilter>().sharedMesh.subMeshCount;

        }
      
    }

    public void SimplifyAllMeshes()
    {      
        MeshFilter[] meshArray = GetComponentsInChildren<MeshFilter>();
        foreach(MeshFilter childMesh in meshArray)
        {
            var meshSimplifier = new UnityMeshSimplifier.MeshSimplifier();
            meshSimplifier.Initialize(childMesh.sharedMesh);
            meshSimplifier.SimplifyMesh(meshQualityDivider);
            var destMesh = meshSimplifier.ToMesh();

            childMesh.mesh = destMesh;
        }
        Debug.Log("All Children Meshes Simplified");
     
    }
}