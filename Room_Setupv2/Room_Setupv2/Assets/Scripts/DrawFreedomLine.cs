using System.Collections;
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

    private VectorLine dottedLine;
    private VectorLine failLine;

    private bool constraintMode;

    private GameObject freedomPos;

    private GameObject freedomLineObj;
    private GameObject failLineObj;

    private GameObject testSphere;

    private int numConstraints;



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
                    preview.transform.position = transform.position;
                    preview.SetActive(true);
                    transform.GetComponent<IntermediateInfo>().HighlightPoint(true);
                }
                else
                {
                    transform.GetComponent<IntermediateInfo>().HighlightPoint(false);
                }
            }
        }
        if(originSet)
        { 
            Vector3 localPoint = domain.transform.InverseTransformPoint(transform.position);
            Vector3 localOriginPoint = domain.transform.InverseTransformPoint(origin);
            Vector3 localDistance = localPoint - localOriginPoint;
            float max = Mathf.Max(Mathf.Abs(localDistance.x), Mathf.Abs(localDistance.y), Mathf.Abs(localDistance.z));
            if (max == Mathf.Abs(localDistance.x))
            {
                localPoint.y = localOriginPoint.y;
                localPoint.z = localOriginPoint.z;
            }
            else if (max == Mathf.Abs(localDistance.y))
            {
                localPoint.x = localOriginPoint.x;
                localPoint.z = localOriginPoint.z;
            }
            else if (max == Mathf.Abs(localDistance.z))
            {
                localPoint.x = localOriginPoint.x;
                localPoint.y = localOriginPoint.y;
            }
            preview.SetActive(true);
            preview.transform.localScale = transform.lossyScale;
            preview.transform.position = domain.transform.TransformPoint(localPoint);
        }
        if (OVRInput.GetDown(OVRInput.Button.One)) //Places the initial sphere
        {
            if (isColliding)
            {
                originSphere = currCollidingObj;
                origin = originSphere.transform.position;
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
            dest = preview.transform.position;
            originSphere.GetComponent<IntermediateInfo>().HighlightPoint(true);
            if (origin != Vector3.zero && dest != Vector3.zero)
            {
                dottedLine.points3[0] = dest;
                dottedLine.points3[1] = origin;

                failLine.points3[1] = origin;
                failLine.points3[0] = dest;
            }
            

            if(!InTruncation(originSphere, preview))
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
            originSet = false;
            if (originSphere.GetComponent<IntermediateInfo>().GetFreedomLine() != null || !InTruncation(originSphere, preview))
            {
                VectorLine.Destroy(ref dottedLine);
                VectorLine.Destroy(ref failLine);
                return;
            }

            GameObject finalLineObj = GameObject.Find("NewFreedomLine");
            finalLineObj.transform.parent = originSphere.transform;
            finalLineObj.name = "FreedomLine";

            originSphere.GetComponent<IntermediateInfo>().SetFreedomLine(dottedLine, finalLineObj);
            originSphere.GetComponent<IntermediateInfo>().HighlightPoint(false);
            SwitchToConstraintMode(originSphere, preview);
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
        Vector3 size = domain.transform.lossyScale * 2;
        size.z = size.z / 10000;
        intermediatePoint.GetComponent<IntermediateInfo>().SpawnPlane(target, size, domain.transform.up);
        dottedLine = new VectorLine("NewLine", new List<Vector3>(), dottedTexture, 8.0f);
        dottedLine.points3.Add(intermediatePoint.transform.position);
        dottedLine.points3.Add(intermediatePoint.transform.position);
        dottedLine.Draw3DAuto();
        freedomLineObj = GameObject.Find("NewLine");
        constraintMode = true;
        numConstraints = 0;
    }
        
    private void constraintModeUpdate()
    {
        preview.SetActive(true);

        GameObject plane = originSphere.GetComponent<IntermediateInfo>().GetPlane();
        preview.transform.localScale = gameObject.transform.lossyScale;
        preview.transform.position = plane.GetComponent<Collider>().ClosestPoint(gameObject.transform.position);

        //lock the preview positions to the axes
        Vector3 localPoint = domain.transform.InverseTransformPoint(preview.transform.position);
        Vector3 localOriginPoint = domain.transform.InverseTransformPoint(originSphere.transform.position);
        Vector3 localDistance = localPoint - localOriginPoint;
        float max = Mathf.Max(Mathf.Abs(localDistance.x), Mathf.Abs(localDistance.y), Mathf.Abs(localDistance.z));
        if (max == Mathf.Abs(localDistance.x))
        {
            if (localPoint.x < 0)
                localPoint.x = -0.5F;
            else
                localPoint.x = 0.5F;
            localPoint.y = localOriginPoint.y;
            localPoint.z = localOriginPoint.z;
        }
        else if (max == Mathf.Abs(localDistance.y))
        {
            if (localPoint.y < 0)
                localPoint.y = -0.5F;
            else
                localPoint.y = 0.5F;
            localPoint.x = localOriginPoint.x;
            localPoint.z = localOriginPoint.z;
        }
        else if (max == Mathf.Abs(localDistance.z))
        {
            if (localPoint.z < 0)
                localPoint.z = -0.5F;
            else
                localPoint.z = 0.5F;
            localPoint.x = localOriginPoint.x;
            localPoint.y = localOriginPoint.y;
        }
        preview.transform.localScale = transform.lossyScale;
        preview.transform.position = domain.transform.TransformPoint(localPoint);

        Vector3 localCoord = domain.transform.InverseTransformPoint(preview.transform.position);
        localCoord.x += 0.5F;
        localCoord.y += 0.5F;
        localCoord.z += 0.5F;
        freedomLineObj.SetActive(true);
        // preview.transform.position = plane.GetComponent<Collider>().ClosestPoint(preview.transform.position);
        dottedLine.textureScale = 1.00f;
        dottedLine.points3[0] = originSphere.transform.position;
        dottedLine.points3[1] = preview.transform.position;
       


        if (OVRInput.GetDown(OVRInput.Button.One) && checkCoords(localCoord))
        {
            numConstraints++;
            GameObject newFixed = GameObject.Instantiate(preview);
            newFixed.AddComponent<SphereCollider>();
            newFixed.transform.parent = domain.transform;
            newFixed.GetComponent<MeshRenderer>().sharedMaterial = fixedMaterial;
            newFixed.tag = "Fixed";
            newFixed.name = newFixed.tag;
            newFixed.layer = LayerMask.NameToLayer("point");
            Networking.GetComponent<Networking>().addToList(newFixed);

            //Line
            ((InitLines)domain.GetComponent(typeof(InitLines))).mainLine.points3.Add(originSphere.transform.position);
            ((InitLines)domain.GetComponent(typeof(InitLines))).mainLine.points3.Add(newFixed.transform.position);

            domain.GetComponent<InitLines>().lineTransformList.Add(originSphere.transform);
            domain.GetComponent<InitLines>().lineTransformList.Add(newFixed.transform);

            if (numConstraints == 2)
                switchToRegularUpdateMode(plane);
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
         * Modify delete tool to allow deleting spheres not on the grid
         * 
         */
    }

    private void switchToRegularUpdateMode(GameObject plane)
    {
        LeftRadialMenu.SetActive(true);
        lightObj.SetActive(true);
        plane.SetActive(false);
        VectorLine.Destroy(ref dottedLine);
        VectorLine.Destroy(ref failLine);
        freedomLineObj = null;
        failLineObj = null;
        dottedLine = null;

        constraintMode = false;

    }

    private bool checkCoords(Vector3 localCoord)
    {
        if(localCoord.x < 0 || localCoord.x > 1 ||
            localCoord.y < 0 || localCoord.y > 1 ||
            localCoord.z < 0 || localCoord.z > 1)
        {
            return false;
        }
        else
        {
            return true;
        }
    }


}

