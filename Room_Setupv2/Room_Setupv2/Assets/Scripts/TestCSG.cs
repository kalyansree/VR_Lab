using System.Collections.Generic;
using UnityEngine;
using Vectrosity;


public class TestCSG : MonoBehaviour {
    private GameObject hemisphere1;
    private GameObject hemisphere2;
    private GameObject final1;
    private GameObject final2;
    private GameObject final3;

    private GameObject plane;

    

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
    public GameObject freedomTarget;

    public GameObject freedomPos;
    private GameObject origin;
    public GameObject pos;

    public Texture2D texture;

    bool showingHemisphere;
    bool calculatedTruncation;

    public Material mat;
    VectorLine demoLine;
    VectorLine dottedLine;
    GameObject lineObj;

    private GameObject plane1;

    void Start () {
        VectorLine.SetCamera3D(myCamera);
        demoLine = new VectorLine("demoLine", new List<Vector3>(), 20.0f, LineType.Discrete);
        Hemisphere.CreateHemisphereMesh();
        demoLine.points3.Add(target1.transform.position);
        demoLine.points3.Add(pos.transform.position);
        demoLine.points3.Add(target2.transform.position);
        demoLine.points3.Add(pos.transform.position);
        demoLine.Draw3DAuto();

        origin = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        origin.transform.position = pos.transform.position;
        Destroy(origin.GetComponent<MeshRenderer>());

        freedomPos = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        freedomPos.transform.localScale = new Vector3(0.01F, 0.01F, 0.01F);
        Destroy(freedomPos.GetComponent<MeshRenderer>());


    }

    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            hemisphere1 = Hemisphere.CreateHemisphere(hemisphereMaterial1, pos.transform.position, target1.transform.position, false);  
            //updatePos = true;
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            hemisphere2 = Hemisphere.CreateHemisphere(hemisphereMaterial2, pos.transform.position, target2.transform.position, false);
            final1 = Hemisphere.GetIntersection(hemisphere1, hemisphere2, truncatedHemisphereMaterial, true);
        }

        if(Input.GetKeyDown(KeyCode.S))
        {
            dottedLine = new VectorLine("dotted", new List<Vector3>(), texture, 16.0f);
            dottedLine.points3.Add(pos.transform.position);
            dottedLine.points3.Add(freedomTarget.transform.position);
            dottedLine.textureScale = 1.00f;
            dottedLine.Draw3DAuto();
            lineObj = GameObject.Find("dotted");
            plane = GameObject.CreatePrimitive(PrimitiveType.Cube);
            plane.transform.localScale = new Vector3(10, 10, 0.001F);
            plane.transform.rotation = Quaternion.LookRotation(origin.transform.position - plane.transform.position);
            plane.transform.position = pos.transform.position;
            plane.GetComponent<MeshRenderer>().sharedMaterial = mat;

            updatePos = true;
        }

        if (Input.GetKeyDown(KeyCode.B))
        { 
            ToggleView();
        }

        if (updatePos)
        {
           
            hemisphere1.transform.rotation = Quaternion.LookRotation(target1.transform.position - hemisphere1.transform.position);
            hemisphere2.transform.rotation = Quaternion.LookRotation(target2.transform.position - hemisphere2.transform.position);
           
            Vector3 euler = hemisphere1.transform.rotation.eulerAngles;
            hemisphere1.transform.eulerAngles = new Vector3(euler.z, euler.y + 90F, euler.x);
            euler = hemisphere2.transform.rotation.eulerAngles;
            hemisphere2.transform.eulerAngles = new Vector3(euler.z, euler.y + 90F, euler.x);
            dottedLine.points3[0] = pos.transform.position;
            dottedLine.points3[1] = freedomTarget.transform.position;
            plane.transform.rotation = Quaternion.LookRotation(origin.transform.position - freedomTarget.transform.position);
            //freedomPos.transform.position = Physics.ClosestPoint(freedomTarget.transform.position, origin.GetComponent<Collider>(), origin.transform.position, origin.transform.rotation);

            //int layerMask = 1 << LayerMask.NameToLayer("hemisphere");
            //Collider[] list = Physics.OverlapSphere(freedomPos.transform.position, freedomPos.transform.localScale.x, layerMask);
            //print(list.Length);
            //if (list.Length == 2)
            //{
            //    lineObj.SetActive(true);
            //}
            //else
            //{
            //    lineObj.SetActive(false);
            //}

            if (InTruncation(origin, freedomTarget)) {
                lineObj.SetActive(true);
            }
            else
            {
                lineObj.SetActive(false);
            }
        }
        demoLine.points3[0] = target1.transform.position;
        demoLine.points3[1] = pos.transform.position;
        demoLine.points3[2] = target2.transform.position;
        demoLine.points3[3] = pos.transform.position;



    }

    void ToggleView()
    {
        if(showingHemisphere)
        {
            showingHemisphere = false;
            hemisphere1.GetComponent<MeshRenderer>().enabled = false;
            hemisphere2.GetComponent<MeshRenderer>().enabled = false;
            final1.GetComponent<MeshRenderer>().enabled = false;
        }
        else
        {
            showingHemisphere = true;
            hemisphere1.GetComponent<MeshRenderer>().enabled = true;
            hemisphere2.GetComponent<MeshRenderer>().enabled = true;
            final1.GetComponent<MeshRenderer>().enabled = true;

        }
    }

    private bool InTruncation(GameObject origin, GameObject dest)
    {
        freedomPos.transform.position = Physics.ClosestPoint(dest.transform.position, origin.GetComponent<Collider>(), origin.transform.position, origin.transform.rotation);
        int layerMask = 1 << LayerMask.NameToLayer("hemisphere");
        Collider[] list = Physics.OverlapSphere(freedomPos.transform.position, freedomPos.transform.lossyScale.x, layerMask);
        print(list.Length);
        if (list.Length == 2)
            return true;
        else
            return false;

    }
}
