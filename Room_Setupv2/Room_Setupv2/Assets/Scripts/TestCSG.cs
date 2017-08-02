using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Parabox.CSG;
using Vectrosity;


public class TestCSG : MonoBehaviour {
    private GameObject hemisphere1;
    private GameObject hemisphere2;
    private GameObject final;
    private bool updatePos;


    //PUBLIC
    public Material hemisphereMaterial1;
    public Material hemisphereMaterial2;
    public Material truncatedHemisphereMaterial;

    public Camera myCamera;
    //ONLY FOR TESTING PURPOSES
    //public bool updatePos;
    public GameObject target1;
    public GameObject target2;
    public GameObject pos;

    public bool flip1;
    public bool flip2;

    bool showingHemisphere;
    bool calculatedTruncation;

    VectorLine demoLine;

    private GameObject plane1;

    void Start () {
        VectorLine.SetCamera3D(myCamera);
        demoLine = new VectorLine("demoLine", new List<Vector3>(), 20.0f, LineType.Discrete);
        demoLine.points3.Add(target1.transform.position);
        demoLine.points3.Add(pos.transform.position);
        demoLine.points3.Add(target2.transform.position);
        demoLine.points3.Add(pos.transform.position);
        demoLine.points3.Add(target2.transform.position);
        demoLine.points3.Add(pos.transform.position);
        demoLine.Draw3DAuto();
    }

    void Update()
    {
        if(Input.GetKeyDown("space"))
        {
            hemisphere1 = Hemisphere.CreateHemisphere(hemisphereMaterial1, pos.transform.position, target1.transform.position, flip1);
            hemisphere2 = Hemisphere.CreateHemisphere(hemisphereMaterial2, pos.transform.position, target2.transform.position, flip2);
            plane1 = GameObject.CreatePrimitive(PrimitiveType.Plane);
            plane1.transform.localScale = new Vector3(0.1F, 0.1F, 0.1F);
            updatePos = true;
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            final = Hemisphere.GetIntersection(hemisphere1, hemisphere2, truncatedHemisphereMaterial, true);
            calculatedTruncation = true;
        }

        if (updatePos)
        {
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

            Vector3 normal = Vector3.Cross(target1.transform.position - pos.transform.position, target2.transform.position - pos.transform.position);
            //plane with normal=normal and contains the point pos.transform.position
            Plane targetPlane = new Plane(normal, pos.transform.position);
            plane1.transform.rotation = Quaternion.LookRotation(targetPlane.normal);
            plane1.transform.position = pos.transform.position;
            demoLine.points3[4] = pos.transform.position + normal;
            demoLine.points3[5] = pos.transform.position;
        }
        if (calculatedTruncation && Input.GetKeyDown(KeyCode.B))
        {
            ToggleView();
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
            hemisphere1.SetActive(false);
            hemisphere2.SetActive(false);
            final.SetActive(true);
        }
        else
        {
            showingHemisphere = true;
            hemisphere1.SetActive(true);
            hemisphere2.SetActive(true);
            final.SetActive(false);
        }

    }
}
