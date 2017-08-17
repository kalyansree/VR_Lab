using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Vectrosity;
using System.Text;

public class DrawFreedomLine : MonoBehaviour
{

    [Tooltip("GameObject containing Camera (CenterEyeAnchor)")]
    public Camera myCamera;    

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

    //--- PUBLIC VARIABLES ---//
    [Tooltip("GameObject of the Domain Cube")]
    public GameObject domain;

    [Tooltip("GameObject of Right Controller")]
    public GameObject RightController;
    [Tooltip("GameObject of Networking")]
    public GameObject Networking;

    public Texture2D dottedTexture;

    public GameObject RadialMenu;

    private bool createdOrigin;

    private bool lockToAxes;

    private VectorLine dottedLine;

    private bool constraintMode;
    // Use this for initialization
    void Start()
    {
        gridGranularity = (float)(1m / 20m);
        originSet = false;
        isColliding = false;
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

    void SwitchToConstraintMode(GameObject intermediatePoint, GameObject target)
    {
        /*
         * 1. Disable switcher
         * 2. Spawn Plane w/ correct material & attach it
         * 3. switch to constraint mode
         */
        RadialMenu.SetActive(false);
        Vector3 size = domain.transform.lossyScale;
        size.z = size.z / 10000;
        intermediatePoint.GetComponent<IntermediateInfo>().SpawnPlane(target, size);
        constraintMode = true;
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
                dottedLine = new VectorLine("NewFreedomLine", new List<Vector3>(), dottedTexture, 16.0f);
                dottedLine.points3.Add(originSphere.transform.position);
                dottedLine.points3.Add(originSphere.transform.position);
                dottedLine.textureScale = 1.00f;
                dottedLine.Draw3DAuto();
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
            }

            //TODO: Only show line when it is in the valid area
        }


        if (OVRInput.GetUp(OVRInput.Button.One) && originSet)
        {
            originSet = false;
            if (originSphere.GetComponent<IntermediateInfo>().GetFreedomLine() != null)
            {
                VectorLine.Destroy(ref dottedLine);
                return;
            }

            //TODO: Check that point is in a valid area

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

    private void constraintModeUpdate()
    {
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

