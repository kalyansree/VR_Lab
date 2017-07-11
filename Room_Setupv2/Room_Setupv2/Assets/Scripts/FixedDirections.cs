using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedDirections : MonoBehaviour {
    public const int X = 0;
    public const int Y = 1;
    public const int Z = 2;

    public bool fixedX;
    public bool fixedY;
    public bool fixedZ;

    public void copy(GameObject other)
    {
        FixedDirections fd = other.AddComponent(typeof(FixedDirections)) as FixedDirections;
        fd.fixedX = this.fixedX;
        fd.fixedY = this.fixedY;
        fd.fixedZ = this.fixedZ;
    }

    public void fixDirection(int dir)
    {
        switch(dir)
        {
            case X:
                this.fixedX = true;
                break;
            case Y:
                this.fixedY = true;
                break;
            case Z:
                this.fixedZ = true;
                break;
        }
    }

    public void unfixDirection(int dir)
    {
        switch (dir)
        {
            case X:
                this.fixedX = false;
                break;
            case Y:
                this.fixedY = false;
                break;
            case Z:
                this.fixedZ = false;
                break;
        }
    }

    public void toggleDirection(int dir)
    {
        switch (dir)
        {
            case X:
                this.fixedX = this.fixedX ? false : true;
                break;
            case Y:
                this.fixedY = this.fixedY ? false : true;
                break;
            case Z:
                this.fixedZ = this.fixedZ ? false : true;
                break;
        }
    }
}
