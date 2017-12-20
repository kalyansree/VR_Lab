using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCanvas : MonoBehaviour {
    public Camera myCamera;
    public GameObject Controller;
    public float xOffset;
    public float yOffset;
    public float zOffset;

    private Vector3 offset;
    // Update is called once per frame

    void Start()
    {
        offset = new Vector3(xOffset, yOffset, zOffset);
    }
    void Update () {
        Vector3 v = myCamera.transform.position - transform.position;

        v.x = v.z = 0.0f;

        transform.LookAt(myCamera.transform.position - v);

        transform.rotation = (myCamera.transform.rotation); // Take care about camera rotation

        transform.position = Controller.transform.position + offset * Controller.transform.localScale.x;
    }
}
