using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputOutputInfo : MonoBehaviour {
    private GameObject origin;
    private GameObject force;
    private GameObject hemisphere;

    private Vector3 forceVector;

    private List<GameObject> connectedPoints;
    private bool directionTowardsOrigin;

    private bool viability;
    void Start()
    {
        connectedPoints = new List<GameObject>();
    }
    public bool Setup(GameObject originPoint, GameObject forcePoint, GameObject hemisphereObj, Vector3 forceVector, bool directionTowardsOrigin)
    {
        this.origin = originPoint;
        this.force = forcePoint;
        this.hemisphere = hemisphereObj;
        this.forceVector = forceVector;
        this.directionTowardsOrigin = directionTowardsOrigin;

        return checkFeasibility();
    }

    public void SetOriginPoint(GameObject originPoint)
    {
        this.origin = originPoint;
    }

    public void SetForcePoint(GameObject forcePoint)
    {
        this.force = forcePoint;
    }

    public void DeleteForcePoint()
    {
        this.force = null;
        this.forceVector = new Vector3(-1, -1, -1);
        this.hemisphere = null;
    }

    public bool SetForceVector(Vector3 forceVector)
    {
        this.forceVector = forceVector;
        return checkFeasibility();

    }
    public GameObject GetOriginPoint()
    {
        return origin;
    }

    public GameObject GetForcePoint()
    {
        return force;
    }

    public Vector3 GetForceVector()
    {
        return forceVector;
    }

    public GameObject GetHemisphereObj()
    {
        return hemisphere;
    }

    public bool GetDirection()
    {
        return directionTowardsOrigin;
    }
    public bool checkFeasibility()
    {
        if(forceVector == new Vector3(-1, -1, -1) || origin == null)
        {
            return false;
        }
        if (origin.CompareTag("Output"))
        {
            foreach (GameObject obj in connectedPoints)
            {
                Vector3 direction;
                if (directionTowardsOrigin)
                {
                    direction = obj.transform.position - origin.transform.position;
                }
                else
                {
                    direction = origin.transform.position - obj.transform.position;
                }
                
                if (checkViability(forceVector, direction) == false)
                {
                    return false;
                }
            }
            
        }
        return true;
    }

    public void toggleHemisphereView(bool visible)
    {
        if(hemisphere != null)
            hemisphere.GetComponent<MeshRenderer>().enabled = visible;
    }

    public void addConnection(GameObject vertex)
    {
        connectedPoints.Add(vertex);
    }

    public void removeConnection(GameObject vertex)
    {
        connectedPoints.Remove(vertex);
    }

    private bool checkViability(Vector3 forceVector, Vector3 vertexVector)
    {
        if (Vector3.Angle(forceVector, vertexVector) > 90)
            return false;
        else
            return true;
    }


}
