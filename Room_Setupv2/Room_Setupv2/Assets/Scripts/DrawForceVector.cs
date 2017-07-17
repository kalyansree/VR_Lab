using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Vectrosity;

public class DrawForceVector : MonoBehaviour {
    public VectorLine forceLine;
    public Texture2D frontTex;
    public Texture2D lineTex;
    public Texture2D backTex;
    [Tooltip("GameObject containing Camera (CenterEyeAnchor)")]
    public Camera myCamera;
    [Tooltip("List of Transforms that are in identical order as mainLine in order to keep the points updated")]
    public List<Transform> lineTransformList;



    public Vector3 dir_angles;

    Text text1;
    //Text text2;




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
    private static int numPoints;

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
    [Tooltip("GameObject of Preview Sphere")]
    public GameObject preview;
    [Tooltip("GameObject of Networking")]
    public GameObject Networking;

    // Use this for initialization
    void Start () {
        VectorLine.SetCamera3D(myCamera);
        forceLine = new VectorLine("ForceLine", new List<Vector3>(), 30.0f);
        VectorLine.SetEndCap("Arrow", EndCap.Both, -1.0F,lineTex, frontTex, backTex);
        forceLine.endCap = "Arrow";
        forceLine.Draw3DAuto();



        //dir_angles = forceLine.transform.localRotation.eulerAngles;

        //text = GameObject.Find("Next Text 0").GetComponent<Text>();

        text1 = GameObject.Find("Direction_Angles1").GetComponent<Text>();




        preview.SetActive(false); //disable until we need it
        gridGranularity = (float)(1m / 20m);
        originSet = false;
        isColliding = false;

        lineTransformList = new List<Transform>();
    }
	
	// Update is called once per frame
	void Update () {
        var distToCube = Vector3.Distance(domain.GetComponent<Collider>().ClosestPoint(gameObject.transform.position), gameObject.transform.position);

        getClosestPoint();
        preview.transform.position = closestPoint;
        preview.transform.localScale = gameObject.transform.lossyScale;
        
        isColliding = false;
        //check if our preview is colliding with a placed sphere
        foreach (Transform transform in ((Networking)Networking.GetComponent(typeof(Networking))).allTransformList)
        {
            //print(dist);
            if (transform.position == closestPoint)
            {
                preview.SetActive(false);
                isColliding = true;
                currCollidingObj = transform.gameObject;
                Color color = ((Renderer)transform.gameObject.GetComponent<Renderer>()).material.color;
                color.a = 1;
                ((Renderer)transform.gameObject.GetComponent<Renderer>()).material.color = color;
            }
            else
            {
                Color color = ((Renderer)transform.gameObject.GetComponent<Renderer>()).material.color;
                color.a = 0.353F;
                ((Renderer)transform.gameObject.GetComponent<Renderer>()).material.color = color;
            }
        }

        if (OVRInput.GetDown(OVRInput.Button.One)) //Places the initial sphere
        {
            if (isColliding)
                originSphere = currCollidingObj;
            else
                originSphere = createPoint();
            originSet = true;
            forceLine.points3.Add(originSphere.transform.position);
            forceLine.points3.Add(originSphere.transform.position);
            forceLine.SetColor(Color.red);
            numPoints = forceLine.points3.Count;

        }

        if (OVRInput.Get(OVRInput.Button.One) && originSet)
        {
            origin = originSphere.transform.position;
            dest = closestPoint;
            if (origin != Vector3.zero && dest != Vector3.zero)
            {
                forceLine.points3[numPoints - 2] = dest;
                forceLine.points3[numPoints - 1] = origin;
                Vector3 dest_local = domain.transform.InverseTransformPoint(dest);
                Vector3 origin_local = domain.transform.InverseTransformPoint(origin);
                dir_angles = dest_local - origin_local;
            }
        }

        if (OVRInput.GetUp(OVRInput.Button.One) && originSet)
        {
            originSet = false;
            GameObject destSphere;
            if (isColliding)
            {
                destSphere = currCollidingObj;
            }
            else
            {
                destSphere = createPoint();
            }

            //add to our list of line coordinates
            lineTransformList.Add(destSphere.transform);
            lineTransformList.Add(originSphere.transform);
        }



        //dir_angles = forceLine.transform.localRotation.eulerAngles;
        if(dir_angles.magnitude > 1)
        {
            dir_angles.Normalize();
        }
        text1.text = dir_angles.magnitude + "\n" + dir_angles.x + "\n" + dir_angles.y + "\n" + dir_angles.z + "\n" ;




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
        ((Networking)Networking.GetComponent(typeof(Networking))).allTransformList.Add(newObj.transform);
        newObj.transform.SetParent(domain.transform, true);
        //print(newObj.transform.localPosition);
        //print(newObj.transform.position);
        return newObj;
    }

    void LateUpdate()
    {
        int i = 0;
        foreach (Transform transform in lineTransformList)
        {
                forceLine.points3[i] = transform.position;
                i++;
        }
    }
}
