using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntermediateInfo : MonoBehaviour {

    private GameObject intermediatePoint;

    private List<GameObject> connectionList;

    private List<GameObject> hemisphereList;

    private GameObject truncatedHemisphere;

    private Material hemisphereMaterial1;
    private Material hemisphereMaterial2;
    private Material truncatedHemisphereMaterial;

    private bool flipDirection1;
    private bool flipDirection2;

    private bool hemisphereVisible;
    private bool truncationExists;

    private bool viability;

    private GameObject domain;


    public void SetupMaterials(Material hemisphereMat1, Material hemisphereMat2, Material truncatedHemisphereMat)
    {
        this.hemisphereMaterial1 = hemisphereMat1;
        this.hemisphereMaterial2 = hemisphereMat2;
        this.truncatedHemisphereMaterial = truncatedHemisphereMat;
    }

    public void SetPoint(GameObject intermediatePoint)
    {
        connectionList = new List<GameObject>();
        hemisphereList = new List<GameObject>();
        this.intermediatePoint = intermediatePoint;
    }

    public void SetDomain(GameObject obj)
    {
        this.domain = obj;
    }
    public bool addConnection(GameObject connection, bool drawnTowardConnection)
    {
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
            connectionList.RemoveAt(connectionIndex);
            hemisphereList.RemoveAt(connectionIndex);
            Destroy(truncatedHemisphere);
            recalculateTruncation();
            return true;
        }
    }

    public List<GameObject> GetConnections()
    {
        return connectionList;
    }

    public void toggleHemisphereView()
    {
        if (hemisphereVisible)
        {
            foreach (GameObject hemiObj in hemisphereList)
            {
                hemiObj.SetActive(false);
            }
            hemisphereVisible = false;
        }
        else
        {
            foreach (GameObject hemiObj in hemisphereList)
            {
                hemiObj.SetActive(true);
            }
            hemisphereVisible = true;
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
        float scaling = 10;
        domain.transform.localScale = domain.transform.localScale * scaling;
        GameObject truncation = Hemisphere.GetIntersection(existingTruncation, newHemisphere, truncatedHemisphereMaterial, true);
        
        if (truncation == null) //means there was no overlapping region
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
            truncatedHemisphere.SetActive(true);
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
            truncatedHemisphere.SetActive(true);
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
}
