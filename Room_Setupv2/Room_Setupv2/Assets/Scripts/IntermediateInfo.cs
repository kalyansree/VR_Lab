using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntermediateInfo : MonoBehaviour {

    private GameObject intermediatePoint;
    private GameObject connection1;
    private GameObject connection2;

    private GameObject hemisphere1;
    private GameObject hemisphere2;
    private GameObject truncatedHemisphere;

    private Material hemisphereMaterial1;
    private Material hemisphereMaterial2;
    private Material truncatedHemisphereMaterial;

    private bool flipDirection1;
    private bool flipDirection2;

    private bool connection1Set;
    private bool connection2Set;

    private bool hemisphereOn;
    private bool truncationOn;

    private bool viability;

    public void SetupMaterials(Material hemisphereMat1, Material hemisphereMat2, Material truncatedHemisphereMat)
    {
        this.hemisphereMaterial1 = hemisphereMat1;
        this.hemisphereMaterial2 = hemisphereMat2;
        this.truncatedHemisphereMaterial = truncatedHemisphereMat;
    }

    public void SetPoint(GameObject intermediatePoint)
    {
        this.intermediatePoint = intermediatePoint;
    }

    public bool AddConnection(GameObject connection, GameObject hemisphere)
    {
        if(connection1Set && connection2Set)
        {
            return false;
        }
        else if(connection1Set)
        {
            connection2Set = true;
            connection2 = connection;
            hemisphere2 = hemisphere;
        }
        else
        {
            connection1Set = true;
            connection1 = connection;
            hemisphere1 = hemisphere;
        }
        return true;
    }

    public void SetTruncation(GameObject truncatedHemi)
    {
        truncatedHemisphere = truncatedHemi;
    }

    public bool RemoveConnection(GameObject connection)
    {
        if(!connection1Set && !connection2Set)
        {
            return false;
        }
        else if(!connection1Set)
        {
            if(checkConnection(connection, connection2))
            {
                truncatedHemisphere = null;
                truncationOn = false;
                connection2Set = false;
                connection2 = null;
                hemisphere2 = null;
                return true;
            }
            return false;
        }
        else if(!connection2Set)
        {
            if (checkConnection(connection, connection1))
            {
                truncatedHemisphere = null;
                truncationOn = false;
                connection1Set = false;
                connection1 = null;
                hemisphere1 = null;
                return true;
            }
            return false;
        }
        else
        {
            if(checkConnection(connection, connection1))
            {
                truncatedHemisphere = null;
                truncationOn = false;
                connection1Set = false;
                connection1 = null;
                hemisphere1 = null;
                return true;
            }
            else if(checkConnection(connection, connection2))
            {
                truncatedHemisphere = null;
                truncationOn = false;
                connection2Set = false;
                connection2 = null;
                hemisphere2 = null;
                return true;
            }
            return false;
        }
    }

    private bool checkConnection(GameObject connection, GameObject connectionPrime)
    {
        if (connection.CompareTag("Intermediate") && connection.transform.position == connectionPrime.transform.position)
        {
            return true;
        }
        return false;
    }

    public List<GameObject> GetConnections()
    {
        List<GameObject> retList = new List<GameObject>();
        if(connection1Set)
        {
            retList.Add(connection1);
        }
        if(connection2Set)
        {
            retList.Add(connection2);
        }
        return retList;
    }

    public void toggleHemisphereView()
    {
        if (hemisphereOn)
        {
            if(connection1Set)
            {
                hemisphere1.SetActive(false);
            }
            if(connection2Set)
            {
                hemisphere2.SetActive(false);
            }
            if(truncationOn)
            {
                truncatedHemisphere.SetActive(false);
            }
        }
        else
        {
            if (connection1Set)
            {
                hemisphere1.SetActive(true);
            }
            if (connection2Set)
            {
                hemisphere2.SetActive(true);
            }
            if (!truncationOn)
            {
                truncatedHemisphere.SetActive(true);
            }
        }
    }

}
