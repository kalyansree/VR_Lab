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

    public List<Transform> forceLineTransformList;

    public List<Vector3> coordLineVectorList_x;
    public List<Vector3> coordLineVectorList_y;
    public List<Vector3> coordLineVectorList_z;
    public List<Vector3> forceVectorList;

    public List<VectorLine> forceLineList;
    public Texture2D frontTex;
    public Texture2D lineTex;
    public Texture2D backTex;

    private VectorLine coordLine_x;
    private VectorLine coordLine_y;
    private VectorLine coordLine_z;

    public GameObject originPos;


    void Start () {
        VectorLine.SetCamera3D(myCamera);
        //Wireframe of cube
        VectorLine line = new VectorLine("Wireframe", new List<Vector3>(), 1.0f, LineType.Discrete);
        VectorLine.SetEndCap("Arrow", EndCap.Both, -1.0F, lineTex, frontTex, backTex);
        Mesh cubeMesh = ((MeshFilter)gameObject.GetComponent("MeshFilter")).mesh;
        line.MakeWireframe(cubeMesh);
        line.drawTransform = gameObject.transform;
        line.Draw3DAuto();

        mainLine = new VectorLine("MainLine", new List<Vector3>(), 4.0f);
        mainLine.Draw3DAuto();


        deformedLine = new VectorLine("deformedLine", new List<Vector3>(), 10.0f);
        deformedLine.Draw3DAuto();

        coordLine_x = new VectorLine("coordLine", new List<Vector3>(), 2.0f);
        coordLine_x.endCap = "Arrow";
        coordLine_x.Draw3DAuto();

        coordLine_y = new VectorLine("coordLine", new List<Vector3>(), 2.0f);
        coordLine_y.endCap = "Arrow";
        coordLine_y.Draw3DAuto();

        coordLine_z = new VectorLine("coordLine", new List<Vector3>(), 2.0f);
        coordLine_z.endCap = "Arrow";
        coordLine_z.Draw3DAuto();

        coordLineVectorList_x.Add(new Vector3(0 ,0 ,0));
        coordLineVectorList_x.Add(new Vector3(0, 0, 0));

        coordLineVectorList_y.Add(new Vector3(0, 0, 0));
        coordLineVectorList_y.Add(new Vector3(0, 0, 0));

        coordLineVectorList_z.Add(new Vector3(0, 0, 0));
        coordLineVectorList_z.Add(new Vector3(0, 0, 0));

        lineTransformList = new List<Transform>();
        deformedLineTransformList = new List<Transform>();

        //Force Line
        forceLineTransformList = new List<Transform>();
        forceVectorList = new List<Vector3>();
        forceLineList = new List<VectorLine>();
        //forceLine = new VectorLine("ForceLine", new List<Vector3>(), 30.0f);
        //VectorLine.SetEndCap("Arrow", EndCap.Both, -1.0F, lineTex, frontTex, backTex);
        //forceLine.endCap = "Arrow";
        //forceLine.Draw3DAuto();
    }

    void Update()
    {
        
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
        i = 0;
        foreach (Transform transform in forceLineTransformList)
        {
            forceLineList[Mathf.FloorToInt(i/2)].points3[i%2] = transform.position;
            i++;
        }

        i = 0;
        foreach (Transform transform in coordLineTransformList)
        {
            coordLine.points3[i] = transform.position;
            i++;
        }
    }
}
