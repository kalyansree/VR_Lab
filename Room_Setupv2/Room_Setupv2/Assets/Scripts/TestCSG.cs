using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Parabox.CSG;

public class TestCSG : MonoBehaviour {

// Initialize two new meshes in the scene
public GameObject Sphere1;
public GameObject Sphere2;



    void Start () {

        // Perform boolean operation
        Mesh m = CSG.Intersect(Sphere1, Sphere2);

        // Create a gameObject to render the result
        GameObject composite = new GameObject("Hemisphere");
        composite.AddComponent<MeshFilter>().sharedMesh = m;
        composite.AddComponent<MeshRenderer>().sharedMaterial = Sphere1.GetComponent<MeshRenderer>().material;
        composite.transform.position = Sphere1.transform.position;

        GameObject composite2 = new GameObject("Hemisphere");
        composite2.AddComponent<MeshFilter>().sharedMesh = m;
        composite2.AddComponent<MeshRenderer>().sharedMaterial = Sphere1.GetComponent<MeshRenderer>().material;
        composite2.transform.position = Sphere1.transform.position;
        var rotationVector = composite2.transform.rotation.eulerAngles;
        rotationVector.y += 60;
        composite2.transform.rotation = Quaternion.Euler(rotationVector);
        composite2.transform.localScale = composite2.transform.localScale * 1.3F;
        Sphere1.SetActive(false);
        Sphere2.SetActive(false);

        Mesh n = CSG.Intersect(composite, composite2);
        GameObject final = new GameObject("Final");
        final.AddComponent<MeshFilter>().sharedMesh = n;
        final.AddComponent<MeshRenderer>().sharedMaterial = composite.GetComponent<MeshRenderer>().material;
        final.transform.position = composite.transform.position;
        composite.SetActive(false);
        composite2.SetActive(false);

    }
}
