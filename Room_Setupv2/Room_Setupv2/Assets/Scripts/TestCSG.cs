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
        flip2 = true;
        flip4 = true;
    }

    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            hemisphere1 = Hemisphere.CreateHemisphere(hemisphereMaterial1, pos.transform.position, target1.transform.position, flip1);
            hemisphere2 = Hemisphere.CreateHemisphere(hemisphereMaterial2, pos.transform.position, target2.transform.position, flip2);
            hemisphere3 = Hemisphere.CreateHemisphere(hemisphereMaterial3, pos.transform.position, target3.transform.position, flip3);
            hemisphere4 = Hemisphere.CreateHemisphere(hemisphereMaterial4, pos.transform.position, target4.transform.position, flip4);
            updatePos = true;
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            final1 = Hemisphere.GetIntersection(hemisphere1, hemisphere2, truncatedHemisphereMaterial, true);
        }
        if(Input.GetKeyDown(KeyCode.S))
        {
            final2 = Hemisphere.GetIntersection(final1, hemisphere3, truncatedHemisphereMaterial, true);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            final3 = Hemisphere.GetIntersection(final2, hemisphere4, truncatedHemisphereMaterial, true);
        }

        if (Input.GetKeyDown(KeyCode.B))
        { 
            ToggleView();
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
}
