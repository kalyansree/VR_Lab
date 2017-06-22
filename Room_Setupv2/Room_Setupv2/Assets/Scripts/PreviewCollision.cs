using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewCollision : MonoBehaviour {
    public bool isColliding;
    public GameObject currCollidingObj;
 
    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Point")) //make sure it is a placed point
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
        if (other.gameObject.CompareTag("Point")) //make sure it is a placed point
        {
            isColliding = false;
            Color color = ((Renderer)other.gameObject.GetComponent<Renderer>()).material.color;
            color.a = 0.353F;
            ((Renderer)other.gameObject.GetComponent<Renderer>()).material.color = color;
        }
    }
}
