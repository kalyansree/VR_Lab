using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObject : MonoBehaviour { 
    private GameObject originSphere;
    private Vector3 origin;
    private Vector3 dest;
    private bool originSet = false;
    void Start()
    {
        LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
        initLine(lineRenderer);

    }
    // Update is called once per frame
    void Update()
    {
        var distToCube = Vector3.Distance(GameObject.Find("Domain").GetComponent<Collider>().ClosestPoint(gameObject.transform.position), gameObject.transform.position);
        if (OVRInput.GetDown(OVRInput.Button.One) && distToCube == 0) //Places the initial sphere
        {
            originSet = true;
            originSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            originSphere.transform.position = gameObject.transform.position;
            originSphere.transform.rotation = gameObject.transform.rotation;
            originSphere.transform.localScale = gameObject.transform.localScale;
            Renderer rend = originSphere.GetComponent<Renderer>();
            if (rend != null)
            {
                rend.material = gameObject.GetComponent<Renderer>().material;
            }
            originSphere.transform.SetParent(GameObject.Find("Domain").transform, true);
        }
        if (OVRInput.Get(OVRInput.Button.One) && originSet)
        {
            LineRenderer lineRenderer = GetComponent<LineRenderer>();
            origin = originSphere.transform.position;
            dest = gameObject.transform.position;
            if (origin != Vector3.zero && dest != Vector3.zero)
            {
                lineRenderer.SetPosition(0, origin);
                lineRenderer.SetPosition(1, dest);
            }
        }

        if (OVRInput.GetUp(OVRInput.Button.One) && originSet)
        {
            originSet = false;
            GameObject destSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            destSphere.transform.position = gameObject.transform.position;
            destSphere.transform.rotation = gameObject.transform.rotation;
            destSphere.transform.localScale = gameObject.transform.localScale;
            Renderer rend = destSphere.GetComponent<Renderer>();
            if (rend != null)
            {
                rend.material = gameObject.GetComponent<Renderer>().material;
            }
            destSphere.transform.SetParent(GameObject.Find("Domain").transform, true);

            //line
            
            LineRenderer lineRenderer = GameObject.Find("Domain").AddComponent<LineRenderer>();
            initLine(lineRenderer);
            lineRenderer.SetPosition(0, origin);
            lineRenderer.SetPosition(1, dest);
            lineRenderer.useWorldSpace = false;
        }


    }
    LineRenderer initLine(LineRenderer lineRenderer)
    {
        Color c1 = Color.yellow;
        Color c2 = Color.red;
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
        return lineRenderer;
    }
}
