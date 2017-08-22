using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Vectrosity;
using System.Text;

public class DrawForceVector : MonoBehaviour {

    [Tooltip("GameObject containing Camera (CenterEyeAnchor)")]
    public Camera myCamera;
    [Tooltip("List of Transforms that are in identical order as mainLine in order to keep the points updated")]

    public Vector3 forceVectorLH;
    public Vector3 forceVectorRH;


    public Material hemisphereMaterial;

    Text forceText;

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
    [Tooltip("GameObject of Preview Sphere")]
    public GameObject preview;
    [Tooltip("GameObject of Networking")]
    public GameObject Networking;

    public GameObject ForceRadialMenuPanel;

    private bool createdOrigin;

    private bool lockToAxes;
    private Texture2D frontTex;
    private Texture2D lineTex;
    private Texture2D backTex;
    // Use this for initialization
    void Start () {
        forceText = GameObject.Find("Direction_Angles1").GetComponent<Text>();
        preview.SetActive(false); //disable until we need it
        gridGranularity = (float)(1m / 20m);
        originSet = false;
        isColliding = false;
    }
	
	// Update is called once per frame
	void Update () {
        var distToCube = Vector3.Distance(domain.GetComponent<Collider>().ClosestPoint(gameObject.transform.position), gameObject.transform.position);

        getClosestPoint();
        preview.transform.position = closestPoint;
        preview.transform.localScale = gameObject.transform.lossyScale;
        
        isColliding = false;
        preview.SetActive(true);
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
            if (isColliding && (currCollidingObj.CompareTag("Input") || currCollidingObj.CompareTag("Output")))
            {
                originSphere = currCollidingObj;
                createdOrigin = false;
            }
                
            else if(!isColliding)
            {
                originSphere = createPoint();
                createdOrigin = true;
            }
            else
            {
                return;
            }

            originSet = true;
            VectorLine forceLine = new VectorLine("NewForceLine", new List<Vector3>(), 30.0f);
            forceLine.endCap = "Arrow";
            forceLine.Draw3DAuto();
            forceLine.points3.Add(originSphere.transform.position);
            forceLine.points3.Add(originSphere.transform.position);
            forceLine.SetColor(Color.blue);
            domain.GetComponent<InitLines>().forceLineList.Add(forceLine);
            
            numLines = domain.GetComponent<InitLines>().forceLineList.Count;

        }

        if (OVRInput.Get(OVRInput.Button.One) && originSet)
        {
            origin = originSphere.transform.position;
            dest = closestPoint;


            if (origin != Vector3.zero && dest != Vector3.zero)
            {
                domain.GetComponent<InitLines>().forceLineList[numLines - 1].points3[0] = dest;
                domain.GetComponent<InitLines>().forceLineList[numLines - 1].points3[1] = origin;
                Vector3 dest_local = domain.transform.InverseTransformPoint(dest);
                Vector3 origin_local = domain.transform.InverseTransformPoint(origin);
                forceVectorLH = dest_local - origin_local;
                if (forceVectorLH.magnitude > 1)
                {
                    forceVectorLH.Normalize();
                }
                forceVectorRH = new Vector3(forceVectorLH.x, forceVectorLH.y, -forceVectorLH.z);
                forceText.text = forceVectorRH.x.ToString("F4") + "\n" + forceVectorRH.y.ToString("F4") + "\n" + forceVectorRH.z.ToString("F4") + "\n" + forceVectorRH.magnitude.ToString("F4") + "\n"; 

            }
        }

