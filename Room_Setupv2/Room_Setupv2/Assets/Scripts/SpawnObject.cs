using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vectrosity;
using UnityEngine.UI;
/*
 * This script is applied to all of the different types of Sphere objects attached to the right controller.
 * It controls how and when the spheres are allowed to be placed.
 * Note that each type of sphere has its own instance of this class, so don't assume that all spheres are looking at the same instances of these variables/objects. 
 * Use static variables if all spheres need to see the same object or value. 
 */
public class SpawnObject : MonoBehaviour {

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

    //Indicates whether the sphere is allowed to be placed. 
    //Not 100% reliable, should always double check conditions for placing.
    private bool allowPlacing;

    //--- PUBLIC VARIABLES ---//
    [Tooltip("If true, will allow user to drag to create a line between two points. If false, will only place point at origin point (Should be false for Input and Output type points.")]
    public bool allowDrag;

    [Tooltip("Determines whether this sphere should be restricted to the boundary of the domain or not")]
    public bool restrictToBoundary;

    [Tooltip("Determines whether this sphere should be restricted to inside the domain or not")]
    public bool restrictToInside;

    [Tooltip("GameObject of the Domain Cube")]
    public GameObject domain;

    [Tooltip("GameObject of Right Controller")]
    public GameObject RightController;
    [Tooltip("GameObject of Preview Sphere")]
    public GameObject preview;
    [Tooltip("GameObject of Networking")]
    public GameObject Networking;


    public Text coordText;
    void Start()
    {
        preview.SetActive(false); //disable until we need it
        gridGranularity = (float)(1m / 20m);
        originSet = false;
        isColliding = false;
        allowPlacing = false;
    }
    void Update()
    {
        var distToCube = Vector3.Distance(domain.GetComponent<Collider>().ClosestPoint(gameObject.transform.position), gameObject.transform.position);

        if (distToCube < 0.1)
        {
            getClosestPoint();
            preview.transform.position = closestPoint;
            preview.transform.localScale = gameObject.transform.lossyScale;
            Vector3 pos = domain.transform.InverseTransformPoint(preview.transform.position);
            double xPos = System.Math.Round(pos.x, 3);
            double yPos = System.Math.Round(pos.y, 3);
            double zPos = System.Math.Round(pos.z, 3);

            bool onBoundary = xPos == -0.5F || yPos == -0.5F || zPos == -0.5F || xPos == 0.5F || yPos == 0.5F || zPos == 0.5F;

            if ((restrictToInside && onBoundary) || (restrictToBoundary && !onBoundary))
            {
                preview.SetActive(false);
                allowPlacing = false;
            }
            else
            {
                preview.SetActive(true);
                allowPlacing = true;
            }
        }
        else
        {
            preview.SetActive(false);
            allowPlacing = false;
        }
        isColliding = false;
        //check if our preview is colliding with a placed sphere
        foreach (Transform transform in ((Networking)Networking.GetComponent(typeof(Networking))).allTransformList)
        {
            //print(dist);
            if (transform.position == closestPoint && distToCube < 0.1)
            {
                preview.SetActive(false);
                isColliding = true;
                currCollidingObj = transform.gameObject;
                if (allowDrag)
                {
                    Color color = ((Renderer)transform.gameObject.GetComponent<Renderer>()).material.color;
                    color.a = 1;
                    ((Renderer)transform.gameObject.GetComponent<Renderer>()).material.color = color;
                    allowPlacing = true;
                }
                else
                {
                    allowPlacing = false; //we dont want to allow placing if there is already a point there and we are not allowed to drag
                }
            }
            else
            {
                Color color = ((Renderer)transform.gameObject.GetComponent<Renderer>()).material.color;
                color.a = 0.353F;
                ((Renderer)transform.gameObject.GetComponent<Renderer>()).material.color = color;
            }
        }

        if (OVRInput.GetDown(OVRInput.Button.One) && allowPlacing) //Places the initial sphere
        {

            if (allowDrag)
            {
                if (isColliding)
                    originSphere = currCollidingObj;
                else
                    originSphere = createPoint();
                originSet = true;
                ((InitLines)domain.GetComponent(typeof(InitLines))).mainLine.points3.Add(originSphere.transform.position);
                ((InitLines)domain.GetComponent(typeof(InitLines))).mainLine.points3.Add(originSphere.transform.position);
                numPoints = ((InitLines)domain.GetComponent(typeof(InitLines))).mainLine.points3.Count;

            }
            else
            {
                originSphere = createPoint();
            }
        }
        if (OVRInput.Get(OVRInput.Button.One) && originSet && allowDrag)
        {
            origin = originSphere.transform.position;
            //if we're not allowed to place, just show the line drawing towards the player's sphere, not the preview sphere
            if (!allowPlacing)
                dest = gameObject.transform.position;
            else
                dest = closestPoint;
            if (origin != Vector3.zero && dest != Vector3.zero)
            {
                ((InitLines)domain.GetComponent(typeof(InitLines))).mainLine.points3[numPoints - 2] = origin;
                ((InitLines)domain.GetComponent(typeof(InitLines))).mainLine.points3[numPoints - 1] = dest;
            }
        }

        if (OVRInput.GetUp(OVRInput.Button.One) && originSet && allowDrag)
        {
            originSet = false;
            GameObject destSphere;
            if (isColliding && origin != dest)
            {
                destSphere = currCollidingObj;
            }
            else
            {
                if (!allowPlacing || origin == dest) //if we aren't allowed to place, we shouldn't
                {
                    ((InitLines)domain.GetComponent(typeof(InitLines))).mainLine.points3.RemoveAt(--numPoints);
                    ((InitLines)domain.GetComponent(typeof(InitLines))).mainLine.points3.RemoveAt(--numPoints);
                    return;
                }
                else
                {
                    destSphere = createPoint();
                }
            }

            //add to our list of line coordinates
            domain.GetComponent<InitLines>().lineTransformList.Add(originSphere.transform);
            domain.GetComponent<InitLines>().lineTransformList.Add(destSphere.transform);

            if(originSphere.CompareTag("Input") || originSphere.CompareTag("Output"))
            {
                originSphere.GetComponent<InputOutputInfo>().addConnection(destSphere);
            }
            if (destSphere.CompareTag("Input") || destSphere.CompareTag("Output"))
            {
                destSphere.GetComponent<InputOutputInfo>().addConnection(originSphere);
            }
        }
    }

    private void getClosestPoint()
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
        if (Vector3.Distance(domain.GetComponent<Collider>().ClosestPoint(closestPoint), closestPoint) > 0) // only triggers when the point is outside the collider
        {
            closestPoint = domain.GetComponent<Collider>().ClosestPoint(closestPoint);
        }
    }

    private GameObject createPoint()
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
        ((Networking)Networking.GetComponent(typeof(Networking))).addToList(newObj);
        newObj.transform.SetParent(domain.transform, true);
        //print(newObj.transform.localPosition);
        //print(newObj.transform.position);
        if(newObj.CompareTag("Input") || newObj.CompareTag("Output"))
        {
            newObj.AddComponent<InputOutputInfo>();
        }
        return newObj;
    }
}
