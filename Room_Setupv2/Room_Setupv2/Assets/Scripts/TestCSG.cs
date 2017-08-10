using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Parabox.CSG;
using Vectrosity;


public class TestCSG : MonoBehaviour {
    private GameObject hemisphere1;
    private GameObject hemisphere2;
    private GameObject hemisphere3;
    private GameObject hemisphere4;
    private GameObject final1;
    private GameObject final2;
    private GameObject final3;

    private GameObject cube1;
    private GameObject cube2;
    private GameObject cube3;
    private GameObject cube4;

    private bool updatePos;


    //PUBLIC
    public Material hemisphereMaterial1;
    public Material hemisphereMaterial2;
    public Material hemisphereMaterial3;
    public Material hemisphereMaterial4;
    public Material truncatedHemisphereMaterial;

    public Camera myCamera;
    //ONLY FOR TESTING PURPOSES
    //public bool updatePos;
    public GameObject target1;
    public GameObject target2;
    public GameObject target3;
    public GameObject target4;
    public GameObject pos;

    public bool flip1;
    public bool flip2;
    public bool flip3;
    public bool flip4;

    bool showingHemisphere;
    bool calculatedTruncation;

    VectorLine demoLine;

    private GameObject plane1;

    void Start () {
        VectorLine.SetCamera3D(myCamera);
        demoLine = new VectorLine("demoLine", new List<Vector3>(), 20.0f, LineType.Discrete);
        Hemisphere.CreateHemisphereMesh();
        demoLine.points3.Add(target1.transform.position);
        demoLine.points3.Add(pos.transform.position);
        demoLine.points3.Add(target2.transform.position);
        demoLine.points3.Add(pos.transform.position);
        demoLine.points3.Add(target3.transform.position);
        demoLine.points3.Add(pos.transform.position);
        demoLine.points3.Add(target4.transform.position);
        demoLine.points3.Add(pos.transform.position);
        demoLine.Draw3DAuto();
    }

    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            hemisphere1 = Hemisphere.CreateHemisphere(hemisphereMaterial1, pos.transform.position, target1.transform.position, flip1);
            hemisphere2 = Hemisphere.CreateHemisphere(hemisphereMaterial2, pos.transform.position, target2.transform.position, flip2);
            hemisphere3 = Hemisphere.CreateHemisphere(hemisphereMaterial3, pos.transform.position, target3.transform.position, flip3);
            hemisphere4 = Hemisphere.CreateHemisphere(hemisphereMaterial4, pos.transform.position, target4.transform.position, flip4);
            cube1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube3 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube4 = GameObject.CreatePrimitive(PrimitiveType.Cube);

