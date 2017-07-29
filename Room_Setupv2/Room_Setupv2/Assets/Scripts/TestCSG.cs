using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Parabox.CSG;
using Vectrosity;


public class TestCSG : MonoBehaviour {
    private GameObject hemisphere1;
    private GameObject hemisphere2;

    private GameObject final;

    public Material hemisphereMaterial;

    private VectorLine hemisphereLine;

    private List<Transform> sphereList;
    public Camera myCamera;
    public GameObject target;
    void Start () {
        sphereList = new List<Transform>();
        Vector3 RotationVec = new Vector3(0,1,0);
        Vector3 scale = new Vector3(1, 1, 1);
        Vector3 position = new Vector3(0, 0, 0);
        VectorLine.SetCamera3D(myCamera);
        hemisphereLine = new VectorLine("HemiLine", new List<Vector3>(), 2.0f, LineType.Discrete);
        hemisphereLine.Draw3DAuto();
        hemisphere1 = CreateHemisphere();
        //DrawHemisphereLines(hemisphere1, "hemisphere");
        hemisphere1.SetActive(false);
        hemisphere2 = CreateHemisphere(Rotation: Quaternion.LookRotation(RotationVec,  RotationVec));
        //composite.SetActive(true);
        //hemisphere2.SetActive(false);
        print(LayerMask.NameToLayer("hemisphere"));


  

    }
    void Update()
    {
        if(Input.GetKeyDown("space"))
        {
            final = GetIntersection(hemisphere1, hemisphere2);
            //final = GetIntersection(Hemisphere, Hemisphere2);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            DrawHemisphereLines(final, "final");
        }
        int i = 0;
        foreach (Transform transform in sphereList)
        {
            hemisphereLine.points3[i] = sphereList[i].position;
            i++;
        }

        
        hemisphere2.transform.rotation = Quaternion.LookRotation(target.transform.position);
        Vector3 euler = hemisphere2.transform.rotation.eulerAngles;
        hemisphere2.transform.eulerAngles = new Vector3(euler.z, euler.y + 90F, euler.x);
        

    }

    public GameObject CreateHemisphere(Vector3 Position = default(Vector3), Quaternion Rotation = default(Quaternion), Vector3? Scale = null)
    {
        GameObject Sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        GameObject Cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Cube.transform.localScale = new Vector3(2, 2, 2);
        GameObject Pivot = new GameObject();
        Sphere.transform.position = Position;
        Cube.transform.position = Position + new Vector3(1,0,0);
        Pivot.transform.position = Position;
        Sphere.transform.parent = Pivot.transform;
        Cube.transform.parent = Pivot.transform;
        //Pivot.transform.rotation = Rotation;

        
        Mesh m = CSG.Intersect(Sphere, Cube);
        m.name = "Hemisphere Mesh";
        GameObject composite = new GameObject("Hemisphere");
        composite.AddComponent<MeshFilter>().sharedMesh = m;
        composite.AddComponent<MeshRenderer>().sharedMaterial = hemisphereMaterial;
        if (Scale == null)
            composite.transform.localScale = new Vector3(1, 1, 1);
        else
            composite.transform.localScale = (Vector3)Scale;
        composite.AddComponent<MeshCollider>();
        composite.GetComponent<MeshCollider>().sharedMesh = m;
        composite.layer = LayerMask.NameToLayer("hemisphere");
        composite.transform.rotation = Rotation;
        Destroy(Sphere);
        Destroy(Pivot);
        Destroy(Cube);
        return composite;
    }

    public GameObject GetIntersection(GameObject Hemi1, GameObject Hemi2, Vector3 Position = default(Vector3), Vector3? Scale = null)
    {
        
        if (Hemi1.transform.localScale == Hemi2.transform.localScale)
        {
            Hemi1.transform.localScale *= 2F;
        }
        Mesh n = CSG.Intersect(Hemi1, Hemi2);
        n.name = "Truncated Sphere";
        GameObject final = new GameObject("Final");
        final.AddComponent<MeshFilter>().sharedMesh = n;
        final.AddComponent<MeshRenderer>().sharedMaterial = hemisphereMaterial;
        final.AddComponent<MeshCollider>();
        final.GetComponent<MeshCollider>().sharedMesh = n;
        final.layer = LayerMask.NameToLayer("final");
        Destroy(Hemi1);
        Destroy(Hemi2);

        return final;
    }

    public List<GameObject> createChildSpheres(GameObject origin, GameObject hemisphere, string layerName)
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
            int layerMask = 1 << LayerMask.NameToLayer(layerName);
            Collider[] hitColliders = Physics.OverlapSphere(uspheres[i].transform.position, scale / 2, layerMask);

            if (hitColliders.Length == 0)
            {
                GameObject obj = uspheres[i];
                uspheres.Remove(obj);
                Destroy(obj);
            }
            else
            {
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

    private void DrawHemisphereLines(GameObject hemisphere, string layerName, Vector3 Position = default(Vector3), Vector3? Scale = null)
    {
        GameObject Sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Sphere.transform.position = Position;
        if (Scale == null)
            Sphere.transform.localScale = new Vector3(1, 1, 1);
        else
            Sphere.transform.localScale = (Vector3)Scale;

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
    }
}
