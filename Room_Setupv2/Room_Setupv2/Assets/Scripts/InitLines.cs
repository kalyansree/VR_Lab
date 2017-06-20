using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vectrosity;

public class InitLines : MonoBehaviour {
    [Tooltip("GameObject containing Camera (CenterEyeAnchor)")]
    public Camera myCamera;

    public VectorLine mainLine; //RightController scripts add on to mainLine
    public List<Transform> transformList;

    // Use this for initialization
    void Start () {
        VectorLine.SetCamera3D(myCamera);
        //Wireframe of cube
        VectorLine line = new VectorLine("Wireframe", new List<Vector3>(), 1.0f, LineType.Discrete);
        Mesh cubeMesh = ((MeshFilter)gameObject.GetComponent("MeshFilter")).mesh;
        line.MakeWireframe(cubeMesh);
        line.Draw3DAuto();
        line.drawTransform = gameObject.transform;

        //main line
        mainLine = new VectorLine("MainLine", new List<Vector3>(), 4.0f);
        mainLine.Draw3DAuto();

        transformList = new List<Transform>();
    }

    void LateUpdate()
    {
        int i = 0;
        foreach (Transform transform in transformList)
        {
            mainLine.points3[i] = transform.position;
            i++;
        }
    }
}
