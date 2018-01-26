using UnityEngine;
using System.Collections;
using System;

[Serializable]
public struct Coord
{
    public int x, y;

    public Coord(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public Coord(Coord c)
    {
        x = c.x;
        y = c.y;
    }

    public Coord(Vector2 vec)
    {
        x = Mathf.RoundToInt(vec.x);
        y = Mathf.RoundToInt(vec.y);
    }
	
	public Vector2 ToVector()
    {
        return new Vector2(x, y);
    }

    public Vector3 ToVector3()
    {
        return new Vector3(x, y);
    }

    public Vector3 ToVector3Z()
    {
        return new Vector3(x, 0, y);
    }

    public float GetSqrMagnitude()
    {
        return new Vector2(x, y).sqrMagnitude;
    }

    public float GetMagnitude()
    {
        return new Vector2(x, y).magnitude;
    }

    public Vector2 GetNormalized()
    {
        return new Vector2(x, y).normalized;
    }

    public override string ToString()
    {
        return "Coord(" + x + ", " + y + ")";
    }

    public static Coord FromVectorFloored(Vector2 vec)
    {
        return new Coord(Mathf.FloorToInt(vec.x), Mathf.FloorToInt(vec.y));
    }

    #region from or to index
    public static Coord IndexToCoord(int i, int width)
    {
        int x = 0;
        int y = 0;
        while (i >= width)
        {
            i -= width;
            y++;
        }
        x = i;

        return new Coord(x, y);
    }

    public static int CoordToIndex(Coord c, int width)
    {
        return c.x + c.y * width;
    }

    public static int CoordToIndex(int x, int y, int width)
    {
        return CoordToIndex(new Coord(x, y), width);
    }
    #endregion

    #region operators
    public static Coord operator +(Coord c1, Coord c2)
    {
        return new Coord(c1.x + c2.x, c1.y + c2.y);
    }

    public static Coord operator -(Coord c1, Coord c2)
    {
        return new Coord(c1.x - c2.x, c1.y - c2.y);
    }

    public bool IsEqualTo(Coord other)
    {
        return x == other.x && y == other.y;
    }
    #endregion

    public static readonly Coord[] neighbours = new Coord[8] {
        new Coord(0, 1), new Coord(1, 1), new Coord(1, 0), new Coord(1, -1),
        new Coord(0, -1), new Coord(-1, -1), new Coord(-1, 0), new Coord(-1, 1)
    };
}