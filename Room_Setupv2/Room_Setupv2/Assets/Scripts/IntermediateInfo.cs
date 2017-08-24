using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vectrosity;
using Parabox.CSG;

public class IntermediateInfo : MonoBehaviour {

    private GameObject intermediatePoint;

    private List<GameObject> connectionList;

    private List<GameObject> hemisphereList;

    private GameObject truncatedHemisphere;

    private Material hemisphereMaterial1;
    private Material truncatedHemisphereMaterial;
    private Material planeMaterial;

    private bool hemisphereVisible;
    private bool truncationExists;

    private bool viability;

    private GameObject domain;

    private GameObject plane;
    private GameObject freedomLineVectorObj;
    private VectorLine freedomLineVector;

    public void SetupMaterials(Material hemisphereMat1, Material truncatedHemisphereMat, Material planeMat)
    {
        this.hemisphereMaterial1 = hemisphereMat1;
        this.truncatedHemisphereMaterial = truncatedHemisphereMat;
        this.planeMaterial = planeMat;
    }

    public void SetObjs(GameObject intermediatePoint, GameObject domain)
    {
        connectionList = new List<GameObject>();
        hemisphereList = new List<GameObject>();
        this.intermediatePoint = intermediatePoint;
        this.domain = domain;
        freedomLineVectorObj = null;
        freedomLineVector = null;
        hemisphereVisible = true;
        truncationExists = false;

        plane = null;
    }

    public List<GameObject> GetConnections()
    {
        return connectionList;
    }

    public List<GameObject> GetHemispheres()
    {
        return hemisphereList;
    }

    public VectorLine GetFreedomLine()
    {
        return freedomLineVector;
    }

    public GameObject GetFreedomLinePObj()
    {
        return freedomLineVectorObj;
    }

    public float GetScaleAsFloat()
    {
        if (truncationExists)
        {
            return hemisphereList[0].transform.lossyScale.x;
        }
        else
            return 0;
    }

    public Vector3 GetScale()
    {
        if (truncationExists)
        {
            return hemisphereList[0].transform.lossyScale;
        }
        else
            return new Vector3(0, 0, 0);
    }

    public GameObject GetPlane()
    {
        return plane;
    }


    public void SetFreedomLine(VectorLine freedomLine, GameObject freedomLineObj)
    {
        this.freedomLineVector = freedomLine;
        this.freedomLineVectorObj = freedomLineObj;
    }

    public void removeFreedomLineAndPlane()
    {
        VectorLine.Destroy(ref freedomLineVector);
        GameObject.Destroy(plane);

        freedomLineVector = null;
        freedomLineVectorObj = null;
        plane = null;
    }
    public bool addConnection(GameObject connection, bool drawnTowardConnection)
    {
        /*
         * TODO:
         * Remove freedomLine and plane if a new connection is added when a constraint already exists
         */ 
        if (connectionList.Contains(connection))
            return false;

        //first, generate a hemisphere
        bool dirTowardConnection = false;
        if(connection.CompareTag("Input") || connection.CompareTag("Output"))
        {
            dirTowardConnection = calculateForceDirection(this.intermediatePoint, connection);
        }
        else
        {
            dirTowardConnection = drawnTowardConnection;
        }

        Vector3 scale = new Vector3(5, 5, 5);
        GameObject hemisphere = Hemisphere.CreateHemisphere(hemisphereMaterial1, intermediatePoint.transform.position, connection.transform.position, dirTowardConnection, scale);
        connectionList.Add(connection);
        hemisphereList.Add(hemisphere);
        hemisphere.transform.parent = intermediatePoint.transform;
        hemisphere.transform.localScale = scale;
        truncatedHemisphere = truncate(hemisphere, truncatedHemisphere);
        
        return true;

    }

    public bool removeConnection(GameObject connection)
    {
        if (!connectionList.Contains(connection))
        {
            return false;
        }
        else
        {
            int connectionIndex = connectionList.IndexOf(connection);
            Destroy(truncatedHemisphere);
            Destroy(hemisphereList[connectionIndex]);
            connectionList.RemoveAt(connectionIndex);
            hemisphereList.RemoveAt(connectionIndex);
            recalculateTruncation();
            return true;
        }
    }

