using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Parabox.CSG;

public class TestCSG : MonoBehaviour {


    

    void Start () {
        Vector3 RotationVec = new Vector3(0, 60, 0);
        Vector3 scale = new Vector3(1, 1, 1);
        Vector3 position = new Vector3(0, 0, 0);
        GameObject composite = CreateHemisphere();
        //composite.SetActive(false);
        GameObject composite2 = CreateHemisphere(Rotation: Quaternion.Euler(RotationVec));
        //composite.SetActive(true);
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
        m.name = "Hemisphere Mesh";
        GameObject composite = new GameObject("Hemisphere");
        composite.AddComponent<MeshFilter>().sharedMesh = m;
        composite.AddComponent<MeshRenderer>().sharedMaterial = Sphere.GetComponent<MeshRenderer>().material;
        if (Scale == null)
            composite.transform.localScale = new Vector3(1, 1, 1);
        else
            composite.transform.localScale = (Vector3)Scale;
        Destroy(Cube);
        composite.AddComponent<MeshCollider>();
        composite.GetComponent<MeshCollider>().sharedMesh = m;
        //createChildSpheres(Sphere, composite);
        Destroy(Sphere);
        
        Destroy(Pivot);

        return composite;
    }

    public GameObject GetIntersection(GameObject Hemi1, GameObject Hemi2, Vector3 Position = default(Vector3), Vector3? Scale = null)
    {
        GameObject Sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Sphere.transform.position = Position;
        if(Scale == null)
            Sphere.transform.localScale = new Vector3(1, 1, 1);
        else
            Sphere.transform.localScale = (Vector3)Scale;
        if (Hemi1.transform.localScale == Hemi2.transform.localScale)
        {
            Hemi1.transform.localScale *= 1.3F;
        }
        Mesh n = CSG.Intersect(Hemi1, Hemi2);
        GameObject final = new GameObject("Final");
        final.AddComponent<MeshFilter>().sharedMesh = n;
        final.AddComponent<MeshRenderer>().sharedMaterial = Hemi1.GetComponent<MeshRenderer>().material;
        Destroy(Hemi1);
        Destroy(Hemi2);
        createChildSpheres(Sphere, final);
        Destroy(Sphere);

        return final;
    }

    public void createChildSpheres(GameObject origin, GameObject hemisphere)
    {
        float scaling = origin.transform.localScale.x / 2;
        int numSpheres = 128;
        Vector3[] pts = PointsOnSphere(numSpheres);
        List<GameObject> uspheres = new List<GameObject>();
        int i = 0;

        foreach (Vector3 value in pts)
        {
            uspheres.Add(GameObject.CreatePrimitive(PrimitiveType.Sphere));
            uspheres[i].transform.position = origin.transform.TransformPoint(value * scaling);
            float scale = scaling / numSpheres * 2;
            uspheres[i].transform.localScale = new Vector3(scale, scale, scale);
            //Vector3 closestPoint = hemisphere.GetComponent<MeshCollider>().ClosestPoint(uspheres[i].transform.position);
            //var distToParent = Vector3.Distance(closestPoint, uspheres[i].transform.position);
            //if (distToParent > 0)
            //{
            //    uspheres[i].SetActive(false);
            //}
            Collider[] hitColliders = Physics.OverlapSphere(uspheres[i].transform.position, scale / 2);
            if (hitColliders.Length <= 6)
            {
                GameObject obj = uspheres[i];
                uspheres.Remove(obj);
                Destroy(obj);
            }
            else
            {
                //uspheres[i].transform.parent = hemisphere.transform;
                i++;
            }

        }
    }

    Vector3[] PointsOnSphere(int n)
    {
        List<Vector3> upts = new List<Vector3>();
        float inc = Mathf.PI * (3 - Mathf.Sqrt(5));
        float off = 2.0f / n;
        float x = 0;
        float y = 0;
        float z = 0;
        float r = 0;
        float phi = 0;

        for (var k = 0; k < n; k++)
        {
            y = k * off - 1 + (off / 2);
            r = Mathf.Sqrt(1 - y * y);
            phi = k * inc;
            x = Mathf.Cos(phi) * r;
            z = Mathf.Sin(phi) * r;

            upts.Add(new Vector3(x, y, z));
        }
        Vector3[] pts = upts.ToArray();
        return pts;
    }
}
