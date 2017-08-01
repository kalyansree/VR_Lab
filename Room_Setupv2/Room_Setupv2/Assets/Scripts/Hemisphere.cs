using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Parabox.CSG;
using Vectrosity;


public static class Hemisphere
{
    private static Mesh hemisphereMesh;

    private static GameObject hemisphere1;
    private static GameObject hemisphere2;
    private static GameObject final;


    public static Material hemisphereMaterial1;
    public static Material hemisphereMaterial2;
    public static Material truncatedHemisphereMaterial;
    
    public static Camera myCamera;
    public static GameObject target;

    private static bool updatePos;
    void Start()
    {
        VectorLine.SetCamera3D(myCamera);
        hemisphereMesh = CreateHemisphereMesh();

        //hemisphere1.SetActive(false);
        //hemisphere2 = CreateHemisphere(Rotation: Quaternion.LookRotation(RotationVec,  RotationVec));
        //composite.SetActive(true);
        //hemisphere2.SetActive(false);




    }
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            hemisphere1 = CreateHemisphere(hemisphereMaterial1, Position: new Vector3(1, 1, 1));
            //hemisphere1.SetActive(false);
            Vector3 RotationVec = new Vector3(0, 1, 0);
            hemisphere2 = CreateHemisphere(hemisphereMaterial2, Position: new Vector3(1, 1, 1));
            //hemisphere2.SetActive(false)
            updatePos = true;
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            final = GetIntersection(hemisphere1, hemisphere2, truncatedHemisphereMaterial);
            //updatePos = false;
        }

        if (updatePos)
        {
            hemisphere1.transform.rotation = Quaternion.LookRotation(target.transform.position - hemisphere1.transform.position);
            Vector3 euler = hemisphere1.transform.rotation.eulerAngles;
            hemisphere1.transform.eulerAngles = new Vector3(euler.z, euler.y + 90F, euler.x);
        }



    }
    public Mesh CreateHemisphereMesh()
    {
        GameObject Sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        GameObject Cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Cube.transform.localScale = new Vector3(2, 2, 2);
        Sphere.transform.position = new Vector3(0, 0, 0);
        Cube.transform.position = new Vector3(1, 0, 0);

        Destroy(Sphere);
        Destroy(Cube);
        Mesh m = CSG.Intersect(Sphere, Cube);
        m.name = "Hemisphere Mesh";
        return m;
    }
    public GameObject CreateHemisphere(Material material, Vector3 Position = default(Vector3), Quaternion Rotation = default(Quaternion), Vector3? Scale = null)
    {
        GameObject composite = new GameObject("Hemisphere");
        composite.AddComponent<MeshFilter>().sharedMesh = hemisphereMesh;
        composite.AddComponent<MeshRenderer>().sharedMaterial = material;
        if (Scale == null)
            composite.transform.localScale = new Vector3(1, 1, 1);
        else
            composite.transform.localScale = (Vector3)Scale;
        composite.AddComponent<MeshCollider>();
        composite.GetComponent<MeshCollider>().sharedMesh = hemisphereMesh;
        composite.layer = LayerMask.NameToLayer("hemisphere");
        composite.transform.rotation = Rotation;
        composite.transform.position = Position;
        GameObject VectorLineObj = DrawHemisphereLines(composite, "hemisphere");
        VectorLineObj.transform.parent = composite.transform;
        VectorLineObj.name = "HemiLine";
        return composite;
    }

    public GameObject GetIntersection(GameObject Hemi1, GameObject Hemi2, Material material)
    {
        Vector3 position;
        if (Hemi1.transform.position != Hemi2.transform.position)
        {
            throw new System.Exception("Hemispheres not in the same position!");
        }
        else
        {
            position = Hemi1.transform.position;
            Hemi1.transform.position = new Vector3(0, 0, 0);
            Hemi2.transform.position = new Vector3(0, 0, 0);
        }
        if (Hemi1.transform.localScale == Hemi2.transform.localScale)
        {
            Hemi1.transform.localScale *= 2F;
        }
        Mesh n = CSG.Intersect(Hemi1, Hemi2);
        n.name = "Truncated Sphere";
        GameObject final = new GameObject("Final");
        final.AddComponent<MeshFilter>().sharedMesh = n;
        final.AddComponent<MeshRenderer>().sharedMaterial = material;
        final.AddComponent<MeshCollider>();
        final.GetComponent<MeshCollider>().sharedMesh = n;
        Hemi1.transform.localScale *= 0.5F;
        Hemi1.transform.position = position;
        Hemi2.transform.position = position;
        final.transform.position = position;
        final.layer = LayerMask.NameToLayer("final");

        GameObject VectorLineObj = DrawHemisphereLines(final, "final");
        VectorLineObj.transform.parent = final.transform;
        VectorLineObj.name = "HemiLine";


        return final;
    }

    public List<GameObject> createChildSpheres(GameObject origin, GameObject hemisphere, string layerName)
    {
        float scaling = origin.transform.localScale.x / 2;
        int numSpheres = 32;
        Vector3[] pts = PointsOnSphere(numSpheres);
        List<GameObject> uspheres = new List<GameObject>();
        int i = 0;

        foreach (Vector3 value in pts)
        {
            float scale = scaling / numSpheres * 2;
            int layerMask = 1 << LayerMask.NameToLayer(layerName);
            Vector3 Position = origin.transform.TransformPoint(value * scaling);
            Collider[] hitColliders = Physics.OverlapSphere(Position, scale / 2, layerMask);

            if (hitColliders.Length != 0)
            {
                uspheres.Add(GameObject.CreatePrimitive(PrimitiveType.Sphere));
                uspheres[i].transform.position = Position;
                uspheres[i].transform.localScale = new Vector3(scale, scale, scale);
                uspheres[i].transform.parent = hemisphere.transform;
                uspheres[i].SetActive(false);
                i++;
            }
        }
        return uspheres;
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

    private GameObject DrawHemisphereLines(GameObject hemisphere, string layerName)
    {
        GameObject Sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Sphere.transform.position = hemisphere.transform.position;
        Sphere.transform.localScale = hemisphere.transform.localScale;
        List<Transform> sphereList = new List<Transform>();
        VectorLine hemisphereLine = new VectorLine("NewHemiLine", new List<Vector3>(), 2.0f, LineType.Discrete);
        GameObject VectorLineObj = GameObject.Find("NewHemiLine");
        hemisphereLine.Draw3DAuto();

        List<GameObject> childList = createChildSpheres(Sphere, hemisphere, layerName);
        foreach (GameObject childSphere in childList)
        {
            hemisphereLine.points3.Add(Sphere.transform.position);
            hemisphereLine.points3.Add(childSphere.transform.position);

            sphereList.Add(Sphere.transform);
            sphereList.Add(childSphere.transform);
        }
        hemisphereLine.color = Color.green;
        //VectorLine line = new VectorLine("Wireframe", new List<Vector3>(), 1.0f, LineType.Discrete);
        //line.MakeWireframe(((MeshFilter)hemisphere.GetComponent("MeshFilter")).mesh);
        //line.drawTransform = hemisphere.transform;
        //line.Draw3DAuto();
        Sphere.SetActive(false);
        return VectorLineObj;
    }
}