    public void toggleHemisphereView(bool visible)
    {
        if (truncationExists)
        {
            hemisphereVisible = visible;
            truncatedHemisphere.GetComponent<MeshRenderer>().enabled = visible;
            if(freedomLineVectorObj != null)
                freedomLineVectorObj.SetActive(visible);
        }
    }
    // Adds connection to the vertex and generates the truncated hemisphere
    private GameObject truncate(GameObject newHemisphere, GameObject existingTruncation)
    {
        if(newHemisphere == null)
        {
            return null;
        }
        if (connectionList.Count == 1) //this is the only connection, nothing to do
        {
            truncationExists = true;
            viability = false;
            return newHemisphere;
        }
        float scaling = 1;
        if (domain.GetComponent<ResizeObject>().getMagnification() >= 1)
        {
            scaling = 10;
        }
        else
        {
            scaling = 20;
        }
        domain.transform.localScale = domain.transform.localScale * scaling;
        GameObject truncation = Hemisphere.GetIntersection(existingTruncation, newHemisphere, truncatedHemisphereMaterial, true);
        
        if (truncation == null) //means there was no ove rlapping region
        {
            viability = false;
            truncationExists = false;
            domain.transform.localScale = domain.transform.localScale / scaling;
            return truncation;
        }
        else
        {
            viability = true;
            truncationExists = true;
            truncation.transform.parent = newHemisphere.transform.parent;
        }

        domain.transform.localScale = domain.transform.localScale / scaling;
        return truncation;
    }

    private void recalculateTruncation()
    {
        if(connectionList.Count == 0)
        {
            truncatedHemisphere = null;
            viability = false;
            truncationExists = false;
        }
        else if(connectionList.Count == 1)
        {
            truncatedHemisphere = hemisphereList[0];
            truncatedHemisphere.GetComponent<MeshRenderer>().enabled = true;
            viability = false;
            truncationExists = true;
        }
        else
        {
            GameObject truncation = hemisphereList[0];
            for(int i = 1; i < hemisphereList.Count; i++)
            {
                truncation = truncate(hemisphereList[i], truncation);
            }
            truncatedHemisphere = truncation;
            truncatedHemisphere.GetComponent<MeshRenderer>().enabled = true;
        }
    }

    //Helper function that calculates whether the direction of the force is towards the I/O point or not
    private bool calculateForceDirection(GameObject intermediatePoint, GameObject IOPoint)
    {
        GameObject forcePoint = IOPoint.GetComponent<InputOutputInfo>().GetForcePoint();
        bool directionTowardOrigin = IOPoint.GetComponent<InputOutputInfo>().GetDirection();
        Vector3 forceVector;
        if(directionTowardOrigin)
        {
            forceVector = IOPoint.transform.position - forcePoint.transform.position;
        }
        else
        {
            forceVector = forcePoint.transform.position - IOPoint.transform.position;
        }
        Vector3 towardIO = IOPoint.transform.position - intermediatePoint.transform.position;
        Vector3 awayFromIO = intermediatePoint.transform.position - IOPoint.transform.position;

        float angleA = Vector3.Angle(forceVector, towardIO);
        float angleB = Vector3.Angle(forceVector, awayFromIO);

        if(angleA > angleB)
        {
            return false;
        }
        else
        {
            return true;
        }

    }

    public void SpawnPlane(GameObject target, Vector3 size)
    {
        plane = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //new Vector3(10, 10, 0.001F)
        plane.transform.localScale = size;
        plane.transform.rotation = Quaternion.LookRotation(target.transform.position - intermediatePoint.transform.position);
        plane.transform.position = intermediatePoint.transform.position;
        plane.GetComponent<MeshRenderer>().sharedMaterial = planeMaterial;
        plane.transform.parent = intermediatePoint.transform;
    }

    public void HighlightPoint(bool highlight)
    {
        if (!hemisphereVisible)
            return;
        float alpha = 0;
        if (highlight)
            alpha = 1;
        else
            alpha = 0.353F;


        Color color = intermediatePoint.GetComponent<Renderer>().material.color;
        color.a = alpha;
        intermediatePoint.GetComponent<Renderer>().material.color = color;

        if(truncationExists)
        {
            color = truncatedHemisphere.GetComponent<Renderer>().material.color;
            color.a = alpha;
            truncatedHemisphere.GetComponent<Renderer>().material.color = color;
        }
    }
}
