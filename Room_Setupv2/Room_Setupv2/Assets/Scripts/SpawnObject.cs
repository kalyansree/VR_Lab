using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vectrosity;

public class SpawnObject : MonoBehaviour { 
    private GameObject originSphere;
    private Vector3 origin;
    private Vector3 dest;
    private bool originSet = false;
    private bool isColliding = false; //whether the hand is close to another point that has been placed
    private int numPoints = 0;
    private GameObject currCollidingObj; 
    
    [Tooltip("If true, will allow user to drag to create a line between two points. If false, will only place point at origin point (Should be false for Input and Output type points.")]
    public bool allowDrag;
    [Tooltip("GameObject of the Domain Cube")]
    public GameObject domain;

    [Tooltip("Determines whether this game object should be restricted to the boundary of the domain or not")]
    public bool restrictToBoundary;

    [Tooltip("Granularity of grid, where 1 is where the whole cube is one grid")]
    private float gridGranularity = 0.01F;

    [Tooltip("ArrayList that contains the coordinates to the closest point to place on the domain")]
    private ArrayList closestPoint;

    void Start()
    {
        closestPoint = new ArrayList();
        closestPoint.Capacity = 3;
    }

    void Update()
    {
        var distToCube = Vector3.Distance(domain.GetComponent<Collider>().ClosestPoint(gameObject.transform.position), gameObject.transform.position);
        if (OVRInput.GetDown(OVRInput.Button.One) && distToCube == 0) //Places the initial sphere
        {
            if (isColliding && allowDrag)
            {
                originSphere = currCollidingObj;
                originSet = true;
                ((InitLines)domain.GetComponent(typeof(InitLines))).mainLine.points3.Add(originSphere.transform.position);
                ((InitLines)domain.GetComponent(typeof(InitLines))).mainLine.points3.Add(originSphere.transform.position);
                numPoints = ((InitLines)domain.GetComponent(typeof(InitLines))).mainLine.points3.Count;
            }
            else if(!allowDrag)
            {
                originSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                originSphere.transform.position = gameObject.transform.position;
                originSphere.transform.rotation = gameObject.transform.rotation;
                originSphere.transform.localScale = gameObject.transform.lossyScale;
                originSphere.tag = "Point";
                Renderer rend = originSphere.GetComponent<Renderer>();
                if (rend != null)
                {
                    rend.material = gameObject.GetComponent<Renderer>().material;
                }
                originSphere.transform.SetParent(domain.transform, true);
            }
        }
        if (OVRInput.Get(OVRInput.Button.One) && originSet && allowDrag)
        {
            origin = originSphere.transform.position;
            dest = gameObject.transform.position;
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
            if (isColliding)
            {
                destSphere = currCollidingObj;
            }
            else
            {
                destSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                destSphere.transform.position = gameObject.transform.position;
                destSphere.transform.rotation = gameObject.transform.rotation;
                destSphere.tag = "Point";
                destSphere.transform.localScale = gameObject.transform.lossyScale;
                Renderer rend = destSphere.GetComponent<Renderer>();
                if (rend != null)
                {
                    rend.material = gameObject.GetComponent<Renderer>().material;
                }
                destSphere.transform.SetParent(domain.transform, true);
            }

            //add to our list of line coordinates
            domain.GetComponent<InitLines>().transformList.Add(originSphere.transform);
            domain.GetComponent<InitLines>().transformList.Add(destSphere.transform);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Point") && allowDrag) //make sure it is a placed point
        {
            isColliding = true;
            currCollidingObj = other.gameObject;
            Color color = ((Renderer)other.gameObject.GetComponent<Renderer>()).material.color;
            color.a = 1;
            ((Renderer)other.gameObject.GetComponent<Renderer>()).material.color = color;
        }
        
            
    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Point") && allowDrag) //make sure it is a placed point
        {
            isColliding = false;
            Color color = ((Renderer)other.gameObject.GetComponent<Renderer>()).material.color;
            color.a = 0.353F;
            ((Renderer)other.gameObject.GetComponent<Renderer>()).material.color = color;
        }
    }

    void getClosestPoint()
    {

        //TODO
        gameObject.transform.position;
    }
}
