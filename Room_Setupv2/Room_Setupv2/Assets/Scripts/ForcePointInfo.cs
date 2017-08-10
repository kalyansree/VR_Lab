using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForcePointInfo : MonoBehaviour {
    private GameObject connectedPoint;
	// Use this for initialization
	public void SetConnectedPoint(GameObject connectedPt)
    {
        this.connectedPoint = connectedPt;
    }

    public GameObject GetConnectedPoint()
    {
        return this.connectedPoint;
    }
}
