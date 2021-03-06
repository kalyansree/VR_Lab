﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vectrosity;
/*
 * This script is applied to the delete Sphere, and is used to delete either existing spheres or existing lines
 */
public class DeleteObject : MonoBehaviour
{

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

    //Granularity of grid
    //e.g 0.25 means the cube will have four grid lines (including edges) running down each dimension of the cube 
    private float gridGranularity;

    //Vector3 that contains the coordinates to the closest point on the grid system to the gameObject (sphere on right controller)
    //Is set by getClosestPoint() function
    private Vector3 closestPoint;

    //Indicates whether the sphere is allowed to be placed. 
    //Not 100% reliable, should always double check conditions for placing.
    private bool allowPlacing;

    //Line used to show lines that will be deleted
    private VectorLine delLine;

    //--- PUBLIC VARIABLES ---//

    [Tooltip("GameObject of the Domain Cube")]
    public GameObject domain;    
    [Tooltip("GameObject of Right Controller")]
    public GameObject RightController;
    [Tooltip("GameObject of Preview Sphere")]
    public GameObject preview;
    [Tooltip("GameObject of Networking")]
    public GameObject Networking;
    [Tooltip("GameObject of CenterEyeAnchor")]
    public GameObject myCamera;


    void Start()
    {
        preview.SetActive(false); //disable until we need it
        gridGranularity = (float)(1m / 20m);
        originSet = false;
        isColliding = false;
        allowPlacing = false;
        delLine = new VectorLine("Delete", new List<Vector3>(), 6.0f, LineType.Discrete);
        VectorLine.SetCamera3D(myCamera);
        delLine.Draw3DAuto();
        
    }
    void Update()
    {
        unHighlightLines();
        //We show the preview once the controller is close enough to the cube and we aren't colliding with an existing sphere

        //isColliding = checkColliding();
        isColliding = checkCollidingVerTwo();

        if (OVRInput.GetDown(OVRInput.Button.One)) //Places the initial sphere
        {
            if (isColliding)
            {
                originSphere = currCollidingObj;
                originSet = true;
                delLine.points3.Add(originSphere.transform.position);
                delLine.points3.Add(originSphere.transform.position);
                delLine.SetColor(Color.red);
            }
        }
        if (OVRInput.Get(OVRInput.Button.One) && originSet)
        {
            origin = originSphere.transform.position;
            dest = gameObject.transform.position;
            if (origin != Vector3.zero && dest != Vector3.zero)
            {
                delLine.points3[0] = origin;
                delLine.points3[1] = dest;
            }
            if(isColliding && origin != dest)
            {
                highlightLine();
                delLine.active = false;
            }
            else
            {

                delLine.active = true;
                unHighlightLines();
            }
        }

        if (OVRInput.GetUp(OVRInput.Button.One)&& originSet)
        {
            delLine.points3.RemoveAt(0);
            delLine.points3.RemoveAt(0); //we do this twice to get rid of both points
            if (isColliding && currCollidingObj.transform.position != originSphere.transform.position)
            {
                deleteLine();
            }
            else if(isColliding)
            {
                if(currCollidingObj.CompareTag("Force"))
                {
                    deleteForcePoint(originSphere);
                }
                else if(currCollidingObj.CompareTag("Input") || currCollidingObj.CompareTag("Output"))
                {
                    GameObject force = currCollidingObj.GetComponent<InputOutputInfo>().GetForcePoint();
                    if(force != null)
                        deleteForcePoint(force);
                    deletePoint();
                }
                else
                {
                    deletePoint();
                }
            }
            originSet = false;
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
    void deletePoint()
    {
        if (originSet == false)
            return;
        ((Networking)Networking.GetComponent(typeof(Networking))).removeFromList(originSphere);

        for (int i = 0; i < ((InitLines)domain.GetComponent(typeof(InitLines))).lineTransformList.Count; i++)
        {
            if(((InitLines)domain.GetComponent(typeof(InitLines))).lineTransformList[i].position == originSphere.transform.position)
            {
                if(i % 2 == 0)
                {
                    ((InitLines)domain.GetComponent(typeof(InitLines))).lineTransformList.RemoveAt(i + 1);
                    ((InitLines)domain.GetComponent(typeof(InitLines))).lineTransformList.RemoveAt(i);

                    ((InitLines)domain.GetComponent(typeof(InitLines))).mainLine.points3.RemoveAt(i + 1);
                    ((InitLines)domain.GetComponent(typeof(InitLines))).mainLine.points3.RemoveAt(i);

                    i -= 1;

                }
                else
                {
                    ((InitLines)domain.GetComponent(typeof(InitLines))).lineTransformList.RemoveAt(i);
                    ((InitLines)domain.GetComponent(typeof(InitLines))).lineTransformList.RemoveAt(i - 1);

                    ((InitLines)domain.GetComponent(typeof(InitLines))).mainLine.points3.RemoveAt(i);
                    ((InitLines)domain.GetComponent(typeof(InitLines))).mainLine.points3.RemoveAt(i - 1);

                    i -= 2;
                }
            }
        }
        foreach (Transform childTransform in domain.transform)
        {
            if(childTransform.position == originSphere.transform.position)
            {
                Destroy(childTransform.gameObject);
            }

            if (childTransform.gameObject.CompareTag("Input") || childTransform.gameObject.CompareTag("Output"))
            {
                childTransform.gameObject.GetComponent<InputOutputInfo>().removeConnection(originSphere);
            }

            if (childTransform.CompareTag("Intermediate"))
            {
                childTransform.GetComponent<IntermediateInfo>().removeConnection(originSphere);
            }
        }

    }

    void deleteLine()
    {
        if (originSet == false)
            return;
        for (int i = 0; i < ((InitLines)domain.GetComponent(typeof(InitLines))).lineTransformList.Count - 1; i++)
        {
            
            //Very long condition... basically means we found the line we were looking for
            if ((((InitLines)domain.GetComponent(typeof(InitLines))).lineTransformList[i].position == originSphere.transform.position && 
                ((InitLines)domain.GetComponent(typeof(InitLines))).lineTransformList[i + 1].position == currCollidingObj.transform.position) ||
                (((InitLines)domain.GetComponent(typeof(InitLines))).lineTransformList[i].position == currCollidingObj.transform.position &&
                ((InitLines)domain.GetComponent(typeof(InitLines))).lineTransformList[i + 1].position == originSphere.transform.position))
            {
                ((InitLines)domain.GetComponent(typeof(InitLines))).lineTransformList.RemoveAt(i + 1);
                ((InitLines)domain.GetComponent(typeof(InitLines))).lineTransformList.RemoveAt(i);

                ((InitLines)domain.GetComponent(typeof(InitLines))).mainLine.points3.RemoveAt(i + 1);
                ((InitLines)domain.GetComponent(typeof(InitLines))).mainLine.points3.RemoveAt(i);
                break;
            } 
        }
        if(originSphere.CompareTag("Input") || originSphere.CompareTag("Output"))
        {
            originSphere.GetComponent<InputOutputInfo>().removeConnection(currCollidingObj);
        }
        if (currCollidingObj.CompareTag("Input") || currCollidingObj.CompareTag("Output"))
        {
            currCollidingObj.GetComponent<InputOutputInfo>().removeConnection(originSphere);
        }

        if (originSphere.CompareTag("Intermediate"))
        {
            originSphere.GetComponent<IntermediateInfo>().removeConnection(currCollidingObj);
        }
        if (currCollidingObj.CompareTag("Intermediate"))
        {
            currCollidingObj.GetComponent<IntermediateInfo>().removeConnection(originSphere);
        }
    }

    void highlightLine()
    {
        if (originSet == false)
            return;
        for (int i = 0; i < ((InitLines)domain.GetComponent(typeof(InitLines))).lineTransformList.Count - 1; i++)
        {

            //Very long condition... basically means we found the line we were looking for
            if ((((InitLines)domain.GetComponent(typeof(InitLines))).lineTransformList[i].position == originSphere.transform.position &&
                ((InitLines)domain.GetComponent(typeof(InitLines))).lineTransformList[i + 1].position == currCollidingObj.transform.position) ||
                (((InitLines)domain.GetComponent(typeof(InitLines))).lineTransformList[i].position == currCollidingObj.transform.position &&
                ((InitLines)domain.GetComponent(typeof(InitLines))).lineTransformList[i + 1].position == originSphere.transform.position))
            {
                ((InitLines)domain.GetComponent(typeof(InitLines))).mainLine.SetColor(Color.red, i/2);
                break;
            }
        }

    }

    void unHighlightLines()
    {
        ((InitLines)domain.GetComponent(typeof(InitLines))).mainLine.SetColor(Color.white);

    }

    void deleteForcePoint(GameObject forcePoint)
    {
        if (originSet == false)
            return;
        ((Networking)Networking.GetComponent(typeof(Networking))).forceTransformList.Remove(forcePoint.transform);
        VectorLine vline = forcePoint.GetComponent<ForcePointInfo>().GetLine();
        VectorLine.Destroy(ref vline);
        GameObject origin = forcePoint.GetComponent<ForcePointInfo>().GetConnectedPoint();
        origin.GetComponent<InputOutputInfo>().DeleteForcePoint();
        Destroy(forcePoint);
    }

    private bool checkColliding()
    {
        getClosestPoint();
        preview.transform.position = closestPoint;
        preview.transform.localScale = gameObject.transform.lossyScale;
        preview.SetActive(true);
        allowPlacing = true;
        bool colliding = false;
        //check if our preview is colliding with a placed sphere
        foreach (Transform transform in ((Networking)Networking.GetComponent(typeof(Networking))).allTransformList)
        {
            //print(dist);
            if (transform.position == closestPoint)
            {
                preview.SetActive(false);
                colliding = true;
                currCollidingObj = transform.gameObject;
                Color color = transform.gameObject.GetComponent<Renderer>().material.color;
                color.a = 1;
                ((Renderer)transform.gameObject.GetComponent<Renderer>()).material.color = color;
            }
            else
            {
                Color color = transform.gameObject.GetComponent<Renderer>().material.color;
                color.a = 0.353F;
                transform.gameObject.GetComponent<Renderer>().material.color = color;
            }
        }

        foreach (Transform transform in ((Networking)Networking.GetComponent(typeof(Networking))).forceTransformList)
        {
            //print(dist);
            if (transform.position == closestPoint)
            {
                preview.SetActive(false);
                colliding = true;
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
        return colliding;
    }

    private bool checkCollidingVerTwo()
    {
        unHighlightAll();
        preview.transform.position = transform.position;
        allowPlacing = true;
        int layerMask = 1 << LayerMask.NameToLayer("point");
        Collider[] colliderList = Physics.OverlapSphere(transform.position, transform.lossyScale.x / 2, layerMask);
        if (colliderList.Length == 0)
        {
            return false;
        }
        else if(colliderList.Length == 1) //means we are colliding with multiple points, which can happen
        {
            currCollidingObj = colliderList[0].gameObject;  
            Color color = currCollidingObj.GetComponent<Renderer>().material.color;
            color.a = 1;
            currCollidingObj.GetComponent<Renderer>().material.color = color;
            return true;
        }
        else if (colliderList.Length > 1)
        {
            currCollidingObj = FindClosestPoint(colliderList, transform.gameObject);
            Color color = currCollidingObj.GetComponent<Renderer>().material.color;
            color.a = 1;
            currCollidingObj.GetComponent<Renderer>().material.color = color;
            return true;
        }

        return false;
    }

    private GameObject FindClosestPoint(Collider[] colliderList, GameObject obj)
    {
        float closestDistance = 100;
        GameObject closestObj = null;

        foreach(Collider col in colliderList)
        {
            float currDistance = Vector3.Distance(obj.transform.position, col.transform.position);
            if (currDistance < closestDistance)
            {
                closestDistance = currDistance;
                closestObj = col.gameObject;
            }
        }
        return closestObj;

    }

    private void unHighlightAll()
    {
        foreach (Transform transform in ((Networking)Networking.GetComponent(typeof(Networking))).allTransformList)
        {
            Color color = transform.gameObject.GetComponent<Renderer>().material.color;
            color.a = 0.353F;
            transform.gameObject.GetComponent<Renderer>().material.color = color;
        }

        foreach (Transform transform in ((Networking)Networking.GetComponent(typeof(Networking))).forceTransformList)
        {
            Color color = ((Renderer)transform.gameObject.GetComponent<Renderer>()).material.color;
            color.a = 0.353F;
            ((Renderer)transform.gameObject.GetComponent<Renderer>()).material.color = color;
        }
    }
}
