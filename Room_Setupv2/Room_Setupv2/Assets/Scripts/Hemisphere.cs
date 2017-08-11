using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Parabox.CSG;
using Vectrosity;


public static class Hemisphere
{
    private static Mesh hemisphereMesh;

    public static void CreateHemisphereMesh()
    {
        GameObject Sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        GameObject Cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Cube.transform.localScale = new Vector3(2, 2, 2);
        Sphere.transform.position = new Vector3(0, 0, 0);
        Cube.transform.position = new Vector3(1, 0, 0);

        GameObject.Destroy(Sphere);
        GameObject.Destroy(Cube);
        Mesh m = CSG.Intersect(Sphere, Cube);
        m.name = "Hemisphere Mesh";
        hemisphereMesh = m;
    }

    public static GameObject CreateHemisphere(Material material, Vector3 Position, Vector3 Target, bool oppositeDir, Vector3? Scale = null)
    {
        GameObject hemisphereGameObject = new GameObject("Hemisphere");
        hemisphereGameObject.AddComponent<MeshFilter>().sharedMesh = hemisphereMesh;
        hemisphereGameObject.AddComponent<MeshRenderer>().sharedMaterial = material;
        if (Scale == null)
            hemisphereGameObject.transform.localScale = new Vector3(1, 1, 1);
        else
            hemisphereGameObject.transform.localScale = (Vector3)Scale;
        hemisphereGameObject.AddComponent<MeshCollider>();
        hemisphereGameObject.GetComponent<MeshCollider>().sharedMesh = hemisphereMesh;
        hemisphereGameObject.layer = LayerMask.NameToLayer("hemisphere");
        hemisphereGameObject.tag = "Hemisphere";

        if (oppositeDir)
        {
            hemisphereGameObject.transform.rotation = Quaternion.LookRotation(Position - Target);
        }
        else
        {
            hemisphereGameObject.transform.rotation = Quaternion.LookRotation(Target - Position);
        }
        Vector3 euler = hemisphereGameObject.transform.rotation.eulerAngles;
        hemisphereGameObject.transform.eulerAngles = new Vector3(euler.z, euler.y + 90F, euler.x);

        hemisphereGameObject.transform.position = Position;

        //Cube
        GameObject cubeGameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cubeGameObject.name = "GuideCube";
        GameObject.Destroy(cubeGameObject.GetComponent<MeshRenderer>());
        SetCubePosition(ref cubeGameObject, hemisphereGameObject, Target, oppositeDir);
        cubeGameObject.transform.parent = hemisphereGameObject.transform;

        //this is not working in the Room Scene, although it is working in TestCSG for some reason -- need to test some more
        //GameObject VectorLineObj = DrawHemisphereLines(hemisphereGameObject, "hemisphere");
        //VectorLineObj.transform.parent = hemisphereGameObject.transform;
        //VectorLineObj.name = "HemiLine";
        return hemisphereGameObject;
    }

    public static GameObject GetIntersection(GameObject obj, GameObject hemisphere, Material material, bool disableHemispheres)
    {

        GameObject cube = hemisphere.transform.Find("GuideCube").gameObject;
        Vector3 size = cube.transform.lossyScale;
        size.x -= 0.01F;
        size.y -= 0.01F;
        size.z -= 0.01F;
        Collider[] hitColliders = Physics.OverlapBox(cube.transform.position, size / 2, cube.transform.rotation);
        bool isColliding = false;
        foreach (Collider col in hitColliders)
        {
            if (col.gameObject == obj)
            {
                isColliding = true;
            }
        }
        if (!isColliding)
        {
            hemisphere.gameObject.SetActive(false);
            obj.gameObject.SetActive(false);
            return null;
        }
        Vector3 position;
        position = obj.transform.position;
        obj.transform.position = new Vector3(0, 0, 0);
        hemisphere.transform.position = obj.transform.position;
        Mesh n = CSG.Intersect(cube, obj);
        n.name = "Truncated Sphere";
        GameObject final = new GameObject("Final");
        final.AddComponent<MeshFilter>().sharedMesh = n;
        final.AddComponent<MeshRenderer>().sharedMaterial = material;
        final.AddComponent<MeshCollider>();
        final.GetComponent<MeshCollider>().sharedMesh = n;
        obj.transform.position = position;
        hemisphere.transform.position = position;
        final.transform.position = position;
        final.layer = LayerMask.NameToLayer("final");
        final.tag = "Final";
        //GameObject VectorLineObj = DrawHemisphereLines(final, "final");
        //VectorLineObj.transform.parent = final.transform;
        //VectorLineObj.name = "HemiLine";

        if (disableHemispheres)
        {
            obj.gameObject.SetActive(false);
            hemisphere.gameObject.SetActive(false);
        }

        return final;
    }

    private static List<GameObject> createChildSpheres(GameObject origin, GameObject hemisphere, string layerName, float radius)
    {
        float scaling = radius;
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

    private static Vector3[] PointsOnSphere(int n)
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

    private static GameObject DrawHemisphereLines(GameObject hemisphere, string layerName)
    {
        GameObject Sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        GameObject.Destroy(Sphere.GetComponent<Collider>());
        Sphere.transform.position = hemisphere.transform.position;
        Sphere.transform.localScale = hemisphere.transform.lossyScale;
        VectorLine hemisphereLine = new VectorLine("NewHemiLine", new List<Vector3>(), 2.0f, LineType.Discrete);
        GameObject VectorLineObj = GameObject.Find("NewHemiLine");
        hemisphereLine.Draw3DAuto();

        List<GameObject> childList = createChildSpheres(Sphere, hemisphere, layerName, Sphere.transform.localScale.x / 2);
        foreach (GameObject childSphere in childList)
        {
            hemisphereLine.points3.Add(Sphere.transform.position);
            hemisphereLine.points3.Add(childSphere.transform.position);
        }
        hemisphereLine.color = Color.green;
        //VectorLine line = new VectorLine("Wireframe", new List<Vector3>(), 1.0f, LineType.Discrete);
        //line.MakeWireframe(((MeshFilter)hemisphere.GetComponent("MeshFilter")).mesh);
        //line.drawTransform = hemisphere.transform;
        //line.Draw3DAuto();
        Sphere.SetActive(false);
        return VectorLineObj;
    }

    private static void SetCubePosition(ref GameObject Cube, GameObject Hemisphere, Vector3 Target, bool flip)
    {
        Cube.transform.localScale = new Vector3(1.1F, 1.1F, 1.1F);
        Cube.transform.position = Hemisphere.transform.position;
        Cube.GetComponent<MeshRenderer>().sharedMaterial = Hemisphere.GetComponent<MeshRenderer>().sharedMaterial;
        if(flip)
        {
            Cube.transform.rotation = Quaternion.LookRotation(Hemisphere.transform.position - Target);
        }
        else
        {
            Cube.transform.rotation = Quaternion.LookRotation(Target - Hemisphere.transform.position);
        }

        Cube.transform.Translate(Vector3.back * (Cube.transform.localScale.x / 2));

    }
}