        if (OVRInput.GetUp(OVRInput.Button.One) && originSet)
        {
            originSet = false;
            GameObject destSphere;
            GameObject InputOutputPoint;
            GameObject forcePoint;
            if (createdOrigin && (!isColliding || (isColliding && !(currCollidingObj.CompareTag("Input") || currCollidingObj.CompareTag("Output")))))
            {
                VectorLine vline = domain.GetComponent<InitLines>().forceLineList[numLines - 1];
                VectorLine.Destroy(ref vline);
                domain.GetComponent<InitLines>().forceLineList.RemoveAt(numLines - 1);
                ((Networking)Networking.GetComponent(typeof(Networking))).forceTransformList.Remove(originSphere.transform);
                Destroy(originSphere);
                createdOrigin = false;
                return;
            }
            else if(!createdOrigin && isColliding)
            {
                VectorLine vline = domain.GetComponent<InitLines>().forceLineList[numLines - 1];
                VectorLine.Destroy(ref vline);
                domain.GetComponent<InitLines>().forceLineList.RemoveAt(numLines - 1);
                createdOrigin = false;
                return;
            }
            else if (isColliding && createdOrigin)
            {
                destSphere = currCollidingObj;
                InputOutputPoint = currCollidingObj;
                forcePoint = originSphere;
            }
            else
            {
                destSphere = createPoint();
                InputOutputPoint = originSphere;
                forcePoint = destSphere;
            }

            if(InputOutputPoint.GetComponent<InputOutputInfo>().GetForcePoint() != null) //we already have a forcepoint
            {
                VectorLine vline = domain.GetComponent<InitLines>().forceLineList[numLines - 1];
                VectorLine.Destroy(ref vline);
                domain.GetComponent<InitLines>().forceLineList.RemoveAt(numLines - 1);
                Networking.GetComponent<Networking>().forceTransformList.Remove(forcePoint.transform);
                Destroy(forcePoint);
                createdOrigin = false;
                return;
            }

            GameObject vectorLineObj = GameObject.Find("NewForceLine");
            vectorLineObj.transform.parent = forcePoint.transform;
            vectorLineObj.name = "ForceLine";
            //add to our force vector
            domain.GetComponent<InitLines>().forceVectorList.Add(forceVectorLH);

            //ForcePointInfo
            forcePoint.GetComponent<ForcePointInfo>().SetConnectedPoint(InputOutputPoint);

            //InputOutputInfo
            Vector3 scale = new Vector3(10, 10, 10);
            GameObject hemisphere = Hemisphere.CreateHemisphere(hemisphereMaterial, InputOutputPoint.transform.position, forcePoint.transform.position, !createdOrigin, scale);
            InputOutputPoint.GetComponent<InputOutputInfo>().Setup(InputOutputPoint, forcePoint, hemisphere, forceVectorRH, createdOrigin);
            hemisphere.transform.parent = forcePoint.transform;
            hemisphere.transform.localScale = scale;
            createdOrigin = false;
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
        ((Networking)Networking.GetComponent(typeof(Networking))).forceTransformList.Add(newObj.transform);
        newObj.transform.SetParent(domain.transform, true);
        newObj.AddComponent<ForcePointInfo>();
        newObj.name = "ForcePoint";
        //print(newObj.transform.localPosition);
        //print(newObj.transform.position);
        return newObj;
    }

    override
    public string ToString()
    {
        int forceVectorIndex = 0;
        StringBuilder sb = new StringBuilder();
        foreach (Transform transform in Networking.GetComponent<Networking>().forceTransformList)
        {
            GameObject IOPoint = transform.gameObject.GetComponent<ForcePointInfo>().GetConnectedPoint();
            if (IOPoint.CompareTag("Input"))
            {
                int inputIndex = Networking.GetComponent<Networking>().allTransformList.IndexOf(IOPoint.transform);
                sb.Append(inputIndex + 1);
                //Quaternion temp = new Quaternion();
                Vector3 currVector = domain.GetComponent<InitLines>().forceVectorList[forceVectorIndex++];
                Vector3 finalVector = new Vector3(currVector.x, currVector.y, -currVector.z);
                //temp.x = currVector.x;
                //temp.y = currVector.y;
                //temp.z = currVector.z;

                //temp.w = currVector.magnitude;

                sb.Append(finalVector.ToString("F4")); //+ temp.ToString("!"));
                sb.Append(";");
            }
        }
        return sb.ToString();
    }

    public void ToggleLockToAxes()
    {
        if (lockToAxes)
        {
            lockToAxes = false;
            ColorBlock colors = ForceRadialMenuPanel.transform.GetChild(0).gameObject.GetComponent<Button>().colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = Color.gray;
            ForceRadialMenuPanel.transform.GetChild(0).gameObject.GetComponent<Button>().colors = colors;
        }
        else
        {
            lockToAxes = true;
            ColorBlock colors = ForceRadialMenuPanel.transform.GetChild(0).gameObject.GetComponent<Button>().colors;
            colors.normalColor = Color.red;
            colors.highlightedColor = Color.yellow;
            ForceRadialMenuPanel.transform.GetChild(0).gameObject.GetComponent<Button>().colors = colors;
        }

        
    }
}
    
