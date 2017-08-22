﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Vectrosity;
using System.Text;

public class DrawFreedomLine : MonoBehaviour
{
    //--- PUBLIC VARIABLES ---//
    [Tooltip("GameObject of the Domain Cube")]
    public GameObject domain;

    [Tooltip("GameObject of Right Controller")]
    public GameObject RightController;
    [Tooltip("GameObject of Networking")]
    public GameObject Networking;

    public Texture2D dottedTexture;

    public GameObject LeftRadialMenu;

    [Tooltip("GameObject containing Camera (CenterEyeAnchor)")]
    public Camera myCamera;

    [Tooltip("GameObject of Preview Sphere")]
    public GameObject preview;

    public GameObject lightObj;

    public Material fixedMaterial;


    //--- PRIVATE VARIABLES --//

    //Reference to where the first sphere was placed in a drag movement
    private GameObject originSphere;

    //Location of the Origin Sphere
    private Vector3 origin;

    //Location of the Destination Sphere
    private Vector3 dest;

    //Indicates whether the origin sphere has been placed
    private bool originSet;

    //Indicates whether the preview sphere is colliding with an existing sphere
    private bool isColliding;

    //Will be set to the object currently colliding with the preview if any
    private GameObject currCollidingObj;

    //Is the number of points inside mainLine in InitLines.cs
    //Note that this is not the same as the number of points that have been placed. Vectrosity creates two points for all line segments, even if they are the same point
    private static int numLines;

    //Granularity of grid
    //e.g 0.25 means the cube will have four grid lines (including edges) running down each dimension of the cube 
    private float gridGranularity;

    //Vector3 that contains the coordinates to the closest point on the grid system to the gameObject (sphere on right controller)
    //Is set by getClosestPoint() function
    private Vector3 closestPoint;

    private bool createdOrigin;

    private bool lockToAxes;

    private VectorLine dottedLine;
    private VectorLine failLine;

    private bool constraintMode;

    private GameObject freedomPos;

    private GameObject freedomLineObj;
    private GameObject failLineObj;

    private GameObject testSphere;



    // Use this for initialization
    void Start()
    {
        gridGranularity = (float)(1m / 20m);
        originSet = false;
        isColliding = false;

        freedomPos = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        freedomPos.transform.localScale = new Vector3(0.001F, 0.001F, 0.001F);
        Destroy(freedomPos.GetComponent<MeshRenderer>());
        Destroy(freedomPos.GetComponent<Collider>());

        testSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Destroy(testSphere.GetComponent<MeshRenderer>());
        Destroy(testSphere.GetComponent<Collider>());
    }
   
    void Update()
    {
        if(constraintMode)
        {
            constraintModeUpdate();
        }
        else
        {
            normalUpdate();
        }        
    }

    GameObject createPoint()
    {
        GameObject newObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        newObj.transform.position = closestPoint;
        newObj.transform.localScale = gameObject.transform.lossyScale;
        newObj.tag = gameObject.tag;
        Renderer rend = newObj.GetComponent<Renderer>();
        if (rend != null)
        {
            rend.material = gameObject.GetComponent<Renderer>().material;
        }
        newObj.name = "FreedomPoint";
        return newObj;
    }

    void getClosestPoint()
    {
        float tileSize = domain.transform.localScale.x * gridGranularity;
        Vector3 vectorToLoc = gameObject.transform.position - domain.transform.position;
        vectorToLoc = domain.transform.InverseTransformDirection(vectorToLoc);
        Vector3 relativePos = new Vector3();
        relativePos.x = Mathf.Round(vectorToLoc.x / tileSize) * tileSize;
        relativePos.y = Mathf.Round(vectorToLoc.y / tileSize) * tileSize;
        relativePos.z = Mathf.Round(vectorToLoc.z / tileSize) * tileSize;
        relativePos = domain.transform.TransformDirection(relativePos);
        closestPoint = relativePos + domain.transform.position;

        if (lockToAxes && originSet)
        {
            Vector3 localClosestPoint = domain.transform.InverseTransformPoint(closestPoint);
            Vector3 localOriginPoint = domain.transform.InverseTransformPoint(origin);
            Vector3 localDistance = localClosestPoint - localOriginPoint;
            float max = Mathf.Max(Mathf.Abs(localDistance.x), Mathf.Abs(localDistance.y), Mathf.Abs(localDistance.z));
            if (max == Mathf.Abs(localDistance.x))
            {
                localClosestPoint.y = localOriginPoint.y;
                localClosestPoint.z = localOriginPoint.z;
            }
            else if (max == Mathf.Abs(localDistance.y))
            {
                localClosestPoint.x = localOriginPoint.x;
                localClosestPoint.z = localOriginPoint.z;
            }
            else if (max == Mathf.Abs(localDistance.z))
            {
                localClosestPoint.x = localOriginPoint.x;
                localClosestPoint.y = localOriginPoint.y;
            }
            closestPoint = domain.transform.TransformPoint(localClosestPoint);
        }
    }

