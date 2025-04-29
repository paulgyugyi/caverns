using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Hexmap
{
    public static int ClusterSize = 7;

    public static List<Vector3Int> GetNeighbors(Vector3Int center)
    {
        List<Vector3Int> offsets = new List<Vector3Int>();

        for (int i = 1; i < 7; i++)
        {
            offsets.Add(GetNeighbor(center, i));
        }
        return offsets;
    }

    public static Vector3Int GetNeighbor(Vector3Int center, int idx)
    {
        Vector3Int offset = center;
        switch(idx)
        {
            case 0:
                break;
            case 1:
                offset += new Vector3Int(center.y % 2, 1, 0);
                break;
            case 2:
                offset += new Vector3Int(1, 0, 0);
                break;
            case 3:
                offset += new Vector3Int(center.y % 2, -1, 0);
                break;
            case 4:
                offset += new Vector3Int(-1 + (center.y % 2), -1, 0);
                break;
            case 5:
                offset += new Vector3Int(-1, 0, 0);
                break;
            case 6:
                offset += new Vector3Int(-1 + (center.y % 2), 1, 0);
                break;
        }
        return offset;
    }

    public static bool InBounds(int startHex, int moveDirection, out int endHex)
    {
        //Debug.Log("InBounds startHex: " + startHex + " moveDirection: " + moveDirection);
        if (moveDirection == 0 )
        {
            endHex = startHex;
            //Debug.Log("endHex: " + endHex);
            return true;
        }
        endHex = -1;
        switch (startHex)
        {
            case 0:
                endHex = moveDirection;
                break;
            case 1:
                switch (moveDirection)
                {
                    case 3:
                        endHex = 2; break;
                    case 4:
                        endHex = 0; break;
                    case 5:
                        endHex = 6; break;
                }
                break;
            case 2:
                switch (moveDirection)
                {
                    case 4:
                        endHex = 3; break;
                    case 5:
                        endHex = 0; break;
                    case 6:
                        endHex = 1; break;    
                }
                break;
            case 3:
                switch (moveDirection)
                {
                    case 5:
                        endHex = 4; break;
                    case 6:
                        endHex = 0; break;
                    case 1:
                        endHex = 2; break;
                }
                break;
            case 4:
                switch (moveDirection)
                {
                    case 6:
                        endHex = 5; break;
                    case 1:
                        endHex = 0; break;
                    case 2:
                        endHex = 3; break;
                }
                break;
            case 5:
                switch (moveDirection)
                {
                    case 1:
                        endHex = 6; break;
                    case 2:
                        endHex = 0; break;
                    case 3:
                        endHex = 4; break;
                }
                break;
            case 6:
                switch (moveDirection)
                {
                    case 2:
                        endHex = 1; break;
                    case 3:
                        endHex = 0; break;
                    case 4:
                        endHex = 5; break;
                }
                break;
        }
        if (endHex == -1)
        {
            //Debug.Log("Out of bounds");
            return false;
        }
        else
        {
            //Debug.Log("endHex: " + endHex);
            return true;
        }
    }

    public static int Opposite(int hex)
    {
        int opposite = 0;
        if (hex > 0)
        {
            opposite = hex + 3;
            if (opposite > 6)
            {
                opposite -= 6;
            }
        }
        return opposite;
    }
}
