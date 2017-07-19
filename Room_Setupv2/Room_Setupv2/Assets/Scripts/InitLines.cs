using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vectrosity;
/*
 * This script is attached to the Domain Game Object and handles most of the Vectrosity work
 * It sets up a Wireframe for the Domain, as well as some code to update the position of all the Vectrosity line endpoints
 */
public class InitLines : MonoBehaviour {
    //--- PRIVATE VARIABLES ---//

    //--- PUBLIC VARIABLES ---//
    [Tooltip("GameObject containing Camera (CenterEyeAnchor)")]
    public Camera myCamera;

    [Tooltip("VectorLine object for main joints from Vectrosity library, is modified by the SpawnObject scripts on the spheres under the Right Controller")]
    public VectorLine mainLine;


    public VectorLine deformedLine;

    [Tooltip("List of Transforms that are in identical order as mainLine in order to keep the points updated")]
    public List<Transform> lineTransformList;

    
    public List<Transform> deformedLineTransformList;


    void Start () {
        VectorLine.SetCamera3D(myCamera);
        //Wireframe of cube
        VectorLine line = new VectorLine("Wireframe", new List<Vector3>(), 1.0f, LineType.Discrete);
        Mesh cubeMesh = ((MeshFilter)gameObject.GetComponent("MeshFilter")).mesh;
        line.MakeWireframe(cubeMesh);
        line.drawTransform = gameObject.transform;
        line.Draw3DAuto();

        mainLine = new VectorLine("MainLine", new List<Vector3>(), 4.0f);
        mainLine.Draw3DAuto();


        deformedLine = new VectorLine("MainLine", new List<Vector3>(), 10.0f);
        deformedLine.Draw3DAuto();
        
        lineTransformList = new List<Transform>();
        deformedLineTransformList = new List<Transform>();
    }

    void LateUpdate()
    {
        int i = 0;
        foreach (Transform transform in deformedLineTransformList)
        {
            if(transform.CompareTag("Input") || transform.CompareTag("Output") || transform.CompareTag("Intermediate") || transform.CompareTag("Fixed"))
            {
                deformedLine.points3[i] = transform.position;
                i++;
            }
        }
        i = 0;
        foreach (Transform transform in lineTransformList)
        {
            if (transform.CompareTag("Input") || transform.CompareTag("Output") || transform.CompareTag("Intermediate") || transform.CompareTag("Fixed"))
            {
                mainLine.points3[i] = transform.position;
                i++;
            }
        }
    }
}
