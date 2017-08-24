using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vectrosity;

public class ForcePointInfo : MonoBehaviour {
    private GameObject IOPoint;
    bool dirTowardOrigin;
    private VectorLine vectorLine;
    bool setupComplete;

    void Update()
    {
        if (setupComplete)
        { 
            if (dirTowardOrigin)
            {
                vectorLine.points3[0] = IOPoint.transform.position;
                vectorLine.points3[1] = transform.position;
            }
            else
            {
                vectorLine.points3[0] = transform.position;
                vectorLine.points3[1] = IOPoint.transform.position;
            }
        }
    }
    public void Setup(GameObject connectedPt, VectorLine vectorLn, bool towardOrigin)
    {
        this.IOPoint = connectedPt;
        this.vectorLine = vectorLn;
        this.dirTowardOrigin = towardOrigin;
        setupComplete = true;
    }

    public GameObject GetConnectedPoint()
    {
        return this.IOPoint;
    }

    public VectorLine GetLine()
    {
        return this.vectorLine;
    }
}
