using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Parabox.CSG;

public class TestCSG : MonoBehaviour {




    void Start () {
        Vector3 RotationVec = new Vector3(0, 60, 0);
        GameObject composite = CreateHemisphere();
        GameObject composite2 = CreateHemisphere(Rotation: Quaternion.Euler(RotationVec));

        GameObject final = GetIntersection(composite, composite2);

        //composite.SetActive(false);
        //composite2.SetActive(false);


    }

    public GameObject CreateHemisphere(Vector3 Position = default(Vector3), Quaternion Rotation = default(Quaternion), Vector3? Scale = null)
    {
        GameObject Sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        GameObject Cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        GameObject Pivot = new GameObject();
        Sphere.transform.position = Position;
        Cube.transform.position = Position + new Vector3(0.5F,0,0);
        Pivot.transform.position = Position;
        Sphere.transform.parent = Pivot.transform;
        Cube.transform.parent = Pivot.transform;
        Pivot.transform.rotation = Rotation;

        Mesh m = CSG.Intersect(Sphere, Cube);
        GameObject composite = new GameObject("Hemisphere");
        composite.AddComponent<MeshFilter>().sharedMesh = m;
        composite.AddComponent<MeshRenderer>().sharedMaterial = Sphere.GetComponent<MeshRenderer>().material;

        if (Scale == null)
            composite.transform.localScale = new Vector3(1, 1, 1);
        else
            composite.transform.localScale = (Vector3)Scale;

        Destroy(Sphere);
        Destroy(Cube);
        Destroy(Pivot);

        return composite;
    }

    public GameObject GetIntersection(GameObject Hemi1, GameObject Hemi2)
    {
        if(Hemi1.transform.localScale == Hemi2.transform.localScale)
        {
            Hemi1.transform.localScale *= 1.3F;
        }
        Mesh n = CSG.Intersect(Hemi1, Hemi2);
        GameObject final = new GameObject("Final");
        final.AddComponent<MeshFilter>().sharedMesh = n;
        final.AddComponent<MeshRenderer>().sharedMaterial = Hemi1.GetComponent<MeshRenderer>().material;

        Destroy(Hemi1);
        Destroy(Hemi2);
        return final;
    }
}
