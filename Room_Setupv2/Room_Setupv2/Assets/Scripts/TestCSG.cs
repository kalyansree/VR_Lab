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

    void Start () {
        VectorLine.SetCamera3D(myCamera);
    }

    void Update()
    {
        if(Input.GetKeyDown("space"))
        {
            hemisphere1 = Hemisphere.CreateHemisphere(hemisphereMaterial1, new Vector3(1, 1, 1), target1.transform.position, false);
            hemisphere2 = Hemisphere.CreateHemisphere(hemisphereMaterial2, new Vector3(1, 1, 1), target2.transform.position, true);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            final = Hemisphere.GetIntersection(hemisphere1, hemisphere2, truncatedHemisphereMaterial, true);
        }

        //if(updatePos)
        //{
        //    hemisphere1.transform.rotation = Quaternion.LookRotation(target.transform.position - hemisphere1.transform.position);
        //    Vector3 euler = hemisphere1.transform.rotation.eulerAngles;
        //    hemisphere1.transform.eulerAngles = new Vector3(euler.z, euler.y + 90F, euler.x);
        //}
    }
}