            updatePos = true;
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            final1 = GetIntersection(hemisphere1, cube2, truncatedHemisphereMaterial, true);
            hemisphere2.SetActive(false);
            cube1.SetActive(false);
        }
        if(Input.GetKeyDown(KeyCode.S))
        {
            final2 = GetIntersection(final1, cube3, truncatedHemisphereMaterial, true);
            hemisphere3.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            final3 = GetIntersection(final2, cube4, truncatedHemisphereMaterial, true);
            hemisphere4.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            ToggleView();
        }



        if (updatePos)
        {
            SetCubePosition(ref cube1, hemisphere1, target1);
            SetCubePosition(ref cube2, hemisphere2, target2);
            SetCubePosition(ref cube3, hemisphere3, target3);
            SetCubePosition(ref cube4, hemisphere4, target4);
            if (flip1)
            {
                hemisphere1.transform.rotation = Quaternion.LookRotation(hemisphere1.transform.position - target1.transform.position);
            }
            else
            {
                hemisphere1.transform.rotation = Quaternion.LookRotation(target1.transform.position - hemisphere1.transform.position);
            }
            Vector3 euler = hemisphere1.transform.rotation.eulerAngles;
            hemisphere1.transform.eulerAngles = new Vector3(euler.z, euler.y + 90F, euler.x);

            if (flip2)
            {
                hemisphere2.transform.rotation = Quaternion.LookRotation(hemisphere2.transform.position - target2.transform.position);
            }
            else
            {
                hemisphere2.transform.rotation = Quaternion.LookRotation(target2.transform.position - hemisphere2.transform.position);
            }
            euler = hemisphere2.transform.rotation.eulerAngles;
            hemisphere2.transform.eulerAngles = new Vector3(euler.z, euler.y + 90F, euler.x);

            if (flip3)
            {
                hemisphere3.transform.rotation = Quaternion.LookRotation(hemisphere3.transform.position - target3.transform.position);
            }
            else
            {
                hemisphere3.transform.rotation = Quaternion.LookRotation(target3.transform.position - hemisphere3.transform.position);
            }
            euler = hemisphere3.transform.rotation.eulerAngles;
            hemisphere3.transform.eulerAngles = new Vector3(euler.z, euler.y + 90F, euler.x);

            if (flip4)
            {
                hemisphere4.transform.rotation = Quaternion.LookRotation(hemisphere4.transform.position - target4.transform.position);
            }
            else
            {
                hemisphere4.transform.rotation = Quaternion.LookRotation(target4.transform.position - hemisphere4.transform.position);
            }
            euler = hemisphere4.transform.rotation.eulerAngles;
            hemisphere4.transform.eulerAngles = new Vector3(euler.z, euler.y + 90F, euler.x);
        }
        demoLine.points3[0] = target1.transform.position;
        demoLine.points3[1] = pos.transform.position;
        demoLine.points3[2] = target2.transform.position;
        demoLine.points3[3] = pos.transform.position;
        demoLine.points3[4] = target3.transform.position;
        demoLine.points3[5] = pos.transform.position;
        demoLine.points3[6] = target4.transform.position;
        demoLine.points3[7] = pos.transform.position;

    }

    void ToggleView()
    {
        if(showingHemisphere)
        {
            showingHemisphere = false;
            hemisphere1.SetActive(false);
            hemisphere2.SetActive(false);
            hemisphere3.SetActive(false);
            hemisphere4.SetActive(false);
            final1.SetActive(false);
            final2.SetActive(false);
            final3.SetActive(true);
        }
        else
        {
            showingHemisphere = true;
            hemisphere1.SetActive(true);
            hemisphere2.SetActive(true);
            hemisphere3.SetActive(true);
            hemisphere4.SetActive(true);
            final1.SetActive(true);
            final2.SetActive(true);
            final3.SetActive(false);
        }
    }

    void SetCubePosition(ref GameObject Cube, GameObject Hemisphere, GameObject Target)
    {
        Cube.transform.localScale = new Vector3(1.1F, 1.1F, 1.1F);
        Cube.transform.position = Hemisphere.transform.position;
        Cube.transform.Translate(Vector3.forward * (Cube.transform.localScale.x / 2));
        Cube.GetComponent<MeshRenderer>().sharedMaterial = Hemisphere.GetComponent<MeshRenderer>().sharedMaterial;
        Cube.transform.rotation = Quaternion.LookRotation(Hemisphere.transform.position - Target.transform.position);

    }

    public static GameObject GetIntersection(GameObject obj, GameObject cube, Material material, bool disableHemispheres)
    {
        //Vector3 position;

        //position = obj.transform.position;
        //obj.transform.position = new Vector3(0, 0, 0);
        //cube.transform.position = obj.transform.position;
        //cube.transform.Translate(Vector3.forward * (cube.transform.localScale.x / 2));

        if (obj.transform.localScale == cube.transform.localScale)
        {
            cube.transform.localScale *= 1.05F;
        }
        Mesh n = CSG.Intersect(cube, obj);
        n.name = "Truncated Sphere";
        GameObject final = new GameObject("Final");
        final.AddComponent<MeshFilter>().sharedMesh = n;
        final.AddComponent<MeshRenderer>().sharedMaterial = material;
        final.AddComponent<MeshCollider>();
        final.GetComponent<MeshCollider>().sharedMesh = n;
        //cube.transform.localScale /= 1.05F;
        //obj.transform.position = position;
        //cube.transform.position = position;
        //final.transform.position = position;
        final.layer = LayerMask.NameToLayer("final");

        //GameObject VectorLineObj = DrawHemisphereLines(final, "final");
        //VectorLineObj.transform.parent = final.transform;
        //VectorLineObj.name = "HemiLine";

        if (disableHemispheres)
        {
            obj.gameObject.SetActive(false);
            cube.gameObject.SetActive(false);
        }

        return final;
    }
}
