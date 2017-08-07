using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputOutputInfo : MonoBehaviour {
    private GameObject origin;
    private GameObject force;
    private GameObject hemisphere;

    private Vector3 forceVector;

    private List<Vector3> incomingJoints;
    private bool directionTowardsOrigin;

    private bool viability;
    void Start()
    {
        incomingJoints = new List<Vector3>();
    }
    public bool Setup(GameObject originPoint, GameObject forcePoint, GameObject hemisphereObj, Vector3 forceVector, bool directionTowardsOrigin)
    {
        this.origin = originPoint;
        this.force = forcePoint;
        this.hemisphere = hemisphereObj;
        this.forceVector = forceVector;
        this.directionTowardsOrigin = directionTowardsOrigin;

        return checkViability();
    }

    public void SetOriginPoint(GameObject originPoint)
    {
        this.origin = originPoint;
    }

    public void SetForcePoint(GameObject forcePoint)
    {
        this.force = forcePoint;
    }

    public void SetHemisphereObj(GameObject hemisphereObj)
    {
        this.hemisphere = hemisphereObj;
    }

    public bool SetForceVector(Vector3 forceVector)
    {
        this.forceVector = forceVector;
        return checkViability();

    }
    public GameObject GetOriginPoint()
    {
        return origin;
    }

    public GameObject GetForcePoint()
    {
        return force;
    }

    public GameObject GetHemisphereObj()
    {
        return hemisphere;
    }

    public bool checkViability()
    {
        return true;


    }

    public void toggleHemisphereView()
    {
        if (hemisphere.activeSelf)
            hemisphere.SetActive(false);
        else
            hemisphere.SetActive(true);
    }


}
