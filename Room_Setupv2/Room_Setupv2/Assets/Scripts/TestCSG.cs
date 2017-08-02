using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Parabox.CSG;
using Vectrosity;


public class TestCSG : MonoBehaviour {
    private GameObject hemisphere1;
    private GameObject hemisphere2;
    private GameObject final;


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

    bool showingHemisphere;
    bool calculatedTruncation;

    VectorLine demoLine;

    void Start () {
        VectorLine.SetCamera3D(myCamera);
        demoLine = new VectorLine("demoLine", new List<Vector3>(), 20.0f, LineType.Discrete);
        demoLine.points3.Add(target1.transform.position);
        demoLine.points3.Add(pos.transform.position);
        demoLine.points3.Add(target2.transform.position);
        demoLine.points3.Add(pos.transform.position);
        demoLine.Draw3DAuto();
    }

    void Update()
    {
        if(Input.GetKeyDown("space"))
        {
            hemisphere1 = Hemisphere.CreateHemisphere(hemisphereMaterial1, pos.transform.position, target1.transform.position, false);
            hemisphere2 = Hemisphere.CreateHemisphere(hemisphereMaterial2, pos.transform.position, target2.transform.position, true);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            final = Hemisphere.GetIntersection(hemisphere1, hemisphere2, truncatedHemisphereMaterial, true);
            calculatedTruncation = true;
        }

        //if(updatePos)
        //{
        //    hemisphere1.transform.rotation = Quaternion.LookRotation(target.transform.position - hemisphere1.transform.position);
        //    Vector3 euler = hemisphere1.transform.rotation.eulerAngles;
        //    hemisphere1.transform.eulerAngles = new Vector3(euler.z, euler.y + 90F, euler.x);
        //}
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
