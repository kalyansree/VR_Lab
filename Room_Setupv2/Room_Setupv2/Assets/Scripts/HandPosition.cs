using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandPosition : MonoBehaviour {
    public Color c1 = Color.yellow;
    public Color c2 = Color.red;
    public OVRInput.Controller LController = OVRInput.Controller.LTouch;
    public OVRInput.Controller RController = OVRInput.Controller.RTouch;
    void Start()
    {
        LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
        lineRenderer.widthMultiplier = 0.001f;
        lineRenderer.positionCount = 2;

        float alpha = 1.0f;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(c1, 0.0f), new GradientColorKey(c2, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
            );
        lineRenderer.colorGradient = gradient;

    }
    // Update is called once per frame
    void Update () {
        LineRenderer lineRenderer = GetComponent<LineRenderer>();
        Vector3 Lcontroller_loc = OVRInput.GetLocalControllerPosition(LController);
        Vector3 Rcontroller_loc = OVRInput.GetLocalControllerPosition(RController);
        if(Lcontroller_loc != Vector3.zero && Rcontroller_loc != Vector3.zero)
        {
            lineRenderer.SetPosition(0, Lcontroller_loc);
            lineRenderer.SetPosition(1, Rcontroller_loc);
        }
    }
}
