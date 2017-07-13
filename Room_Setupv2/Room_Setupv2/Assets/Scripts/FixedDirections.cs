using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FixedDirections : MonoBehaviour {
    public const int X = 0;
    public const int Y = 1;
    public const int Z = 2;

    public bool fixedX;
    public bool fixedY;
    public bool fixedZ;

    public GameObject RightPanel;
    public void copy(GameObject other)
    {
        FixedDirections fd = other.AddComponent(typeof(FixedDirections)) as FixedDirections;
        fd.fixedX = this.fixedX;
        fd.fixedY = this.fixedY;
        fd.fixedZ = this.fixedZ;
    }

    public void toggleDirection(int dir)
    {
        switch (dir)
        {
            case X:
                this.fixedX = this.fixedX ? false : true;
                toggleColor(dir, this.fixedX);
                break;
            case Y:
                this.fixedY = this.fixedY ? false : true;
                toggleColor(dir, this.fixedY);
                break;
            case Z:
                this.fixedZ = this.fixedZ ? false : true;
                toggleColor(dir, this.fixedZ);
                break;
        }
    }

    private void toggleColor(int dir, bool fix)
    {
        ColorBlock colors = RightPanel.transform.GetChild(dir).gameObject.GetComponent<Button>().colors;
        if (fix)
        {
            colors.normalColor = Color.red;
            colors.highlightedColor = Color.yellow;
        }
            
        else
        {
            colors.normalColor = Color.white;
            colors.highlightedColor = Color.gray;
        }

        RightPanel.transform.GetChild(dir).gameObject.GetComponent<Button>().colors = colors;
    }

    public void reapplyColor()
    {
        if(fixedX)
        {
            ColorBlock colors = RightPanel.transform.GetChild(X).gameObject.GetComponent<Button>().colors;
            colors.normalColor = Color.red;
            colors.highlightedColor = Color.yellow;
            RightPanel.transform.GetChild(X).gameObject.GetComponent<Button>().colors = colors;
        }
        else if(!fixedX)
        {
            ColorBlock colors = RightPanel.transform.GetChild(X).gameObject.GetComponent<Button>().colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = Color.gray;
            RightPanel.transform.GetChild(X).gameObject.GetComponent<Button>().colors = colors;
        }
        if(fixedY)
        {
            ColorBlock colors = RightPanel.transform.GetChild(Y).gameObject.GetComponent<Button>().colors;
            colors.normalColor = Color.red;
            colors.highlightedColor = Color.yellow;
            RightPanel.transform.GetChild(Y).gameObject.GetComponent<Button>().colors = colors;
        }
        else if(!fixedY)
        {
            ColorBlock colors = RightPanel.transform.GetChild(Y).gameObject.GetComponent<Button>().colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = Color.gray;
            RightPanel.transform.GetChild(Y).gameObject.GetComponent<Button>().colors = colors;
        }
        if (fixedZ)
        {
            ColorBlock colors = RightPanel.transform.GetChild(Z).gameObject.GetComponent<Button>().colors;
            colors.normalColor = Color.red;
            colors.highlightedColor = Color.yellow;
            RightPanel.transform.GetChild(Z).gameObject.GetComponent<Button>().colors = colors;
        }
        else if (!fixedZ)
        {
            ColorBlock colors = RightPanel.transform.GetChild(Z).gameObject.GetComponent<Button>().colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = Color.gray;
            RightPanel.transform.GetChild(Z).gameObject.GetComponent<Button>().colors = colors;
        }

    }
}
