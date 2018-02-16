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

    public List<Vector3> forceVectorList;
    public Texture2D frontTex;
    public Texture2D lineTex;
    public Texture2D backTex;

    public List<List<Vector3>> pointLists;
    public List<VectorLine> deformedLineList;

    public Color mainColor = Color.black;

    void Awake()
    {
        VectorLine.SetEndCap("Arrow", EndCap.Both, -1.0F, lineTex, frontTex, backTex);
    }

    void Start () {
        deformedLineList = new List<VectorLine>();
        pointLists = new List<List<Vector3>>();
        VectorLine.SetCamera3D(myCamera);
        //Wireframe of cube
        VectorLine line = new VectorLine("Wireframe", new List<Vector3>(), 1.0f, LineType.Discrete);

        Mesh cubeMesh = ((MeshFilter)gameObject.GetComponent("MeshFilter")).mesh;
        line.MakeWireframe(cubeMesh);
        line.drawTransform = gameObject.transform;
        line.Draw3DAuto();

        mainLine = new VectorLine("MainLine", new List<Vector3>(), 10.0f);
        mainLine.SetColor(mainColor);
        mainLine.Draw3DAuto();

        deformedLine = new VectorLine("deformedLine", new List<Vector3>(), 10.0f);
        deformedLine.Draw3DAuto();

        lineTransformList = new List<Transform>();
        deformedLineTransformList = new List<Transform>();

        //Force Line
        forceVectorList = new List<Vector3>();
        //forceLine = new VectorLine("ForceLine", new List<Vector3>(), 30.0f);
        //VectorLine.SetEndCap("Arrow", EndCap.Both, -1.0F, lineTex, frontTex, backTex);
        //forceLine.endCap = "Arrow";
        //forceLine.Draw3DAuto();
    }
    void Update()
    {
        mainLine.SetColor(mainColor);    
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

        for (i = 0; i < deformedLineList.Count; i++)
        {
            List<Vector3> worldCoords = new List<Vector3>();
            foreach (Vector3 point in pointLists[i])
            {
                worldCoords.Add(transform.TransformPoint(point));
            }
            deformedLineList[i].points3 = worldCoords;
        }
    }

    public void addDeformedLine(VectorLine vectorLine, List<Vector3> pointList)
    {
        deformedLineList.Add(vectorLine);
        pointLists.Add(pointList);
    }
}
