using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Parabox.CSG;
using UnityEditor;

public class TestCSG : MonoBehaviour {

// Initialize two new meshes in the scene
public GameObject Sphere1;
public GameObject Sphere2;
public GameObject Sphere3;



    void Start () {

        // Perform boolean operation
        Mesh m = CSG.Intersect(Sphere1, Sphere2);

        // Create a gameObject to render the result
        GameObject composite = new GameObject("Hemisphere");
        composite.AddComponent<MeshFilter>().sharedMesh = m;
        composite.AddComponent<MeshRenderer>().sharedMaterial = Sphere1.GetComponent<MeshRenderer>().material;
        composite.transform.position = Sphere1.transform.position;
        Sphere1.SetActive(false);
        Sphere2.SetActive(false);

        Mesh n = CSG.Intersect(composite, Sphere3);
        GameObject final = new GameObject("Final");
        final.AddComponent<MeshFilter>().sharedMesh = n;
        final.AddComponent<MeshRenderer>().sharedMaterial = composite.GetComponent<MeshRenderer>().material;
        final.transform.position = composite.transform.position;
        composite.SetActive(false);
        //GameObject prefab = PrefabUtility.CreatePrefab("Assets/Meshes/Hemisphere.prefab", composite, ReplacePrefabOptions.ReplaceNameBased);
        //SaveMesh(composite.GetComponent<MeshFilter>().mesh, "HemisphereFinal");
        //Object prefab = PrefabUtility.CreateEmptyPrefab("Assets/Meshes/hemisphere.prefab");
        //PrefabUtility.ReplacePrefab(composite, prefab, ReplacePrefabOptions.ConnectToPrefab);

    }

    public static void SaveMesh(Mesh mesh, string name)
    {
        string path = "Assets/Meshes/" + name + ".asset";
        print(path);

        AssetDatabase.CreateAsset(mesh, path);
        AssetDatabase.SaveAssets();
    }
}