    private void normalUpdate()
    {
        preview.SetActive(false);
        getClosestPoint();
        isColliding = false;
        //check if our preview is colliding with a placed sphere
        foreach (Transform transform in ((Networking)Networking.GetComponent(typeof(Networking))).allTransformList)
        {
            if (transform.CompareTag("Intermediate"))
            {
                if (transform.position == closestPoint && transform.GetComponent<IntermediateInfo>().GetConnections().Count >= 1)
                {
                    isColliding = true;
                    currCollidingObj = transform.gameObject;
                    transform.GetComponent<IntermediateInfo>().HighlightPoint(true);
                }
                else
                {
                    transform.GetComponent<IntermediateInfo>().HighlightPoint(false);
                }
            }
        }

        if (OVRInput.GetDown(OVRInput.Button.One)) //Places the initial sphere
        {
            if (isColliding)
            {
                originSphere = currCollidingObj;
                createdOrigin = false;

                originSet = true;
                dottedLine = new VectorLine("NewFreedomLine", new List<Vector3>(), dottedTexture, 8.0f);
                dottedLine.points3.Add(originSphere.transform.position);
                dottedLine.points3.Add(originSphere.transform.position);
                dottedLine.textureScale = 1.00f;
                dottedLine.Draw3DAuto();
                freedomLineObj = GameObject.Find("NewFreedomLine");

                failLine = new VectorLine("failLine", new List<Vector3>(), 8.0f);
                failLine.points3.Add(originSphere.transform.position);
                failLine.points3.Add(originSphere.transform.position);
                failLine.Draw3DAuto();
                failLine.SetColor(Color.red);
                failLineObj = GameObject.Find("failLine");
                
            }
        }

        if (OVRInput.Get(OVRInput.Button.One) && originSet)
        {
            origin = originSphere.transform.position;
            dest = transform.position;

            if (origin != Vector3.zero && dest != Vector3.zero)
            {
                dottedLine.points3[0] = dest;
                dottedLine.points3[1] = origin;

                failLine.points3[1] = origin;
                failLine.points3[0] = dest;
            }
            

            if(!InTruncation(originSphere, gameObject))
            {
                freedomLineObj.SetActive(false);
                failLineObj.SetActive(true);
            }
            else
            {
                freedomLineObj.SetActive(true);
                failLineObj.SetActive(false);
            }
        }


        if (OVRInput.GetUp(OVRInput.Button.One) && originSet)
        {
            VectorLine.Destroy(ref failLine);
            originSet = false;
            if (originSphere.GetComponent<IntermediateInfo>().GetFreedomLine() != null || !InTruncation(originSphere, gameObject))
            {
                VectorLine.Destroy(ref dottedLine);
                return;
            }

            GameObject destSphere;
            destSphere = createPoint();
            destSphere.transform.parent = originSphere.transform;
            GameObject freedomLineObj = GameObject.Find("NewFreedomLine");
            freedomLineObj.transform.parent = originSphere.transform;
            freedomLineObj.name = "FreedomLine";

            originSphere.GetComponent<IntermediateInfo>().SetFreedomLine(dottedLine, freedomLineObj);

            SwitchToConstraintMode(originSphere, destSphere);
        }
    }

