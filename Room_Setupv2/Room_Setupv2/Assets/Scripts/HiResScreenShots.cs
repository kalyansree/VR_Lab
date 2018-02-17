using UnityEngine;
using System.Collections;

public class HiResScreenShots : MonoBehaviour
{
    int resWidth = 2160;
    int resHeight = 1200;

    public GameObject XPlane;
    public GameObject YPlane;
    public GameObject ZPlane;
    //public GameObject IsoPlane;

    GameObject[] planeArr;

    void Start()
    {
        planeArr = new GameObject[4];
        planeArr[0] = XPlane;
        planeArr[1] = YPlane;
        planeArr[2] = ZPlane;
        //planeArr[3] = IsoPlane;
    }
    public static string ScreenShotName(int width, int height, string name)
    {
        return string.Format("{0}/Screenshots/{1}_{2}x{3}_{4}.png",
                              Application.dataPath,
                              name,
                              width, height,
                              System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
    }

    void LateUpdate()
    {
        if (Input.GetKeyUp("k"))
        {
            takePics();
        }
    }

    void takePics()
    {
        foreach (GameObject go in planeArr)
        {
            RenderTexture tex = (RenderTexture)go.GetComponent<Renderer>().material.mainTexture;
            int width = (int)tex.width;
            int height = (int)tex.height;
            RenderTexture.active = tex;
            Texture2D screenShot = new Texture2D(width, height, TextureFormat.RGB24, false);
            screenShot.ReadPixels(new Rect(0,0, width, height), 0, 0);
            byte[] bytes = screenShot.EncodeToPNG();
            string filename = ScreenShotName(width, height, go.name);
            print(filename);
            System.IO.File.WriteAllBytes(filename, bytes);

        }

        //Application.CaptureScreenshot(ScreenShotName(resWidth, resHeight, "POV"), 7);
    }
}