    private bool InTruncation(GameObject origin, GameObject dest)
    {
        testSphere.transform.position = origin.transform.position;
        testSphere.transform.localScale = origin.GetComponent<IntermediateInfo>().GetScale();
        testSphere.AddComponent<SphereCollider>();
        freedomPos.transform.position = Physics.ClosestPoint(dest.transform.position, testSphere.GetComponent<Collider>(), origin.transform.position, origin.transform.rotation);
        Destroy(testSphere.GetComponent<Collider>());
        List<GameObject> hemisphereList = origin.GetComponent<IntermediateInfo>().GetHemispheres();
        int layerMask = 1 << LayerMask.NameToLayer("hemisphere");
        Collider[] list = Physics.OverlapSphere(freedomPos.transform.position, freedomPos.transform.lossyScale.x, layerMask);
        if (list.Length == hemisphereList.Count)
            return true;
        else
            return false;

    }

    void SwitchToConstraintMode(GameObject intermediatePoint, GameObject target)
    {
        /*
         * 1. Disable switcher
         * 2. Turn Down light
         * 3. Spawn Plane w/ correct material & attach it
         * 4. switch to constraint mode
         * 5. Initialize dottedLine
         */
        LeftRadialMenu.SetActive(false);
        lightObj.SetActive(false);
        Vector3 size = domain.transform.lossyScale;
        size.z = size.z / 10000;
        intermediatePoint.GetComponent<IntermediateInfo>().SpawnPlane(target, size);
        dottedLine = new VectorLine("NewFreedomLine", new List<Vector3>(), dottedTexture, 8.0f);
        dottedLine.points3.Add(originSphere.transform.position);
        dottedLine.points3.Add(originSphere.transform.position);
        ((InitLines)domain.GetComponent(typeof(InitLines))).mainLine.points3.Add(originSphere.transform.position);
        ((InitLines)domain.GetComponent(typeof(InitLines))).mainLine    .points3.Add(originSphere.transform.position);
        dottedLine.textureScale = 1.00f;
        dottedLine.Draw3DAuto();

        constraintMode = true;
    }
        
    private void constraintModeUpdate()
    {
        dottedLine.points3[0] = originSphere.transform.position;
        dottedLine.points3[1] = preview.transform.position;

        ((InitLines)domain.GetComponent(typeof(InitLines))).mainLine.points3[0] = originSphere.transform.position;
        ((InitLines)domain.GetComponent(typeof(InitLines))).mainLine.points3[0] = preview.transform.position;
        preview.SetActive(true);

        GameObject plane = originSphere.GetComponent<IntermediateInfo>().GetPlane();
        preview.transform.localScale = gameObject.transform.lossyScale;
        preview.transform.position = plane.GetComponent<Collider>().ClosestPoint(gameObject.transform.position);

        if(OVRInput.GetDown(OVRInput.Button.One))
        {
            GameObject newFixed = GameObject.Instantiate(preview);
            newFixed.transform.parent = domain.transform;
            newFixed.GetComponent<MeshRenderer>().sharedMaterial = fixedMaterial;
            newFixed.tag = "Fixed";
            newFixed.name = newFixed.tag;
            Networking.GetComponent<Networking>().addToList(newFixed);

            //Line

            domain.GetComponent<InitLines>().lineTransformList.Add(originSphere.transform);
            domain.GetComponent<InitLines>().lineTransformList.Add(newFixed.transform);
            lightObj.SetActive(true);
            plane.SetActive(false);
            VectorLine.Destroy(ref dottedLine);
            dottedLine = null;
            constraintMode = false;            
            //create a new fixed point at the position of the preview
            // Hide Freedom Line and Freedom Point
        }
        /*
         * 1. Have origin sphere already locked where the intermediate point is.
         * In other words, the user should not need to start drawing, there should already be a line going from
         * the origin to the closest point to the user's controller on the plane.
         * 
         * 2. When user presses A, simply place the constraint where the preview is.
         * 
         * 3. Hide freedom line and freedom point after constraint is placed.
         * 
         * 
         * TODO:
         * If constraint is deleted, or another connection is added to this point,
         * get rid of everything (including the invisible freedom line / point.
         * 
         * Modify delete tool to allow deleting spheres not on the grid
         * 
         */
    }
}
